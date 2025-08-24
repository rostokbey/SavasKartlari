using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : NetworkBehaviour
{
    public static BattleManager Instance;

    [Header("Kart Prefabları ve Konumları")]
    public GameObject characterPrefab; // Varsayılan prefab (CardData.characterPrefab3D yoksa)
    public GameObject cardPrefab;      // Kart UI prefabı (elde/gösterimde)
    public Transform[] playerGridPositions;
    public Transform[] enemyGridPositions;

    [Header("Spawner Noktaları")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    [Header("UI")]
    public Button attackButton;
    public Button skillButton;
    public Transform enemyCardParent;
    public GameObject cardSlotPrefab;

    // (Opsiyonel) UI'dan doğrudan başlatmak istersen bu butonu sahneden bağlayabilirsin
    [Tooltip("İstersen sahneden bağla; bağlanmazsa da OnStartBattleButton() çağrılabilir.")]
    public Button startBattleButton;

    [Header("Turn Yönetimi")]
    public ulong currentTurnClientId;

    private readonly List<Character> allCharacters = new();
    private readonly Dictionary<ulong, List<string>> playerSubmittedCardIds = new();

    private List<Character> turnOrder = new();
    private int currentIndex = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {

        // TEST: Eğer ağ henüz başlamadıysa Host olarak başlat
        if (NetworkManager.Singleton &&
       !NetworkManager.Singleton.IsServer &&
       !NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("[BM] Test: Host başladı (Editor/tek cihaz testi).");
        }


        // UI buton bağlama (varsa)
        if (startBattleButton != null)
            startBattleButton.onClick.AddListener(OnStartBattleButton);

        //test sonrası sil !!!! 
        if (!IsServer) return;

        var playerCards = StartBattleManager.Instance?.selectedMatchCards;
        var enemyCards = StartBattleManager.Instance?.enemyMatchCards;

        Debug.Log($"[BM] Start: playerCards={playerCards?.Count}, enemyCards={enemyCards?.Count}");

        if (playerCards == null || enemyCards == null)
        {
            Debug.LogError("❌ BattleManager: Kartlar alınamadı.");
            return;
        }

        Debug.Log($"🟩 Oyuncu kartları sayısı: {playerCards.Count}");
        Debug.Log($"🟥 Düşman kartları sayısı: {enemyCards.Count}");

        // Oyuncu tarafının kart UI'larını göster
        SpawnPlayerCards(playerCards);

        // Düşman destesini UI'da göster ()
        //ShowEnemyDeck(enemyCards);
        // Kart listelerini aldıktan sonra:
        var handUI = FindObjectOfType<HandUIManager>(true);
        if (handUI != null && StartBattleManager.Instance != null)
        {
            var playerDeck = StartBattleManager.Instance.selectedMatchCards;

            // Sadece TEK çağrı! İkinciyi sil.
            // Eğer HandUIManager.cardUIPrefab'ı Inspector'dan zaten CardUI_BattlePrefab'a atadıysan:
            handUI.Init(playerDeck);

            // Eğer Inspector’da atamadıysan ve koddan vermek istiyorsan:
            // handUI.Init(playerDeck, cardUIBattlePrefabRef); // (GameObject referansı)
        }


        // 3D karakterleri sahneye bas deneme satrı sonra sil!!!
        SpawnCharacters(playerCards, enemyCards);

        if (NetworkManager.Singleton.ConnectedClientsList.Count > 0)
            currentTurnClientId = NetworkManager.Singleton.ConnectedClientsList[0].ClientId;

       
        

    }

    // ---------------------------------------------------------------------
    #region UI'dan Savaş Başlatma (opsiyonel)

    // UI'daki "SAVAŞ BAŞLAT" butonuna verebileceğin güvenli çağrı.
    public void OnStartBattleButton()
    {
        var playerCards = StartBattleManager.Instance?.selectedMatchCards;
        var enemyCards = StartBattleManager.Instance?.enemyMatchCards;

        if (playerCards == null || enemyCards == null)
        {
            Debug.LogWarning("StartBattle: Kart listeleri bulunamadı.");
            return;
        }

        // Sunucu değilsek sunucu zaten Start'ta basacaktır; tek cihaz testinde de çalışsın:
        if (IsServer)
            SpawnCharacters(playerCards, enemyCards);

        StartBattle();
    }

    #endregion
    // ---------------------------------------------------------------------

    #region Oyuncu Kart UI

    // ------------------------------
    // Oyuncu kartlarını UI'da gösterme (CardUI spawn)
    // ------------------------------
    public void SpawnPlayerCards(List<CardData> selectedCards)
    {
        for (int i = 0; i < selectedCards.Count && i < playerGridPositions.Length; i++)
        {
            var cardData = selectedCards[i];

            // CardUI prefabını grid konumuna instantiate et
            GameObject go = Instantiate(cardPrefab, playerGridPositions[i].position, Quaternion.identity);
            go.transform.SetParent(playerGridPositions[i], false); // parent + local sıfır

            var ui = go.GetComponent<CardUI>();
            if (ui != null)
                ui.SetCardData(cardData, false); // savaş sahnesinde butonlar gizli
        }
    }

    // ---------------------------------------------------------
    // 3D KARAKTER SPAWN (oyuncu + düşman) — hizalama iyileştirilmiş
    // ---------------------------------------------------------
    public void SpawnCharacters(List<CardData> playerCards, List<CardData> enemyCards)
    {
        if (!IsServer)
        {
            Debug.LogWarning("SpawnCharacters sadece sunucu tarafından çağrılabilir.");
            return;
        }

        // ---- Oyuncu ----
        for (int i = 0; i < playerCards.Count && i < playerGridPositions.Length; i++)
        {
            var card = playerCards[i];
            var grid = playerGridPositions[i];

            // Kart özel prefabı varsa onu, yoksa default'u kullan
            var prefabToUse = ResolvePrefab(card);
            if (prefabToUse == null) { Debug.LogWarning($"[BM] Prefab yok: {card.cardName}"); continue; }


            if (prefabToUse == characterPrefab)
            {
                Debug.LogWarning($"[Spawn] {card.cardName}: CARD PREFAB YOK → Default kullanılıyor! " +
                                 "QR prefab yolu/isimleri ve Resources konumunu kontrol et.");
            }


            Debug.Log($"[A] Oyuncu spawn -> {card.cardName}, Prefab: {(card.characterPrefab3D ? card.characterPrefab3D.name : "Default")}");

            var obj = Instantiate(prefabToUse, grid.position, grid.rotation);
            var net = obj.GetComponent<Unity.Netcode.NetworkObject>();

            // 1) Spawn
            if (net && Unity.Netcode.NetworkManager.Singleton && Unity.Netcode.NetworkManager.Singleton.IsServer)
                net.Spawn();

            // 2) Parent
            if (net && Unity.Netcode.NetworkManager.Singleton && Unity.Netcode.NetworkManager.Singleton.IsServer)
                net.TrySetParent(grid, false);
            else
                obj.transform.SetParent(grid, false);

            // 3) Lokal hizalama
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            var ch = obj.GetComponent<Character>();
            if (ch != null)
            {
                ch.Setup(card);
                allCharacters.Add(ch);
            }
        }

        // ---- Düşman ----
        for (int i = 0; i < enemyCards.Count && i < enemyGridPositions.Length; i++)
        {
            var card = enemyCards[i];
            var grid = enemyGridPositions[i];

            var prefabToUse = ResolvePrefab(card);
            if (prefabToUse == null) { Debug.LogWarning($"[BM] Prefab yok (enemy): {card.cardName}"); continue; }


            Debug.Log($"[B] Düşman spawn -> {card.cardName}, Prefab: {(card.characterPrefab3D ? card.characterPrefab3D.name : "Default")}");

            var obj = Instantiate(prefabToUse, grid.position, grid.rotation);
            var net = obj.GetComponent<Unity.Netcode.NetworkObject>();

            if (net && Unity.Netcode.NetworkManager.Singleton && Unity.Netcode.NetworkManager.Singleton.IsServer)
                net.Spawn();

            if (net && Unity.Netcode.NetworkManager.Singleton && Unity.Netcode.NetworkManager.Singleton.IsServer)
                net.TrySetParent(grid, false);
            else
                obj.transform.SetParent(grid, false);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f); // rakip bize dönsün
            obj.transform.localScale = Vector3.one;

            var ch = obj.GetComponent<Character>();
            if (ch != null)
            {
                ch.Setup(card);
                allCharacters.Add(ch);
            }
        }

        AssignTurnToClient(currentTurnClientId);
    }



    #endregion
    // ---------------------------------------------------------------------

    #region Turn Sistemi

    public void AssignTurnToClient(ulong clientId)
    {
        foreach (var ch in allCharacters)
        {
            bool isPlayer = ch.OwnerClientId == clientId;
            ch.SetTurn(isPlayer);
        }
    }

    public void StartBattle()
    {
        turnOrder = new List<Character>(FindObjectsOfType<Character>());
        currentIndex = 0;
        AssignTurn();
    }

    public void EndTurn()
    {
        if (turnOrder == null || turnOrder.Count == 0) return;

        turnOrder[currentIndex].SetTurn(false);
        currentIndex = (currentIndex + 1) % turnOrder.Count;
        AssignTurn();
    }

    void AssignTurn()
    {
        if (turnOrder == null || turnOrder.Count == 0) return;
        turnOrder[currentIndex].SetTurn(true);
    }

    #endregion
    // ---------------------------------------------------------------------

    #region RPC ve Kart Gönderme

    [ServerRpc(RequireOwnership = false)]
    public void SubmitDeckServerRpc(FixedString128Bytes[] cardIdArray, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;
        List<string> cardIds = cardIdArray.Select(id => id.ToString()).ToList();

        playerSubmittedCardIds[senderId] = cardIds;
        Debug.Log($"📨 {senderId} oyuncusu {cardIds.Count} kart gönderdi.");

        if (playerSubmittedCardIds.Count >= 2)
        {
            Debug.Log("🚀 Her iki oyuncudan deste geldi.");
            SpawnCharactersFromSubmittedDecks();
        }
    }

    private void SpawnCharactersFromSubmittedDecks()
    {
        var players = new List<ulong>(playerSubmittedCardIds.Keys);
        var deck1 = playerSubmittedCardIds[players[0]];
        var deck2 = playerSubmittedCardIds[players[1]];

        var deckManager = FindObjectOfType<DeckManagerObject>();

        var playerCards = deck1.Select(id => deckManager.GetCardById(id)).Where(c => c != null).ToList();
        var enemyCards = deck2.Select(id => deckManager.GetCardById(id)).Where(c => c != null).ToList();

        SpawnCharacters(playerCards, enemyCards);
        StartBattle();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendAttackServerRpc() => EndTurn();

    [ServerRpc(RequireOwnership = false)]
    public void SendSkillAttackServerRpc(string ability)
    {
        Debug.Log("Yetenek kullanıldı: " + ability);
        EndTurn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayCardServerRpc(string cardId, ServerRpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;

        var card = FindObjectOfType<DeckManagerObject>().GetCardById(cardId);
        if (card == null)
        {
            Debug.LogWarning("Kart bulunamadı: " + cardId);
            return;
        }

        // ✅ Kullanılacak prefab'ı seç
        GameObject prefabToUse = ResolvePrefab(card);
        if (prefabToUse == null) return;


        // ✅ Spawn pozisyonunu seç
        Vector3 spawnPos = senderClientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId
            ? playerSpawnPoint.position
            : enemySpawnPoint.position;

        // ✅ Instantiate et
        GameObject obj = Instantiate(prefabToUse, spawnPos, Quaternion.identity);

        // hizalama (spawn point child'ı değil; sahneye serbest)
        var ch = obj.GetComponent<Character>();
        if (ch != null) ch.Setup(card);

        obj.GetComponent<NetworkObject>()?.SpawnWithOwnership(senderClientId);
        allCharacters.Add(ch);
    }

    #endregion
    // ---------------------------------------------------------------------

    #region Victory

    public void CheckVictory()
    {
        int alive = 0;
        ulong winnerId = 0;

        foreach (var ch in allCharacters)
        {
            if (ch.hp > 0)
            {
                alive++;
                winnerId = ch.OwnerClientId;
            }
        }

        if (alive == 1)
            GrantRewardTo(winnerId);
    }

    public void GrantRewardTo(ulong winnerId)
    {
        bool isLocalPlayerWinner = (NetworkManager.Singleton.LocalClientId == winnerId);

        var usedCards = FindObjectOfType<DeckManagerObject>().currentMatchDeck;
        var xpManager = FindObjectOfType<MatchEndXPManager>();

        if (xpManager != null)
        {
            xpManager.GrantMatchRewards(isLocalPlayerWinner, usedCards);
        }

        Debug.Log(isLocalPlayerWinner ? "Kazandın! Ödül verildi." : "Kaybettin. Az da olsa XP verildi.");
    }

    #endregion

    private GameObject ResolvePrefab(CardData card)
    {
        // 1) JWT/CSV’den gelen Resources yolu
        if (!string.IsNullOrEmpty(card.prefab))
        {
            var res = Resources.Load<GameObject>(card.prefab);
            if (res != null) return res;
            Debug.LogWarning($"[BM] Resources prefab bulunamadı: {card.prefab}  (Kart: {card.cardName})");
        }

        // 2) Kart üstüne atalı prefab (opsiyonel)
        if (card.characterPrefab3D != null) return card.characterPrefab3D;

        // 3) Inspector’dan verdiğin default
        if (characterPrefab != null) return characterPrefab;

        Debug.LogError("[BM] Prefab seçilemedi: characterPrefab da atanmadı!");
        return null;
    }

    #region UI

    public void ShowEnemyDeck(List<CardData> enemyCards)
    {
        if (enemyCardParent == null || cardSlotPrefab == null) return;

        foreach (Transform child in enemyCardParent)
            Destroy(child.gameObject);

        foreach (CardData card in enemyCards)
        {
            GameObject slot = Instantiate(cardSlotPrefab, enemyCardParent);
            slot.transform.localScale = Vector3.one;

            CardUI ui = slot.GetComponent<CardUI>();
            if (ui != null)
                ui.SetCardData(card, false);
        }
    }

    

    #endregion
}
