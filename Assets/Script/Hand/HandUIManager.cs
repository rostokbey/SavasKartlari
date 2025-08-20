using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUIManager : MonoBehaviour
{
    public Transform handContent;      // HandScroll/Viewport/Content
    public Button deckBackButton;      // DeckBackButton (üstüne kart arkalýðý görseli)
    public Button endTurnButton;       // EndTurnButton
    public GameObject cardUIPrefab;    // CardUI prefab (BattleManager.Init ile de veriyoruz)
    public int handSize = 5;

    private readonly List<CardData> drawPile = new();
    private readonly List<CardData> hand = new();

    public void Init(List<CardData> playerDeck, GameObject cardUIPrefabRef)
    {
        cardUIPrefab = cardUIPrefabRef;

        drawPile.Clear();
        hand.Clear();
        drawPile.AddRange(playerDeck);
        Shuffle(drawPile);
        DrawTo(handSize);
        //RenderHand();

        deckBackButton.onClick.RemoveAllListeners();
        deckBackButton.onClick.AddListener(() =>
        {
            Draw(1);
            RenderHand();
        });

        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(() => BattleManager.Instance.EndTurn());
    }

    void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    void DrawTo(int target) { while (hand.Count < target && drawPile.Count > 0) Draw(1); }

    void Draw(int count)
    {
        for (int i = 0; i < count && drawPile.Count > 0; i++)
        {
            var top = drawPile[0];
            drawPile.RemoveAt(0);
            hand.Add(top);
        }
    }

    void RenderHand()
    {
        foreach (Transform c in handContent) Destroy(c.gameObject);
        foreach (var card in hand)
        {
            var go = Object.Instantiate(cardUIPrefab, handContent);
            var ui = go.GetComponent<CardUI>();
            ui.SetCardData(card, false);      // elde açýk
            ui.SetInteractable(true);
            ui.onClick = () => PlayCard(card);
        }
    }

    void PlayCard(CardData card)
    {
        hand.Remove(card);
        RenderHand();
        BattleManager.Instance.PlayCardServerRpc(card.id); // sahaya 3D prefab zaten burada spawn ediliyor
    }
}
