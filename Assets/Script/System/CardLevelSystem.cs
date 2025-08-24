// Dosya adı: CardLevelSystem.cs
using UnityEngine;

public class CardLevelSystem : MonoBehaviour
{
    public static CardLevelSystem Instance { get; private set; }

    [Header("XP Ayarları")]
    public int maxLevel = 99;
    public int duplicateScanXpDefault = 100;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // ----------------- Dış API -----------------

    public void AddExperienceByKey(string cardId, int xp)
    {
        var inv = PlayerInventory.Instance;
        if (inv == null) return;

        var c = inv.myCards.Find(x => x.id == cardId);
        if (c == null) return;

        AddExperience(c, xp);
        inv.SaveToDisk();
    }

    public void AddExperience(CardData c, int xp)
    {
        if (c == null) return;
        if (c.level >= maxLevel) return;

        c.xp += Mathf.Max(0, xp);

        while (c.level < maxLevel && c.xp >= XpForNextLevel(c.level))
        {
            c.xp -= XpForNextLevel(c.level);
            c.level++;
            ApplyLevelGains(c, c.level);
        }
    }

    public void ApplyToCard(CardData c)
    {
        if (c == null) return;

        // Görünen değerleri base’ten başlayıp, mevcut level’e kadar kazanç ekleyerek kur.
        int hp = c.baseHP;
        int str = c.baseDamage;
        int dx = (c.dex > 0) ? c.dex : c.baseDex;

        for (int L = 2; L <= Mathf.Max(1, c.level); L++)
            AddRoleBasedGains(c, L, ref hp, ref str, ref dx);

        c.baseHP = hp;
        c.baseDamage = str;
        c.dex = dx;
    }

    // ----------------- İç Hesaplar -----------------

    public int XpForNextLevel(int level)
    {
        // Basit artan eğri; tablolayabiliriz
        float v = 100f * Mathf.Pow(1.08f, Mathf.Max(0, level - 1));
        return Mathf.RoundToInt(v);
    }

    public void ApplyLevelGains(CardData c, int targetLevel)
    {
        int hp = c.baseHP;
        int str = c.baseDamage;
        int dx = (c.dex > 0) ? c.dex : c.baseDex;

        AddRoleBasedGains(c, targetLevel, ref hp, ref str, ref dx);

        c.baseHP = hp;
        c.baseDamage = str;
        c.dex = dx;
    }

    enum Role { Tank, Warrior, Mage, Support, Assassin, Unknown }

    Role ResolveRole(CardData c)
    {
        string n = (c.cardName ?? "").ToLowerInvariant();
        string r = (c.rarity ?? "").ToLowerInvariant();
        string a = (c.ability ?? "").ToLowerInvariant();

        if (n.Contains("suikast") || n.Contains("assassin") || n.Contains("rogue")) return Role.Assassin;
        if (n.Contains("tank") || r.Contains("tank")) return Role.Tank;
        if (n.Contains("büy") || n.Contains("mage") || a.Contains("büy")) return Role.Mage;
        if (n.Contains("destek") || n.Contains("support") || a.Contains("heal")) return Role.Support;
        if (n.Contains("savaş") || n.Contains("war") || r.Contains("fighter")) return Role.Warrior;

        return Role.Unknown;
    }

    bool IsLegendaryGroup(CardData c)
    {
        string r = (c.rarity ?? "").ToLowerInvariant();
        return r.Contains("efsane") || r.Contains("legend") || r.Contains("multi");
    }

    void AddRoleBasedGains(CardData c, int levelStep, ref int hp, ref int str, ref int dex)
    {
        var role = ResolveRole(c);
        bool legendaryGroup = IsLegendaryGroup(c);

        switch (role)
        {
            case Role.Tank:
                hp += 14; str += 2; dex += 6; break;

            case Role.Warrior:
                hp += 10; str += 8; dex += 2; break;

            case Role.Mage:
                hp += 10; str += 6; dex += 4; break;

            case Role.Support:
                hp += 10; str += 3; dex += 3; break;

            case Role.Assassin:
                hp += 10;
                str += legendaryGroup ? 11 : 9;
                dex += legendaryGroup ? 2 : 1;
                break;

            default:
                hp += 10; str += 5; dex += 2; break;
        }
    }
}
