using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QRDataManager : MonoBehaviour
{
    public static QRDataManager Instance;

    public Image characterImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI strText;
    public TextMeshProUGUI abilityText;
    public TextMeshProUGUI passiveText;

    public Sprite defaultSprite;

    private void Awake()
    {
        Instance = this;
    }

    public void ParseQRData(string qrText)
    {
        var data = new Dictionary<string, string>();
        foreach (var part in qrText.Split('|'))
        {
            var pair = part.Split(':');
            if (pair.Length == 2) data[pair[0]] = pair[1];
        }

        string id = data.TryGetValue("ID", out var _id) ? _id : "NONE";
        string name = data.TryGetValue("NAME", out var _nm) ? _nm : "Unknown";
        string hp = data.TryGetValue("HP", out var _hp) ? _hp : "0";
        string str = data.TryGetValue("STR", out var _str) ? _str : "0";
        string ability = data.TryGetValue("ABILITY", out var _ab) ? _ab : "None";
        string passive = data.TryGetValue("PASSIVE", out var _ps) ? _ps : "None";
        string rarity = data.TryGetValue("RARITY", out var _rt) ? _rt : "Yaygın";
        string prefabPath = data.TryGetValue("PREFAB", out var _pp) ? _pp : null;

        // UI
        nameText.text = name.Replace("_", " ");
        hpText.text = "HP: " + hp;
        strText.text = "STR: " + str;
        abilityText.text = "Ability: " + ability;
        passiveText.text = "Passive: " + passive;

        // 2D sprite (opsiyonel)
        var loadedSprite = Resources.Load<Sprite>("Characters/" + name);
        characterImage.sprite = loadedSprite != null ? loadedSprite : defaultSprite;

        // 3D model (blend/prefab)
        GameObject loadedPrefab = null;

        // 1) QR’dan geldi ise onu dene
        if (!string.IsNullOrEmpty(prefabPath))
            loadedPrefab = Resources.Load<GameObject>(prefabPath);

        // 2) Gelmediyse NAME ile otomatik dene
        if (loadedPrefab == null)
            loadedPrefab = Resources.Load<GameObject>("Prefabs3D/" + name);

        if (loadedPrefab != null)
            Debug.Log("🧩 3D Prefab yüklendi: " + loadedPrefab.name);
        else
            Debug.LogWarning("❌ 3D Prefab bulunamadı. Aranan yol: " + (prefabPath ?? ("Prefabs3D/" + name)));

        // Kart verisi
        var card = new CardData(
            id: id,
            cardName: name,
            baseHP: int.Parse(hp),
            baseDamage: int.Parse(str),
            rarity: rarity,
            ability: ability,
            passive: passive,
            level: 1,
            xp: 0,
            skillCooldownMax: 3,
            characterSprite: characterImage.sprite
        );
        card.characterPrefab3D = loadedPrefab;

        FindObjectOfType<PlayerInventory>()?.AddCard(card);
        FindObjectOfType<DeckManagerObject>()?.fullDeck.Add(card);

        // (Opsiyonel) dinleyenlere haber ver – test spawn için çok işe yarar
        OnCardReady?.Invoke(card);
    }

    // Kart hazır olduğunda tetiklenecek event (opsiyonel)
    public static System.Action<CardData> OnCardReady;

}

