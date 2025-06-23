using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelectPopup : MonoBehaviour
{
    public static DeckSelectPopup Instance;

    public GameObject popupPanel;
    public Button[] deckButtons; // 5 buton olacak

    private CardData pendingCard;

    void Awake()
    {
        Instance = this;

        // Her butona dinleyici ekle
        for (int i = 0; i < deckButtons.Length; i++)
        {
            int deckIndex = i;
            deckButtons[i].onClick.AddListener(() => AddCardToDeck(deckIndex));
        }
    }

    public void ShowDeckChoice(CardData card)
    {
        pendingCard = card;
        popupPanel.SetActive(true);
    }

    void AddCardToDeck(int deckIndex)
    {
        DeckManagerObject deckManager = FindObjectOfType<DeckManagerObject>();

        if (deckManager == null)
        {
            Debug.LogError("DeckManagerObject bulunamadı!");
            popupPanel.SetActive(false);
            return;
        }

        List<CardData> selectedDeck = deckIndex switch
        {
            0 => deckManager.deck1,
            1 => deckManager.deck2,
            2 => deckManager.deck3,
            3 => deckManager.deck4,
            4 => deckManager.deck5,
            _ => null
        };

        if (selectedDeck == null)
        {
            Debug.LogWarning("Geçersiz deste indexi.");
            popupPanel.SetActive(false);
            return;
        }

        if (selectedDeck.Count >= 25)
        {
            Debug.LogWarning("Bu desteye zaten 25 kart eklenmiş.");
            popupPanel.SetActive(false);
            return;
        }

        selectedDeck.Add(pendingCard);
        Debug.Log($"✅ {pendingCard.cardName} kartı {deckIndex + 1}. desteye eklendi.");
        popupPanel.SetActive(false);
    }
}
