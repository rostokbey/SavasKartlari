using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchReadyButton : MonoBehaviour
{
    public Button readyButton;

    void Start()
    {
        readyButton.onClick.AddListener(OnReadyClicked);
    }

    void OnReadyClicked()
    {
        var deckManager = FindObjectOfType<DeckManagerObject>();
        if (deckManager == null)
        {
            Debug.LogError("❌ DeckManagerObject bulunamadı!");
            return;
        }

        // Örnek amaçlı: tüm fullDeck'teki kartları seçili sayıyoruz
        List<string> selectedIds = new List<string>();
        foreach (var card in deckManager.fullDeck)
        {
            selectedIds.Add(card.id);
        }

        deckManager.PrepareMatchDeck(selectedIds);

        Debug.Log("✅ MatchReadyButton -> PrepareMatchDeck çağrıldı.");
    }
}
