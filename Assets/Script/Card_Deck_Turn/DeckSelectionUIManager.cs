using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckSelectionUIManager : MonoBehaviour
{
    public RectTransform contentParent; // ScrollView -> Content
    public GameObject cardPanelTemplate; // Sahnedeki pasif panel
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
        Debug.Log("Deck seçenekleri gösteriliyor.");

        if (deckManager == null || deckManager.fullDeck.Count == 0)
        {
            Debug.LogWarning("❌ DeckManager yok veya kart eklenmemiş.");
            return;
        }

        foreach (Transform child in contentParent)
            Destroy(child.gameObject); // Önce temizlik

        foreach (CardData card in deckManager.fullDeck)
        {
            GameObject panel = Instantiate(cardPanelTemplate, contentParent);
            panel.SetActive(true);

            // Atamaları yap
            panel.transform.Find("Image").GetComponent<Image>().sprite = card.characterSprite;
            panel.transform.Find("nameText").GetComponent<TextMeshProUGUI>().text = card.cardName;
            panel.transform.Find("levelText").GetComponent<TextMeshProUGUI>().text = "Seviye: " + card.level;
            panel.transform.Find("xpText").GetComponent<TextMeshProUGUI>().text = "XP: " + card.xp + "/100";
        }
    }
}
