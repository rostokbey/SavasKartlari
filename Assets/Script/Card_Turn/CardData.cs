using UnityEngine;

[System.Serializable]
public class CardData
{
    public string id;
    public string cardName;
    public int baseHP;
    public int baseDamage;
    public int dex;
    public string rarity;
    public string ability;
    public string passive;
    public int level;
    public int xp;
    public int skillCooldownMax;
    public Sprite characterSprite;

    // 👇 Detay paneli için gereken alanlar (bunlar eksikmiş)

    public string cardClass;
    public string race;
    public string combination;
    public string lore;
    public string combo;
    public string story;
    public string className;





    public CardData(string id, string cardName, int baseHP, int baseDamage, string rarity,
                    string ability, string passive, int level, int xp, int skillCooldownMax,
                    Sprite characterSprite)
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
    }

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
            characterSprite
        )
        {
            dex = this.dex,
            cardClass = this.cardClass,
            race = this.race,
            combination = this.combination,
            lore = this.lore,
            combo = this.combo,
            story = this.story,
            className = this.className
        };
    }
}
