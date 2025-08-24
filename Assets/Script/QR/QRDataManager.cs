using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QRDataManager : MonoBehaviour
{
    public static QRDataManager Instance;

    [Header("Preview UI (opsiyonel)")]
    public Image characterImage;
    public TextMeshProUGUI nameText, hpText, strText, dexText, abilityText, passiveText;
    public Sprite defaultListSprite;

    /// Kart hazır olduğunda dışarı bildirmek için (opsiyonel)
    public static System.Action<CardData> OnCardReady;

    void Awake() => Instance = this;

    /// <summary>
    /// QR’dan gelen metin = JWT string
    /// </summary>
    public void ParseQRData(string jwtText)
    {
        if (string.IsNullOrWhiteSpace(jwtText))
        {
            Debug.LogWarning("[QR] Boş QR metni.");
            return;
        }

        // 1) JWT’yi çöz (imza doğrulama DEV’de kapalı olabilir)
        if (!JwtCardVerifier.TryParse(jwtText, out var p, out var err))
        {
            Debug.LogError("[QR] JWT doğrulama hatası: " + err);
            return;
        }

        // 2) UI önizleme (opsiyonel)
        if (nameText) nameText.text = p.name?.Replace("_", " ") ?? "";
        if (hpText) hpText.text = "HP: " + p.hp;
        if (strText) strText.text = "STR: " + p.str;
        if (dexText) dexText.text = "DEX: " + p.dex;         // Tokenında yoksa 0/varsayılan döner
        if (abilityText) abilityText.text = "Ability: " + p.ability;
        if (passiveText) passiveText.text = "Passive: " + p.passive;

        // Liste görseli (Characters/NAME ya da CardArtResolver iç mantığın)
        var listSprite = CardArtResolver.GetSprite(
            new CardData { cardName = p.name },
            defaultListSprite
        );
        if (characterImage)
        {
            characterImage.sprite = listSprite != null ? listSprite : defaultListSprite;
            characterImage.enabled = (characterImage.sprite != null);
        }

        // 3) CardData oluştur (prefab YÜKLEME! sadece yol string’i saklanır)
        var card = new CardData
        {
            id = p.id,
            cardName = p.name,
            rarity = p.rarity,

            baseHP = p.hp,
            baseDamage = p.str,
            baseDex = p.dex,          // payload’da yoksa 0 kalır; istersen burada bir başlangıç kuralı uygulayabiliriz
            dex = p.dex,

            ability = p.ability,
            passive = p.passive,

            level = 1,
            xp = 0,
            skillCooldownMax = 3,

            characterSprite = listSprite,  // UI list görseli
            characterPrefab3D = null,        // 3D prefabı sahnede Resources’tan yüklenecek
            prefab = p.prefab          // Örn: "Prefabs3D/AgirZirh_insan3D"
        };

        // 4) Envantere ekle
        var inv = PlayerInventory.Instance ?? FindObjectOfType<PlayerInventory>();
        if (inv != null)
        {
            inv.AddCard(card);
            Debug.Log($"[Inventory] eklendi: {card.cardName} ({card.id})");
        }
        else
        {
            Debug.LogWarning("[QR] PlayerInventory bulunamadı.");
        }

        // 5) Dışarı haber ver (opsiyonel)
        OnCardReady?.Invoke(card);
    }
}
