using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HandUIManager : MonoBehaviour
{
    [Header("UI Alanları")]
    public Transform handArea;      // Kartların görüneceği panel
    public Button attackButton;     // Saldır butonu

    [Header("Prefab")]
    public GameObject cardPrefab;   // CardUI_Battle prefabı

    private List<CardUI> handCards = new List<CardUI>();
    private CardData selectedCard;  // Oyuncunun seçtiği kart

    private CardData currentSelectedCard; // o tur seçilen kart

    // Başlatma -> Desteden 5 kart çek
    public void Init(List<CardData> deck)
    {
        // Hand area boş mu kontrol et
        if (handArea == null || cardPrefab == null)
        {
            Debug.LogError("[HandUI] Referanslar eksik! handArea veya cardPrefab atanmadı.");
            return;
        }

        // önce eldeki kartları temizle
        foreach (Transform child in handArea)
            Destroy(child.gameObject);

        // sadece 5 kart çekelim
        foreach (var card in deck.Take(5))
        {
            var go = Instantiate(cardPrefab, handArea);
            var ui = go.GetComponent<CardUI>();

            // savaş modu
            ui.isInBattle = true;
            ui.SetCardData(card, true);

            // seçilince HandUIManager’a haber ver
            ui.onSelect = (selectedCard) =>
            {
                currentSelectedCard = selectedCard;
                Debug.Log($"[HandUI] Kart seçildi: {selectedCard.cardName}");
            };
        }

        // Attack butonuna bağla
        if (attackButton != null)
        {
            attackButton.onClick.RemoveAllListeners();
            attackButton.onClick.AddListener(OnAttackPressed);
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

        // BattleManager’a haber ver → sahneye spawn et
        BattleManager.Instance.Attack(currentSelectedCard);

        // seçim sıfırlanır
        currentSelectedCard = null;
    }

}
