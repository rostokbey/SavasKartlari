using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class QRDataManager : MonoBehaviour
{
    public static QRDataManager Instance;

    [Header("Preview UI (opsiyonel)")]
    public Image characterImage;
    public TextMeshProUGUI nameText, hpText, strText, abilityText, passiveText;
    public Sprite defaultListSprite;

    // Kart hazır olduğunda dışarı bildirmek için (opsiyonel)
    public static Action<CardData> OnCardReady;

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

        // 1) JWT’yi çöz (imza doğrulama şimdilik kapalı olabilir)
        if (!JwtCardVerifier.TryParse(jwtText, out var p, out string err))
        {
            Debug.LogError("[QR] JWT çözümleme hatası: " + err);
            return;
        }

        // 2) UI önizleme (opsiyonel)
        if (nameText) nameText.text = p.name?.Replace("_", " ") ?? "";
        if (hpText) hpText.text = "HP: " + p.hp;
        if (strText) strText.text = "STR: " + p.str;
        if (abilityText) abilityText.text = "Ability: " + p.ability;
        if (passiveText) passiveText.text = "Passive: " + p.passive;

        // 3) Liste görseli: Resources/Characters/{ad} dene, yoksa default
        Sprite listSprite = null;
        if (!string.IsNullOrEmpty(p.name))
            listSprite = Resources.Load<Sprite>("Characters/" + p.name);
        if (!listSprite) listSprite = defaultListSprite;
        if (characterImage) characterImage.sprite = listSprite;

        // 4) CardData oluştur (yeni yapıcı: baseDex + dex)
        // Not: payload’da dex yoksa 0 bırakıyoruz; eğer eklediysen p.dex’i yazabilirsin.
        int baseDex = 0, dex = 0;

        var card = new CardData(
            p.id, p.name,
            p.hp, p.str,
            baseDex, dex,
            p.rarity, p.ability, p.passive,
            1,           // level
            0,           // xp
            3,           // skillCooldownMax (starter)
            listSprite,
            null,        // 3D prefab sahnede yüklenecekse burada null bırak
            p.prefab     // Resources yolu (ör: "Prefabs3D/AgirZirh_insan3D")
        );

        // 5) Envantere ekle
        var inv = PlayerInventory.Instance ?? FindObjectOfType<PlayerInventory>();
        if (inv != null)
        {
            inv.AddCard(card);
            Debug.Log($"[QR] Envantere eklendi: {card.cardName} ({card.id})");
        }
        else
        {
            Debug.LogWarning("[QR] PlayerInventory bulunamadı.");
        }

        // 6) Dışarı haber ver (opsiyonel)
        OnCardReady?.Invoke(card);
    }
}
