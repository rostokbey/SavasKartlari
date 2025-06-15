// TurnDrawManager.cs – Her tur kart çekme, elde tutma ve sahaya koyma sistemi

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Unity.Netcode;

public class TurnDrawManager : MonoBehaviour
{
    public Transform handParent;               // Elde gösterilecek UI alaný
    public GameObject cardSlotPrefab;          // Her kartý temsil eden prefab

    private List<CardData> handCards = new();  // Elde tutulan kartlar
    private List<CardData> drawPile = new();   // Çekilecek kalan kartlar

    private const int maxHandSize = 5;         // Elde max 5 kart olabilir
    private const int maxTotalPlays = 12;      // Maç boyunca en fazla 12 kart oynanabilir
    private int totalPlayedCards = 0;

    private ulong localPlayerId => NetworkManager.Singleton.LocalClientId;

    void Start()
    {
        // Match destesiyle baþlat
        var deck = FindObjectOfType<DeckManagerObject>();
        drawPile = new List<CardData>(deck.currentMatchDeck);

        Shuffle(drawPile);
        DrawCard(); // ilk kart tur baþlarken
    }

    public void OnTurnStart()
    {
        DrawCard();
    }

    public void DrawCard()
    {
        if (handCards.Count >= maxHandSize || drawPile.Count == 0)
        {
            Debug.Log("Elde yer yok veya kart kalmadý");
            return;
        }

        CardData card = drawPile[0];
        drawPile.RemoveAt(0);
        handCards.Add(card);

        CreateCardUI(card);
    }

    void CreateCardUI(CardData card)
    {
        GameObject cardUI = Instantiate(cardSlotPrefab, handParent);
        var slot = cardUI.GetComponent<CardSlotUI>();
        slot?.SetCardInfo(card);
    }

    void TryPlayCard(CardData card, GameObject slotUI)
    {
        if (!TurnManager.Instance.IsMyTurn(localPlayerId))
        {
            Debug.Log("Sýra sende deðil.");
            return;
        }

        if (totalPlayedCards >= maxTotalPlays)
        {
            Debug.Log("12 kart zaten oynandý");
            return;
        }

        FindObjectOfType<BattleManager>().PlayCardServerRpc(card.id);

        handCards.Remove(card);
        Destroy(slotUI);

        totalPlayedCards++;
    }

    void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(0, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
