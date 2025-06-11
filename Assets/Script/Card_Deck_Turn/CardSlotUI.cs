using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CardSlotUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public Image characterImage; // ✅ görsel alanı
    public Button button;

    public Action OnCardClicked;

    public void SetCardInfo(CardData card)
    {
        if (nameText == null || levelText == null || xpText == null || characterImage == null || button == null)
        {
            Debug.LogError("❌ CardSlotUI: UI referanslarından biri atanmadı!");
            return;

        }
        if (card.characterSprite == null)
            Debug.LogWarning("❌ Görsel atanamadı: " + card.cardName);
        else
            Debug.Log("🖼️ Sprite atanıyor: " + card.characterSprite.name);

        nameText.text = card.cardName;
        levelText.text = "Seviye: " + card.level;
        xpText.text = "XP: " + card.xp + "/100";

        characterImage.sprite = card.characterSprite; // ✅ görsel set

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnCardClicked?.Invoke());
    }
}
