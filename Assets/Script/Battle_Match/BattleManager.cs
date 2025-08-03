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
    public GameObject characterPrefab; // Varsayılan prefab
    public GameObject cardPrefab; // CardUI prefabı
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

        var playerCards = StartBattleManager.Instance?.selectedMatchCards;
        var enemyCards = StartBattleManager.Instance?.enemyMatchCards;

        if (playerCards == null || enemyCards == null)
        {
            Debug.LogError("❌ BattleManager: Kartlar alınamadı.");
            return;
        }

        Debug.Log($"🟩 Oyuncu kartları sayısı: {playerCards.Count}");
        Debug.Log($"🟥 Düşman kartları sayısı: {enemyCards.Count}");

        SpawnPlayerCards(playerCards);
        ShowEnemyDeck(enemyCards);

        SpawnCharacters(playerCards, enemyCards);

        if (NetworkManager.Singleton.ConnectedClientsList.Count > 0)
            currentTurnClientId = NetworkManager.Singleton.ConnectedClientsList[0].ClientId;
    }

    #region Oyuncu Kart UI

    public void SpawnPlayerCards(List<CardData> selectedCards)
    {
        for (int i = 0; i < selectedCards.Count && i < playerGridPositions.Length; i++)
        {
            var cardData = selectedCards[i];
            GameObject go = Instantiate(cardPrefab, playerGridPositions[i].position, Quaternion.identity);
            go.transform.SetParent(playerGridPositions[i]);
            CardUI ui = go.GetComponent<CardUI>();
            if (ui != null)
                ui.SetCardData(cardData, false);
        }
    }

    #endregion

    #region Karakter Spawn

    public void SpawnCharacters(List<CardData> playerCards, List<CardData> enemyCards)
    {
        if (!IsServer)
        {
            Debug.LogWarning("SpawnCharacters sadece sunucu tarafından çağrılabilir.");
            return;
        }

        for (int i = 0; i < playerCards.Count && i < playerGridPositions.Length; i++)
        {
            var prefab = playerCards[i].characterPrefab3D != null ? playerCards[i].characterPrefab3D : characterPrefab;
            GameObject obj = Instantiate(prefab, playerGridPositions[i].position, Quaternion.identity);
            obj.transform.localScale = Vector3.one;

            Character ch = obj.GetComponent<Character>();
            ch.Setup(playerCards[i]);
            allCharacters.Add(ch);
            obj.GetComponent<NetworkObject>()?.SpawnWithOwnership(NetworkManager.Singleton.ConnectedClientsList[0].ClientId);
        }

        for (int i = 0; i < enemyCards.Count && i < enemyGridPositions.Length; i++)
        {
            var prefab = enemyCards[i].characterPrefab3D != null ? enemyCards[i].characterPrefab3D : characterPrefab;
            GameObject obj = Instantiate(prefab, enemyGridPositions[i].position, Quaternion.identity);
            obj.transform.localScale = Vector3.one;

            Character ch = obj.GetComponent<Character>();
            ch.Setup(enemyCards[i]);
            allCharacters.Add(ch);
            obj.GetComponent<NetworkObject>()?.SpawnWithOwnership(NetworkManager.Singleton.ConnectedClientsList[1].ClientId);
        }

        AssignTurnToClient(currentTurnClientId);
    }

    #endregion

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

        var prefab = card.characterPrefab3D != null ? card.characterPrefab3D : characterPrefab;

        Vector3 spawnPos = senderClientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId
            ? playerSpawnPoint.position
            : enemySpawnPoint.position;

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
        var ch = obj.GetComponent<Character>();
        ch.Setup(card);

        obj.GetComponent<NetworkObject>()?.SpawnWithOwnership(senderClientId);
        allCharacters.Add(ch);
    }

    #endregion

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

    #region UI

    public void ShowEnemyDeck(List<CardData> enemyCards)
    {
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
