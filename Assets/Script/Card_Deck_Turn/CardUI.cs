using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("UI Referansları")]
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text damageText;
    public TMP_Text levelText;
    public TMP_Text xpText;
    public TMP_Text dexText;
    public Image characterImage;
    public Button detailButton;

    [Header("Battle Durumu")]
    public bool isInBattle = false;

    private CardData cardData;

    void Start()
    {
        if (detailButton != null)
        {
            detailButton.onClick.RemoveAllListeners();
            detailButton.onClick.AddListener(OnCardClicked);
            detailButton.gameObject.SetActive(!isInBattle);
        }
    }


    public void SetCardData(CardData data)
    {
        cardData = data;

        nameText.text = data.cardName.Replace("_", " ");
        hpText.text = "HP: " + data.baseHP;
        damageText.text = "STR: " + data.baseDamage;
        levelText.text = "Lv: " + data.level;
        xpText.text = "XP: " + data.xp + "/100";
        dexText.text = "DEX: " + data.dex;

        if (characterImage != null)
            characterImage.sprite = data.characterSprite;

        // Detay butonu durumu tekrar kontrol
        if (detailButton != null)
            detailButton.gameObject.SetActive(!isInBattle);
    }

    public void OnCardClicked()
    {
        if (!isInBattle)
        {
            CardDetailPanel.Instance?.ShowCardDetails(cardData);
        }
    }
}
