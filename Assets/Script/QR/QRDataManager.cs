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
        Dictionary<string, string> data = new Dictionary<string, string>();
        string[] parts = qrText.Split('|');

        foreach (string part in parts)
        {
            string[] pair = part.Split(':');
            if (pair.Length == 2)
                data[pair[0]] = pair[1];
        }

        string id = data.ContainsKey("ID") ? data["ID"] : "NONE";
        string name = data.ContainsKey("NAME") ? data["NAME"] : "Unknown";
        string hp = data.ContainsKey("HP") ? data["HP"] : "0";
        string str = data.ContainsKey("STR") ? data["STR"] : "0";
        string ability = data.ContainsKey("ABILITY") ? data["ABILITY"] : "None";
        string passive = data.ContainsKey("PASSIVE") ? data["PASSIVE"] : "None";
        string rarity = data.ContainsKey("RARITY") ? data["RARITY"] : "Yaygın";

        nameText.text = name.Replace("_", " ");
        hpText.text = "HP: " + hp;
        strText.text = "STR: " + str;
        abilityText.text = "Ability: " + ability;
        passiveText.text = "Passive: " + passive;

        // ✅ Dinamik Sprite Yükleme
        Sprite loadedSprite = Resources.Load<Sprite>("Characters/" + name);
        if (loadedSprite != null)
        {
            Debug.Log("🖼️ Dinamik sprite yüklendi: " + loadedSprite.name);
            characterImage.sprite = loadedSprite;
        }
        else
        {
            Debug.LogWarning("❌ Dinamik sprite bulunamadı, default atanıyor: " + name);
            characterImage.sprite = defaultSprite;
        }

        CardData card = new CardData(
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

        FindObjectOfType<PlayerInventory>()?.AddCard(card);
        FindObjectOfType<DeckManagerObject>()?.fullDeck.Add(card);
    }
}
