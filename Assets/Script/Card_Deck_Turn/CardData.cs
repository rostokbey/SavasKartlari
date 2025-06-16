using UnityEngine;

[System.Serializable]
public class CardData
{
    public string id;
    public string cardName;
    public int baseHP;
    public int baseDamage;
    public string rarity;
    public string ability;
    public string passive;
    public int level;
    public int xp;
    public int skillCooldownMax;
    public Sprite characterSprite;

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
}
