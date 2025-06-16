using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DeckSelectionUIManager : MonoBehaviour
{
    public Transform contentParent; // ScrollView Content objesi
    public List<Transform> cardUIElements; // Sahne üzerindeki her kart UI nesnesi

    private DeckManagerObject deckManager;

    void Start()
    {
        deckManager = FindObjectOfType<DeckManagerObject>();
        DisplayDeckOptions();
    }

    public void DisplayDeckOptions()
    {
        Debug.Log("📋 Deck seçenekleri gösteriliyor.");
        if (deckManager == null || deckManager.fullDeck.Count == 0)
        {
            Debug.LogWarning("⚠️ Kart yok veya DeckManager eksik.");
            return;
        }

        for (int i = 0; i < deckManager.fullDeck.Count && i < cardUIElements.Count; i++)
        {
            var ui = cardUIElements[i];
            var data = deckManager.fullDeck[i];

            ui.Find("NameText").GetComponent<TextMeshProUGUI>().text = data.cardName;
            ui.Find("LevelText").GetComponent<TextMeshProUGUI>().text = "Seviye: " + data.level;
            ui.Find("XpText").GetComponent<TextMeshProUGUI>().text = "XP: " + data.xp;
            ui.Find("Image").GetComponent<Image>().sprite = data.characterSprite;

            ui.gameObject.SetActive(true);
        }

        // Kullanılmayan UI slotlarını kapat
        for (int i = deckManager.fullDeck.Count; i < cardUIElements.Count; i++)
        {
            cardUIElements[i].gameObject.SetActive(false);
        }
    }
}
