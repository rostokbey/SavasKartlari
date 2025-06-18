using UnityEngine;
using TMPro;

public class CardDetailPanel : MonoBehaviour
{
    public static CardDetailPanel Instance;

    [Header("UI Referansları")]
    public GameObject panel;

    public GameObject inventoryPanel; // 💡 Buraya InventoryUI panelini atayacağız (DeckSelectPanel)

    public TMP_Text nameText;
    public TMP_Text dexText;
    public TMP_Text classText;
    public TMP_Text raceText;
    public TMP_Text comboText;
    public TMP_Text storyText;
    public TMP_Text rarityText;
    public TMP_Text passiveText;
    public TMP_Text abilityText;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowCardDetails(CardData card)
    {
        nameText.text = card.cardName.Replace("_", " ");
        dexText.text = "DEX: " + card.dex;
        classText.text = "Class: " + card.className;
        raceText.text = "Irk: " + card.race;
        comboText.text = "Kombinasyon: " + card.combo;
        storyText.text = "Hikaye: " + card.story;
        rarityText.text = "Nadirlik: " + card.rarity;
        passiveText.text = "Pasif: " + card.passive;
        abilityText.text = "Aktif: " + card.ability;

        panel.SetActive(true);
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false); // Envanteri gizle
    }

    public void ClosePanel()
    {
        panel.SetActive(false);

        // Envanteri geri getir
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);
    }
}
