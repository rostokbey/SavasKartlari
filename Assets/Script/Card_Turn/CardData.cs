using UnityEngine;

[System.Serializable]
public class CardData
{
    public string id; // Kartın benzersiz kimliği (örneğin: "ID001")
    public string cardName; // Kartın adı (örneğin: "Misilleme_Savaşçısı")
    public string className;   // Karakterin sınıfı (örneğin: Warrior, Mage)
    public string race;        // Karakterin ırkı (örneğin: Human, Orc)
    public string combo;       // Kombinasyon açıklaması (örneğin: "Fire + Sword")
    public int baseHP; // Kartın başlangıç can değeri
    public int baseDamage; // Kartın temel saldırı gücü
    public string rarity; // Kartın nadirlik seviyesi ("Yaygın", "Nadir", "Efsanevi" vs.)
    public string ability; // Kartın özel yeteneği (örneğin: "Alev Darbesi")
    public string passive; // Kartın pasif etkisi (örneğin: "Zırh Artışı")
    public int level; // Kartın seviyesi (örn: 1'den başlar)
    public int xp; // Kartın mevcut deneyim puanı
    public int skillCooldownMax; // Yeteneğin kaç turda yeniden kullanılabileceği
    public int dex; // Kartın çeviklik (hız/öncelik) değeri

    public Sprite characterSprite; // Kartın 2D görünümünü temsil eden sprite (envanterde ve UI'da gösterilir)
    public GameObject characterPrefab3D; // Kartın savaş sahnesindeki 3D model prefabı

    // Yapıcı (constructor) - Yeni bir kart oluştururken kullanılır
    public CardData(
        string id,
        string cardName,
        int baseHP,
        int baseDamage,
        string rarity,
        string ability,
        string passive,
        int level,
        int xp,
        int skillCooldownMax,
        Sprite characterSprite = null,
        GameObject characterPrefab3D = null,
        int dex = 10
    )
    {
        this.id = id;
        this.cardName = cardName;
        this.baseHP = baseHP;
        this.baseDamage = baseDamage;
        this.rarity = rarity;
        this.ability = ability;
        this.passive = passive;
        this.level = level;
        this.xp = xp;
        this.skillCooldownMax = skillCooldownMax;
        this.characterSprite = characterSprite;
        this.characterPrefab3D = characterPrefab3D;
        this.dex = dex;
    }

    // Kartın birebir kopyasını oluşturur (klonlama)
    public CardData Clone()
    {
        return new CardData(
            id,
            cardName,
            baseHP,
            baseDamage,
            rarity,
            ability,
            passive,
            level,
            xp,
            skillCooldownMax,
            characterSprite,
            characterPrefab3D,
            dex
        );
    }
}
