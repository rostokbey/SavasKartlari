using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

public class BattleManager : NetworkBehaviour
{
    public static BattleManager Instance;

    [Header("Kart Prefabları ve Konumları")]
    public GameObject characterPrefab; // Karakter prefabı (spawn için)
    public GameObject cardPrefab; // CardUI prefabı (UI için)
    public Transform[] playerGridPositions;
    public Transform[] enemyGridPositions;

    [Header("Spawner Noktaları")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    [Header("UI")]
    public Button attackButton;
    public Button skillButton;
    public Transform enemyCardParent; // düşman kartlarının UI'ı burada gösterilecek
    public GameObject cardSlotPrefab; // düşman kartı için slot prefabı

    [Header("Turn Yönetimi")]
    public ulong currentTurnClientId;

    private List<Character> allCharacters = new();
    private Dictionary<ulong, List<string>> playerSubmittedCardIds = new();

    private List<Character> turnOrder = new();
    private int currentIndex = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        if (!IsServer) return;

        // StartBattleManager'dan kartları al
        var playerCards = StartBattleManager.Instance?.selectedMatchCards;
        var enemyCards = StartBattleManager.Instance?.enemyMatchCards;

        if (playerCards == null || enemyCards == null)
        {
            Debug.LogError("❌ BattleManager: Kartlar alınamadı.");
            return;
        }

        Debug.Log($"🟩 Oyuncu kartları sayısı: {playerCards.Count}");
        Debug.Log($"🟥 Düşman kartları sayısı: {enemyCards.Count}");

        // UI'da göster
        SpawnPlayerCards(playerCards);
        ShowEnemyDeck(enemyCards);

        // Karakterleri oluştur
        SpawnCharacters(playerCards, enemyCards);

        if (NetworkManager.Singleton.ConnectedClientsList.Count > 0)
            currentTurnClientId = NetworkManager.Singleton.ConnectedClientsList[0].ClientId;
    }


    #region Düşman Kartlarını Oluşturma

    void SpawnEnemyCards()
    {
        for (int i = 0; i < 3; i++)
        {
            CardData dummyEnemyCard = new CardData(
                "enemy_" + i,
                "Düşman " + (i + 1),
                100,
                20,
                "Rare",
                "Fire",
                "Shield",
                1,
                0,
                3,
                null
            );

            GameObject slot = Instantiate(cardSlotPrefab, enemyCardParent);
            CardUI ui = slot.GetComponent<CardUI>();
            if (ui != null)
            {
                ui.SetCardData(dummyEnemyCard);
            }
        }
    }

    #endregion

    #region Oyuncu Kartlarını Yerleştirme

    public void SpawnPlayerCards(List<CardData> selectedCards)
    {
        for (int i = 0; i < selectedCards.Count && i < playerGridPositions.Length; i++)
        {
            var cardData = selectedCards[i];
            GameObject go = Instantiate(cardPrefab, playerGridPositions[i].position, Quaternion.identity);
            go.transform.SetParent(playerGridPositions[i]);
            CardUI ui = go.GetComponent<CardUI>();
            if (ui != null)
                ui.SetCardData(cardData);
        }
    }

    #endregion

    #region Karakter Spawn & Turn Sistemi

    public void SpawnCharacters(List<CardData> playerCards, List<CardData> enemyCards)
    {
        if (!IsServer)
        {
            Debug.LogWarning("SpawnCharacters sadece sunucu tarafından çağrılabilir.");
            return;
        }
        for (int i = 0; i < playerCards.Count && i < playerGridPositions.Length; i++)
        {
            GameObject obj = Instantiate(characterPrefab, playerGridPositions[i].position, Quaternion.identity);
            obj.transform.localScale = Vector3.one;

            Character ch = obj.GetComponent<Character>();
            ch.Setup(playerCards[i]);
            allCharacters.Add(ch);
            obj.GetComponent<NetworkObject>()?.SpawnWithOwnership(NetworkManager.Singleton.ConnectedClientsList[0].ClientId);
        }

        for (int i = 0; i < enemyCards.Count && i < enemyGridPositions.Length; i++)
        {
            GameObject obj = Instantiate(characterPrefab, enemyGridPositions[i].position, Quaternion.identity);
            obj.transform.localScale = Vector3.one;

            Character ch = obj.GetComponent<Character>();
            ch.Setup(enemyCards[i]);
            allCharacters.Add(ch);
            obj.GetComponent<NetworkObject>()?.SpawnWithOwnership(NetworkManager.Singleton.ConnectedClientsList[1].ClientId);
        }

        AssignTurnToClient(currentTurnClientId);
    }

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
        turnOrder[currentIndex].SetTurn(false);
        currentIndex = (currentIndex + 1) % turnOrder.Count;
        AssignTurn();
    }

    void AssignTurn()
    {
        turnOrder[currentIndex].SetTurn(true);
    }

    #endregion

    #region RPC ve Kart Gönderme

    [ServerRpc(RequireOwnership = false)]
    public void SubmitDeckServerRpc(FixedString128Bytes[] cardIdArray, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;
        List<string> cardIds = cardIdArray.Select(id => id.ToString()).ToList();

        playerSubmittedCardIds[senderId] = cardIds;
        Debug.Log("📨 Oyuncu senderId, cardIds.Count kart gönderdi.");
        Debug.Log("🎮 Toplam gönderilen deste sayısı: {playerSubmittedCardIds.Count}");

        if (playerSubmittedCardIds.Count >= 2)
        {
            Debug.Log("🚀 Her iki oyuncudan deste geldi, karakterler spawn ediliyor...");
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
    public void SendAttackServerRpc()
    {
        Debug.Log("Saldırı yapıldı.");
        EndTurn();
    }

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

        Vector3 spawnPos = senderClientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId
            ? playerSpawnPoint.position
            : enemySpawnPoint.position;

        GameObject obj = Instantiate(characterPrefab, spawnPos, Quaternion.identity);
        var ch = obj.GetComponent<Character>();
        ch.Setup(card);

        obj.GetComponent<NetworkObject>()?.SpawnWithOwnership(senderClientId);
        allCharacters.Add(ch);
    }

    #endregion

    #region Victory & Rewards

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

        if (isLocalPlayerWinner)
        {
            Debug.Log("Kazandın! Ödül ve XP verildi.");
        }
        else
        {
            Debug.Log("Kaybettin. Kartlara az da olsa XP verildi.");
        }
    }

    #endregion

    #region UI Güncelleme

    public void ShowEnemyDeck(List<CardData> enemyCards)
    {
        foreach (Transform child in enemyCardParent)
            Destroy(child.gameObject);

        foreach (CardData card in enemyCards)
        {
            GameObject slot = Instantiate(cardSlotPrefab, enemyCardParent); slot.transform.localScale = Vector3.one;

            CardUI ui = slot.GetComponent<CardUI>();
            if (ui != null)
                ui.SetCardData(card);
        }
    }

    #endregion
}