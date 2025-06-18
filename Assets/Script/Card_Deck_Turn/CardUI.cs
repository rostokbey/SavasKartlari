
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public Image characterImage;
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text strText;
    public TMP_Text dexText; // 🛡️ Yeni: Savunma (DEX)

    private CardData cardData;

    public void SetCardData(CardData data)
    {
        cardData = data;

        if (characterImage != null)
            characterImage.sprite = data.characterSprite;

        if (nameText != null)
            nameText.text = data.cardName;

        if (hpText != null)
            hpText.text = "HP: " + data.baseHP;

        if (strText != null)
            strText.text = "STR: " + data.baseDamage;

        if (dexText != null)
            dexText.text = "DEX: " + data.dex; // 🛡️ Savunma bilgisi
    }

    public void OnClick()
    {
        CardDetailPanel.Instance.ShowDetails(cardData);
    }
}
