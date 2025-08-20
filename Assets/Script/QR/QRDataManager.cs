using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QRDataManager : MonoBehaviour
{
    public static QRDataManager Instance;

    [Header("Preview UI (opsiyonel)")]
    public Image characterImage;
    public TextMeshProUGUI nameText, hpText, strText, abilityText, passiveText;
    public Sprite defaultSprite;

    public static System.Action<CardData> OnCardReady;

    void Awake() => Instance = this;

    public void ParseQRData(string qrText)
    {
        // --- 1) QR sözlüğe ayrıştır ---
        var data = new Dictionary<string, string>();
        foreach (var part in qrText.Split('|'))
        {
            var pair = part.Split(':');
            if (pair.Length == 2) data[pair[0]] = pair[1];
        }

        string id = data.TryGetValue("ID", out var _id) ? _id : "NONE";
        string nameRaw = data.TryGetValue("NAME", out var _nm) ? _nm : "Unknown";
        string ability = data.TryGetValue("ABILITY", out var _ab) ? _ab : "None";
        string passive = data.TryGetValue("PASSIVE", out var _ps) ? _ps : "None";
        string rarity = data.TryGetValue("RARITY", out var _rt) ? _rt : "Yaygın";
        string prefabPath = data.TryGetValue("PREFAB", out var _pp) ? _pp?.Trim() : null;

        // Sayıları güvenli çevir
        int hp = (data.TryGetValue("HP", out var _hp) && int.TryParse(_hp, out var hpVal)) ? hpVal : 0;
        int str = (data.TryGetValue("STR", out var _str) && int.TryParse(_str, out var strVal)) ? strVal : 0;

        // UI önizleme (varsa)
        if (nameText) nameText.text = nameRaw.Replace("_", " ");
        if (hpText) hpText.text = "HP: " + hp;
        if (strText) strText.text = "STR: " + str;
        if (abilityText) abilityText.text = "Ability: " + ability;
        if (passiveText) passiveText.text = "Passive: " + passive;

        // 2D sprite (opsiyonel)
        var loadedSprite = Resources.Load<Sprite>("Characters/" + nameRaw);
        if (characterImage)
            characterImage.sprite = loadedSprite ? loadedSprite : defaultSprite;

        // --- 2) 3D Prefab bul ---
        GameObject loadedPrefab = null;

        // a) QR PREFAB alanı geldiyse önce onu dene
        if (!string.IsNullOrEmpty(prefabPath))
            loadedPrefab = Resources.Load<GameObject>(prefabPath);

        // b) Olmazsa NAME ile dene (Resources/Prefabs3D/NAME)
        if (!loadedPrefab)
            loadedPrefab = Resources.Load<GameObject>("Prefabs3D/" + nameRaw);

        if (loadedPrefab)
            Debug.Log("🧩 3D Prefab yüklendi: " + loadedPrefab.name);
        else
            Debug.LogWarning("❌ 3D Prefab bulunamadı. Aranan: " + (prefabPath ?? ("Prefabs3D/" + nameRaw)));

        // --- 3) CardData oluştur ---
        var card = new CardData(
            id: id,
            cardName: nameRaw,
            baseHP: hp,
            baseDamage: str,
            rarity: rarity,
            ability: ability,
            passive: passive,
            level: 1,
            xp: 0,
            skillCooldownMax: 3,
            characterSprite: characterImage ? characterImage.sprite : null
        );
        card.characterPrefab3D = loadedPrefab;

        // Envantere ve desteye ekle
        FindObjectOfType<PlayerInventory>()?.AddCard(card);
        FindObjectOfType<DeckManagerObject>()?.fullDeck.Add(card);

        // Dışarı haber ver (isteğe bağlı)
        OnCardReady?.Invoke(card);
    }
}
