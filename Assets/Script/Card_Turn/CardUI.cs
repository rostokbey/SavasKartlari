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

    [Tooltip("Kartın ön yüzü (karakter görseli)")]
    public Image characterImage;   // front
    [Tooltip("Kartın arka yüzü (ters kapak görseli)")]
    public Image backImage;        // back (opsiyonel)

    [Header("Butonlar (envanterde görünür)")]
    public Button detailButton;
    public Button selectButton;

    [Header("Battle Durumu")]
    public bool isInBattle = false;

    private CardData cardData;

    void Start()
    {
        // Detay butonu
        if (detailButton != null)
        {
            detailButton.onClick.RemoveAllListeners();
            detailButton.onClick.AddListener(OnCardClicked);
            detailButton.gameObject.SetActive(!isInBattle);
        }

        // Seç butonu
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectClicked);
            selectButton.gameObject.SetActive(!isInBattle);
        }
    }

    // ---- Overload 1: Eski çağrıları korumak için (tek parametre) ----
    public void SetCardData(CardData data)
    {
        SetCardData(data, true); // default: butonlar açık
    }

    // ---- Overload 2: Envanter/Savaş genel kullanım ----
    public void SetCardData(CardData data, bool showButtons = true)
    {
        cardData = data;

        if (nameText) nameText.text = data.cardName.Replace("_", " ");
        if (hpText) hpText.text = "HP: " + data.baseHP;
        if (damageText) damageText.text = "STR: " + data.baseDamage;
        if (levelText) levelText.text = "Lv: " + data.level;
        if (xpText) xpText.text = "XP: " + data.xp + "/100";
        if (dexText) dexText.text = "DEX: " + data.baseDEX;

        if (characterImage) characterImage.sprite = data.characterSprite;

        // Buton görünürlükleri (savaşta gizlemek için showButtons=false gönder)
        if (detailButton) detailButton.gameObject.SetActive(showButtons && !isInBattle);
        if (selectButton) selectButton.gameObject.SetActive(showButtons && !isInBattle);

        // Varsayılan: ön yüz açık
        SetFaceUp(true);
    }

    /// Ön/arka yüz kontrolü (elde ters kart göstermek için)
    public void SetFaceUp(bool faceUp)
    {
        if (characterImage) characterImage.gameObject.SetActive(faceUp);
        if (backImage) backImage.gameObject.SetActive(!faceUp);
    }

    public void SetBattleMode(bool on)
    {
        isInBattle = on;

        if (nameText) nameText.gameObject.SetActive(!on);
        if (hpText) hpText.gameObject.SetActive(!on);
        if (dexText) dexText.gameObject.SetActive(!on);
        if (levelText) levelText.gameObject.SetActive(!on);
        if (xpText) xpText.gameObject.SetActive(!on);

        if (detailButton) detailButton.gameObject.SetActive(!on);
        // selectButton'ı savaş akışına göre açık/kapalı bırakabilirsin
    }

    // ---- Tıklama API'lerin (sende zaten vardı) ----
    public System.Action onClick;

    public void SetInteractable(bool b)
    {
        var btn = GetComponent<Button>();
        if (btn) btn.interactable = b;
    }

    public void OnButtonClick() => onClick?.Invoke();

    public void OnCardClicked()
    {
        if (!isInBattle)
            CardDetailPanel.Instance?.ShowCardDetails(cardData);
    }

    public void OnSelectClicked()
    {
        if (!isInBattle)
            DeckSelectPopup.Instance?.ShowDeckChoice(this.cardData);
    }
}
