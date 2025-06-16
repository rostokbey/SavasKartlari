using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode;

public class TurnDrawManager : MonoBehaviour
{
    [Header("Sahnedeki kart panel template")]
    public GameObject cardPanelTemplate; // SetActive(false) yapýlmýþ panel (sahne içinde)

    [Header("Kartlarýn gösterileceði el alaný")]
    public Transform handParent; // El kartlarýnýn gösterileceði layout

    private List<CardData> handCards = new();
    private Queue<CardData> drawQueue = new();
    private int cardsPlayed = 0;
    private const int MaxHandSize = 5;
    private const int MaxCardsPerMatch = 12;

    private ulong localPlayerId => NetworkManager.Singleton.LocalClientId;

    void Start()
    {
        var deck = FindObjectOfType<DeckManagerObject>();
        if (deck != null && deck.currentMatchDeck.Count > 0)
        {
            drawQueue = new Queue<CardData>(Shuffle(deck.currentMatchDeck));
            DrawCard();
        }
    }

    public void OnTurnStart()
    {
        DrawCard();
    }

    public void DrawCard()
    {
        if (drawQueue.Count == 0 || handCards.Count >= MaxHandSize)
            return;

        CardData card = drawQueue.Dequeue();
        handCards.Add(card);

        GameObject panel = Instantiate(cardPanelTemplate, handParent);
        panel.SetActive(true);

        panel.transform.Find("Image").GetComponent<Image>().sprite = card.characterSprite;
        panel.transform.Find("nameText").GetComponent<TextMeshProUGUI>().text = card.cardName;
        panel.transform.Find("levelText").GetComponent<TextMeshProUGUI>().text = "Seviye: " + card.level;
        panel.transform.Find("xpText").GetComponent<TextMeshProUGUI>().text = "XP: " + card.xp + "/100";

        Button btn = panel.GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(() => PlayCard(card, panel));
    }

    void PlayCard(CardData card, GameObject panelObj)
    {
        if (!TurnManager.Instance.IsMyTurn(localPlayerId))
        {
            Debug.Log("Sýra sende deðil");
            return;
        }

        if (cardsPlayed >= MaxCardsPerMatch)
        {
            Debug.Log("12 kart sýnýrý aþýldý");
            return;
        }

        FindObjectOfType<BattleManager>()?.PlayCardServerRpc(card.id);
        handCards.Remove(card);
        Destroy(panelObj);
        cardsPlayed++;
    }

    List<CardData> Shuffle(List<CardData> input)
    {
        List<CardData> list = new(input);
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
        return list;
    }
}
