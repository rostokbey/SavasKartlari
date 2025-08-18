using System.Xml.Linq;
using TMPro;
using UnityEngine;
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
    public Button selectButton;

    [Header("Battle Durumu")]
    public bool isInBattle = false;

    private CardData cardData;

    void Start()
    {
        // Detay butonu ayarı
        if (detailButton != null)
        {
            detailButton.onClick.RemoveAllListeners();
            detailButton.onClick.AddListener(OnCardClicked);
            detailButton.gameObject.SetActive(!isInBattle);
        }

        // Seç butonu ayarı
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectClicked);
            selectButton.gameObject.SetActive(!isInBattle); // Savaşta görünmesin
        }
    }

    public void SetCardData(CardData data, bool showButtons = true)
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

        // Butonları sadece showButtons == true ise göster
        if (detailButton != null)
            detailButton.gameObject.SetActive(showButtons);
        if (selectButton != null)
            selectButton.gameObject.SetActive(showButtons);
    }

    public void SetBattleMode(bool on)
    {
        nameText.gameObject.SetActive(!on);
        hpText.gameObject.SetActive(!on);
        dexText.gameObject.SetActive(!on);
       levelText.gameObject.SetActive(!on);
        xpText.gameObject.SetActive(!on);
        detailButton.gameObject.SetActive(!on);
        // SelectButton açık kalsın (kartı oynamak için) ya da onu da kapatabilirsin
    }


    public System.Action onClick;

    public void SetInteractable(bool b)
    {
        var btn = GetComponent<UnityEngine.UI.Button>();
        if (btn != null) btn.interactable = b;
    }

    // Button OnClick'ten bağlayacağız
    public void OnButtonClick()
    {
        onClick?.Invoke();
    }



    public void OnCardClicked()
    {
        if (!isInBattle)
        {
            CardDetailPanel.Instance?.ShowCardDetails(cardData);
        }
    }

    public void OnSelectClicked()
    {
        if (!isInBattle)
        {
            DeckSelectPopup.Instance?.ShowDeckChoice(this.cardData);
        }
    }
}
