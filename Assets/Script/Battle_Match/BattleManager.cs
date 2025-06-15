using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : NetworkBehaviour
{
    public static BattleManager Instance;

    public GameObject characterPrefab;
    public Transform[] playerGridPositions;
    public Transform[] enemyGridPositions;
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    public Button attackButton;
    public Button skillButton;

    public Transform enemyCardParent;
    public GameObject cardSlotPrefab;

    public ulong currentTurnClientId;

    private List<Character> allCharacters = new();
    private Dictionary<ulong, List<string>> playerSubmittedCardIds = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        if (IsServer && NetworkManager.Singleton.ConnectedClientsList.Count > 0)
        {
            currentTurnClientId = NetworkManager.Singleton.ConnectedClientsList[0].ClientId;
        }
    }

    void Update()
    {
        bool isTurn = NetworkManager.Singleton.LocalClientId == currentTurnClientId;
        attackButton.interactable = isTurn;
        skillButton.interactable = isTurn;
    }

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

    public List<Character> turnOrder = new();
    private int currentIndex = 0;

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

    public void ShowEnemyDeck(List<CardData> enemyCards)
    {
        foreach (Transform child in enemyCardParent)
            Destroy(child.gameObject);

        foreach (CardData card in enemyCards)
        {
            GameObject slot = Instantiate(cardSlotPrefab, enemyCardParent);
            slot.transform.localScale = Vector3.one;
        }
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


    [ServerRpc(RequireOwnership = false)]
    public void SubmitDeckServerRpc(FixedString128Bytes[] cardIdArray, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;
        List<string> cardIds = cardIdArray.Select(id => id.ToString()).ToList();

        playerSubmittedCardIds[senderId] = cardIds;
        Debug.Log($"📨 Oyuncu {senderId}, {cardIds.Count} kart gönderdi.");
        Debug.Log($"🎮 Toplam gönderilen deste sayısı: {playerSubmittedCardIds.Count}");

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
}
