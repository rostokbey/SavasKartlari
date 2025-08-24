using System;

[Serializable]
public class CardDTO
{
    public string id;
    public string name;                // CardData.cardName
    public int rarity;

    public int baseHP;
    public int baseDamage;
    public int baseDex;

    public int hp;                     // runtime snapshot (yoksa base'e düşeceğiz)
    public int dex;                    // runtime snapshot

    public string ability;
    public string passive;

    public int level;
    public int xp;
    public int skillCooldownMax;

    public string prefab;              // Resources yolu (örn: "Prefabs3D/AgirZirh_insan3D")

    // Opsiyonel meta
    public string className;
    public string race;
    public string combo;
}
