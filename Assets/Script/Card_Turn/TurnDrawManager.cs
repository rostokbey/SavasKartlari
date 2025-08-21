using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode;

public class TurnDrawManager : MonoBehaviour
{
    public GameObject cardUIPrefab; // CardUI prefab
    public Transform handParent;   // El alaný

    private Queue<CardData> drawQueue = new();
    private const int MaxHandSize = 5;

    private void Start()
    {
        DeckManagerObject deck = FindObjectOfType<DeckManagerObject>();

        if (deck != null && deck.currentMatchDeck.Count > 0)
        {
            drawQueue = new Queue<CardData>(Shuffle(deck.currentMatchDeck));
            DrawCard();
        }
    }

    private List<CardData> Shuffle(List<CardData> list)
    {
        System.Random rng = new();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardData temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
        return list;
    }

    public void OnTurnStart()
    {
        DrawCard();
    }

    public void DrawCard()
    {
        if (handParent.childCount >= MaxHandSize || drawQueue.Count == 0)
            return;

        CardData nextCard = drawQueue.Dequeue();
        GameObject cardGO = Instantiate(cardUIPrefab, handParent);
        CardUI cardUI = cardGO.GetComponent<CardUI>();
        if (cardUI != null)
        {
            cardUI.SetCardData(nextCard, true);
        }
    }
}
