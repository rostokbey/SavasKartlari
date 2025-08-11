using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DeckViewerUIManager : MonoBehaviour
{
    [Header("UI Refs")]
    public GameObject deckViewPanel;     // ❗ DeckViewPanel (kırmızı arka planlı panel)
    public Transform decksParent;        // ❗ ScrollView/Viewport/Content
    public GameObject deckPanelPrefab;   // ❗ İçinde “DeckTitle” (TMP_Text) ve “Content” (Transform) var
    public GameObject cardUIPrefab;      // ❗ CardUI componentli prefab

    [Header("Data")]
    public DeckManagerObject deckManager;

    public void ShowAllDecks()
    {
        // 1) Paneli görünür yap
        if (deckViewPanel != null && !deckViewPanel.activeSelf)
            deckViewPanel.SetActive(true);

        // 2) Güvenlik kontrolleri
        if (deckManager == null)
            deckManager = FindObjectOfType<DeckManagerObject>();

        if (deckManager == null) { Debug.LogError("❌ DeckManagerObject yok."); return; }
        if (decksParent == null) { Debug.LogError("❌ decksParent atanmadı."); return; }
        if (deckPanelPrefab == null) { Debug.LogError("❌ deckPanelPrefab atanmadı."); return; }
        if (cardUIPrefab == null) { Debug.LogError("❌ cardUIPrefab atanmadı."); return; }

        // 3) Temizle
        foreach (Transform child in decksParent) Destroy(child.gameObject);

        // 4) Tüm desteleri sırala
        var allDecks = new List<List<CardData>> {
            deckManager.deck1, deckManager.deck2, deckManager.deck3, deckManager.deck4, deckManager.deck5
        };

        for (int i = 0; i < allDecks.Count; i++)
        {
            var deck = allDecks[i];
            var deckPanel = Instantiate(deckPanelPrefab, decksParent);
            deckPanel.name = $"Deck_{i + 1}";

            // Başlık
            var titleTf = deckPanel.transform.Find("DeckTitle");
            if (titleTf != null)
            {
                var title = titleTf.GetComponent<TMP_Text>();
                if (title != null) title.text = $"Deste {i + 1}  ({deck.Count}/25)";
            }
            else
            {
                Debug.LogWarning($"⚠ DeckPanelPrefab içinde 'DeckTitle' bulunamadı. ({deckPanel.name})");
            }

            // İçerik alanı
            var content = deckPanel.transform.Find("Content");
            if (content == null)
            {
                Debug.LogError($"❌ DeckPanelPrefab içinde 'Content' bulunamadı. ({deckPanel.name})");
                continue;
            }

            // Kartları bas
            foreach (var card in deck)
            {
                var cardUIObj = Instantiate(cardUIPrefab, content);
                var ui = cardUIObj.GetComponent<CardUI>();
                if (ui != null) ui.SetCardData(card, false); // savaş butonları gizli
            }
        }

        // 5) Layout’u yenile (bazı cihazlarda gerekli)
        var parentRect = decksParent as RectTransform;
        if (parentRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
        }

        Debug.Log("📜 Tüm desteler listelendi ve panel aktif.");
    }
}
