using UnityEngine;

public class CardData
{
    // Kimlik / İsim
    public string id;
    public string cardName;

    // Base istatistikler
    public int baseHP;
    public int baseDamage;

    // DEX: base ve anlık
    public int baseDex;   // küçük x
    public int dex;       // küçük x

    // Diğer özellikler
    public string rarity;
    public string ability;
    public string passive;

    // RPG ilerleme
    public int level;
    public int xp;
    public int skillCooldownMax;

    // Sınıf/ırk/kombo (opsiyonel)
    public string className;
    public string race;
    public string combo;

    // Görseller / Prefab’lar
    public Sprite characterSprite;      // 2D liste görseli
    public GameObject characterPrefab3D;// sahnede spawn’lanacak 3D prefab (istenirse)
    public string prefab;               // Resources yolu (örn: "Prefabs3D/AgirZirh_insan3D")

    // ---------------- Constructors ----------------

    /// <summary>
    /// GERİYE UYUMLU ESKİ SÜRÜM (tek dex parametresi alır)
    /// </summary>
    public CardData(
        string id, string name, int hp, int dmg, int dex,
        string rarity, string ability, string passive,
        int level, int xp, int skillCdMax,
        Sprite sp, GameObject prefab3d, string prefabPath = null)
    {
        this.id = id;
        this.cardName = name;

        this.baseHP = hp;
        this.baseDamage = dmg;

        this.baseDex = dex; // tek parametre geldiyse ikisine de yaz
        this.dex = dex;

        this.rarity = rarity;
        this.ability = ability;
        this.passive = passive;

        this.level = level;
        this.xp = xp;
        this.skillCooldownMax = skillCdMax;

        this.characterSprite = sp;
        this.characterPrefab3D = prefab3d;
        this.prefab = prefabPath;
    }

    /// <summary>
    /// YENİ SÜRÜM (baseDex + dex birlikte)
    /// </summary>
    public CardData(
        string id, string name, int hp, int dmg, int baseDex, int dex,
        string rarity, string ability, string passive,
        int level, int xp, int skillCdMax,
        Sprite sp, GameObject prefab3d, string prefabPath = null)
    {
        this.id = id;
        this.cardName = name;

        this.baseHP = hp;
        this.baseDamage = dmg;

        this.baseDex = baseDex;
        this.dex = dex;

        this.rarity = rarity;
        this.ability = ability;
        this.passive = passive;

        this.level = level;
        this.xp = xp;
        this.skillCooldownMax = skillCdMax;

        this.characterSprite = sp;
        this.characterPrefab3D = prefab3d;
        this.prefab = prefabPath;
    }

    // Basit kopya (ui/listelerde işine yarar)
    public CardData Clone()
    {
        return new CardData(
            id, cardName,
            baseHP, baseDamage,
            baseDex, dex,
            rarity, ability, passive,
            level, xp, skillCooldownMax,
            characterSprite, characterPrefab3D, prefab
        )
        {
            className = this.className,
            race = this.race,
            combo = this.combo
        };
    }
}
