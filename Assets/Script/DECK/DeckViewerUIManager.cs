using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DeckViewerUIManager : MonoBehaviour
{
    public GameObject deckPanelPrefab; // Her bir deste için gösterilecek panel
    public Transform decksParent;      // Panellerin parent objesi (scroll alaný olabilir)
    public GameObject cardUIPrefab;    // Her kart için UI (CardUI prefabý olabilir)
    public DeckManagerObject deckManager;

    public void ShowAllDecks()
    {
        ClearDeckViews();

        List<List<CardData>> allDecks = new()
        {
            deckManager.deck1,
            deckManager.deck2,
            deckManager.deck3,
            deckManager.deck4,
            deckManager.deck5
        };

        for (int i = 0; i < allDecks.Count; i++)
        {
            GameObject deckPanel = Instantiate(deckPanelPrefab, decksParent);
            deckPanel.name = $"Deck_{i + 1}";
            deckPanel.transform.Find("DeckTitle").GetComponent<TMP_Text>().text = $"Deste {i + 1}";

            Transform content = deckPanel.transform.Find("Content");

            foreach (var card in allDecks[i])
            {
                GameObject cardUI = Instantiate(cardUIPrefab, content);
                cardUI.GetComponent<CardUI>().SetCardData(card);
            }
        }
    }

    void ClearDeckViews()
    {
        foreach (Transform child in decksParent)
            Destroy(child.gameObject);
    }
}
