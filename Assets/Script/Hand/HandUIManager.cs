using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUIManager : MonoBehaviour
{
    [Header("UI Alanlarý")]
    public Transform handArea;      // Kartlarýn görüneceði panel
    public Button attackButton;     // Saldýr butonu

    [Header("Prefab")]
    public GameObject cardPrefab;   // CardUI_Battle prefabý

    private List<CardUI> handCards = new List<CardUI>();
    private CardData selectedCard;  // Oyuncunun seçtiði kart

    // Baþlatma -> Desteden 5 kart çek
    public void Init(List<CardData> deck)
    {
        handCards.Clear();

        for (int i = 0; i < 5 && i < deck.Count; i++)
        {
            var card = deck[i];

            // Kart UI oluþtur
            var uiObj = Instantiate(cardPrefab, handArea);
            var ui = uiObj.GetComponent<CardUI>();
            ui.SetCardData(card, true);

            // Seçilirse sahneye spawn et
            ui.onSelect = (c) =>
            {
                OnCardSelected(c);
            };

            handCards.Add(ui);
        }

        // Saldýr butonu baþlangýçta kapalý
        if (attackButton != null)
            attackButton.gameObject.SetActive(false);
    }

    // Kart seçildiðinde
    private void OnCardSelected(CardData card)
    {
        selectedCard = card;
        Debug.Log("[HandUI] Kart seçildi: " + card.cardName);

        // 3D prefab sahneye spawn edilir
        var bm = FindObjectOfType<BattleManager>();
        bm.SpawnCharacter(card, true);

        // Saldýr butonu aktif edilir
        if (attackButton != null)
            attackButton.gameObject.SetActive(true);
    }

    // Saldýr butonuna basýldýðýnda
    public void OnAttackPressed()
    {
        if (selectedCard == null) return;

        Debug.Log("[HandUI] Saldýr butonu basýldý: " + selectedCard.cardName);

        var bm = FindObjectOfType<BattleManager>();
        bm.Attack(selectedCard);

        // Ýstersen butonu tekrar kapatabilirsin (tur sistemi için)
        attackButton.gameObject.SetActive(false);
    }
}
