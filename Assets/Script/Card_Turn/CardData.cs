using UnityEngine;

[System.Serializable]
public class CardData
{
    // Kimlik / temel bilgiler
    public string id;
    public string cardName;

    // Temel istatistikler (base değerler kayıtta tutulur)
    public int baseHP;
    public int baseDamage;

    // Yeni: savunma/dex
    public int baseDex;
    public int dex;

    // Tanımsal alanlar
    public string rarity;   // örn: Common/Rare/Legendary...
    public string ability;
    public string passive;

    // İlerleme
    public int level = 1;
    public int xp = 0;
    public int skillCooldownMax = 3;

    // Görseller
    public Sprite characterSprite;     // liste/kart UI sprite
    public GameObject characterPrefab3D; // sahnede spawn edilecek 3D prefab (isteğe bağlı)

    // ÖNEMLİ: 3D prefab yolu (Resources köküne göre)
    // örn: "Prefabs3D/AgirZirh_insan3D"
    public string prefab;

    public CardData Clone()
    {
        return new CardData
        {
            id = id,
            cardName = cardName,

            baseHP = baseHP,
            baseDamage = baseDamage,

            baseDex = baseDex,
            dex = dex,

            rarity = rarity,
            ability = ability,
            passive = passive,

            level = level,
            xp = xp,
            skillCooldownMax = skillCooldownMax,

            characterSprite = characterSprite,
            characterPrefab3D = characterPrefab3D,

            prefab = prefab
        };
    }
}
