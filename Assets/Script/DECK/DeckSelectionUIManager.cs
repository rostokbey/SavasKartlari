using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckSelectionUIManager : MonoBehaviour
{
    public RectTransform contentParent; // ScrollView -> Content
    public GameObject cardUIPrefab; 	// CardUI prefab
    public DeckManagerObject deckManager;

    void Start()
    {
        if (deckManager == null)
        {
            deckManager = FindObjectOfType<DeckManagerObject>();
            if (deckManager == null)
            {
                Debug.LogError("❌ DeckManagerObject bulunamadı!");
                return;
            }
        }

        DisplayDeckOptions();
    }

    public void DisplayDeckOptions()
    {
        if (deckManager == null || deckManager.fullDeck.Count == 0)
        {
            Debug.LogWarning("❌ DeckManager yok veya kart eklenmemiş.");
            return;
        }

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (CardData card in deckManager.fullDeck)
        {
            GameObject cardGO = Instantiate(cardUIPrefab, contentParent);
            CardUI cardUI = cardGO.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.SetCardData(card);
            }
        }
    }
}
