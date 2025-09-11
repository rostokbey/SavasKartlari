using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HandUIManager : MonoBehaviour
{
    [Header("UI Alanları")]
    public Transform handArea;      // Kartların görüneceği panel (Horizontal Layout Group olmalı)
    public Button attackButton;     // Saldır butonu

    [Header("Prefab")]
    public GameObject cardPrefab;   // CardUI_Battle prefabı (üzerinde CardUI.cs)

    [Header("Arka Plan (opsiyonel)")]
    public GameObject backgroundCard; // Sadece bir tane arka kart resmi, opsiyonel

    private List<CardUI> handCards = new List<CardUI>();
    private CardData currentSelectedCard; // o tur seçilen kart

    // Başlatma -> Desteden 5 kart çek
    public void Init(List<CardData> deck)
    {
        // Null kontrolleri
        if (handArea == null || cardPrefab == null || deck == null)
        {
            Debug.LogError("[HandUI] Gerekli referanslar atanmamış! handArea / cardPrefab / deck kontrol et.");
            return;
        }

        // önce eldeki kartları temizle
        foreach (Transform child in handArea)
            Destroy(child.gameObject);
        handCards.Clear();
        currentSelectedCard = null;

        // Arka plan kartını oluştur (sadece bir kez, opsiyonel)
        if (backgroundCard != null)
        {
            var bgCard = Instantiate(backgroundCard, handArea);
            bgCard.transform.SetAsFirstSibling();
        }

        // sadece 5 kart çekelim
        foreach (var card in deck.Take(5))
        {
            var go = Instantiate(cardPrefab, handArea);
            var ui = go.GetComponent<CardUI>();

            if (ui == null)
            {
                Debug.LogWarning("[HandUI] CardUI component bulunamadı on prefab!");
                continue;
            }

            // savaş modu
            ui.isInBattle = true;
            ui.SetCardData(card, true);

            // seçilince HandUIManager'a haber ver
            ui.onSelect = (selectedCard) =>
            {
                currentSelectedCard = selectedCard;
                Debug.Log($"[HandUI] Kart seçildi: {selectedCard.cardName}");
            };

            handCards.Add(ui);
        }

        // Attack butonuna bağla (tek seferlik)
        if (attackButton != null)
        {
            attackButton.onClick.RemoveAllListeners();
            attackButton.onClick.AddListener(OnAttackPressed);
        }
        else
        {
            Debug.LogWarning("[HandUI] attackButton atanmamış!");
        }
    }

    private void OnAttackPressed()
    {
        if (currentSelectedCard == null)
        {
            Debug.LogWarning("[HandUI] Hiç kart seçilmedi.");
            return;
        }

        Debug.Log($"[HandUI] Attack basıldı, kart oynanıyor: {currentSelectedCard.cardName}");

        if (BattleManager.Instance != null)
        {
            // isPlayerTurn true çünkü oyuncu Attack tuşuna bastı
            BattleManager.Instance.Attack(currentSelectedCard, true);
        }
        else
        {
            Debug.LogError("[HandUI] BattleManager.Instance bulunamadı!");
        }

        // seçim sıfırlanır
        currentSelectedCard = null;
    }
}