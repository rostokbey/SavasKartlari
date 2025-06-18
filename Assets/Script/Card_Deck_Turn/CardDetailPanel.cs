
using UnityEngine;
using TMPro;

public class CardDetailPanel : MonoBehaviour
{
    public static CardDetailPanel Instance; // 🔧 Singleton tanımı

    [Header("UI Referansları")]
    public TMP_Text nameText;
    public TMP_Text classText;
    public TMP_Text raceText;
    public TMP_Text comboText;
    public TMP_Text storyText;
    public TMP_Text dexText;

    public GameObject panel; // Detay panelini gizleyip göstermek için

    private void Awake()
    {
        Instance = this; // Singleton ayarı
    }

    public void ShowDetails(CardData data)
    {
        if (nameText != null) nameText.text = data.cardName;
        if (classText != null) classText.text = "Class: " + data.cardClass;
        if (raceText != null) raceText.text = "Irk: " + data.race;
        if (comboText != null) comboText.text = "Kombinasyon: " + data.combo;
        if (storyText != null) storyText.text = "Hikaye: " + data.story;
        if (dexText != null) dexText.text = "DEX: " + data.dex;

        if (panel != null) panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }
}
