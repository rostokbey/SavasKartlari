using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUIManager : MonoBehaviour
{
    [Header("UI Alanlar�")]
    public Transform handArea;      // Kartlar�n g�r�nece�i panel
    public Button attackButton;     // Sald�r butonu

    [Header("Prefab")]
    public GameObject cardPrefab;   // CardUI_Battle prefab�

    private List<CardUI> handCards = new List<CardUI>();
    private CardData selectedCard;  // Oyuncunun se�ti�i kart

    // Ba�latma -> Desteden 5 kart �ek
    public void Init(List<CardData> deck)
    {
        handCards.Clear();

        for (int i = 0; i < 5 && i < deck.Count; i++)
        {
            var card = deck[i];

            // Kart UI olu�tur
            var uiObj = Instantiate(cardPrefab, handArea);
            var ui = uiObj.GetComponent<CardUI>();
            ui.SetCardData(card, true);

            // Se�ilirse sahneye spawn et
            ui.onSelect = (c) =>
            {
                OnCardSelected(c);
            };

            handCards.Add(ui);
        }

        // Sald�r butonu ba�lang��ta kapal�
        if (attackButton != null)
            attackButton.gameObject.SetActive(false);
    }

    // Kart se�ildi�inde
    private void OnCardSelected(CardData card)
    {
        selectedCard = card;
        Debug.Log("[HandUI] Kart se�ildi: " + card.cardName);

        // 3D prefab sahneye spawn edilir
        var bm = FindObjectOfType<BattleManager>();
        bm.SpawnCharacter(card, true);

        // Sald�r butonu aktif edilir
        if (attackButton != null)
            attackButton.gameObject.SetActive(true);
    }

    // Sald�r butonuna bas�ld���nda
    public void OnAttackPressed()
    {
        if (selectedCard == null) return;

        Debug.Log("[HandUI] Sald�r butonu bas�ld�: " + selectedCard.cardName);

        var bm = FindObjectOfType<BattleManager>();
        bm.Attack(selectedCard);

        // �stersen butonu tekrar kapatabilirsin (tur sistemi i�in)
        attackButton.gameObject.SetActive(false);
    }
}
