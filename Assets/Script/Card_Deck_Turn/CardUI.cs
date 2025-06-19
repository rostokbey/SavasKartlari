using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public Image characterImage;
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text strText;
    public TMP_Text dexText;
    public TMP_Text levelText;
    public TMP_Text xpText;


    private CardData cardData;

    void Awake()
    {
        // Eğer prefabta buton varsa, bu scripti ona bağla
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnCardClicked);
        }
        else
        {
            // Eğer bu objede değilse, çocuklarında ara
            btn = GetComponentInChildren<Button>();
            if (btn != null)
                btn.onClick.AddListener(OnCardClicked);
        }
    }

    public void SetCardData(CardData data)
    {
        cardData = data;

        if (characterImage != null)
            characterImage.sprite = data.characterSprite;

        if (nameText != null)
            nameText.text = data.cardName.Replace("_", " ");

        if (hpText != null)
            hpText.text = "HP: " + data.baseHP;

        if (strText != null)
            strText.text = "STR: " + data.baseDamage;

        if (dexText != null)
            dexText.text = "DEX: " + data.dex;

        if (dexText != null)
            dexText.text = "LEVEL: " + data.level;

        if (dexText != null)
            dexText.text = "XP: " + data.xp;
    }


    public void OnCardClicked()
    {
        if (CardDetailPanel.Instance != null)
        {
            CardDetailPanel.Instance.ShowCardDetails(cardData);

            // Kart listesi panelini kapat (Inventory panelinin GameObject'ini kapat)
            if (CardDetailPanel.Instance.panel != null)
            {
                // varsayalım ki Inventory paneli CardDetailPanel'den bir üst GameObject'te duruyor
                GameObject inventoryPanel = transform.parent.parent.gameObject; // veya direkt referans da verebilirsin
                inventoryPanel.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("CardDetailPanel.Instance bulunamadı.");
        }
    }

}
