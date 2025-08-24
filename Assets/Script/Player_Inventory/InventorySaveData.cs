using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySaveData
{
    public List<CardDTO> cards = new List<CardDTO>();
}

[Serializable]
public class CardDTO
{
    public string id;
    public string cardName;
    public string className;
    public string race;
    public string combo;

    public int baseHP;
    public int baseDamage;
    public int baseDEX;

    public string rarity;
    public string ability;
    public string passive;

    public int level;
    public int xp;
    public int skillCooldownMax;

   

    // Sprite diske yaz�lamaz. Sadece path tutuyoruz (iste�e ba�l�).
    public string spritePath;

    // 3D prefab i�in Resources yolu (varsa)
    public string prefabPath;
}

public static class CardDataMapper
{
    public static CardDTO ToDTO(CardData c)
    {
        return new CardDTO
        {
            id = c.id,
            cardName = c.cardName,
            className = c.className,
            race = c.race,
            combo = c.combo,

            baseHP = c.baseHP,
            baseDamage = c.baseDamage,
            baseDEX = c.baseDEX,

            rarity = c.rarity,
            ability = c.ability,
            passive = c.passive,

            level = c.level,
            xp = c.xp,
            skillCooldownMax = c.skillCooldownMax,

           

            // Sprite/Prefab i�in yaln�zca isim/yol sakl�yoruz (varsa)
            spritePath = c.characterSprite != null ? c.characterSprite.name : null,
            prefabPath = (c.characterPrefab3D != null) ? c.characterPrefab3D.name : null
        };
    }

    public static CardData FromDTO(CardDTO d)
    {
        // Sprite ve Prefab'� Resources'tan dene (iste�e ba�l�)
        Sprite sp = null;
        if (!string.IsNullOrEmpty(d.spritePath))
            sp = Resources.Load<Sprite>("Characters/" + d.spritePath);

        GameObject prefab = null;
        if (!string.IsNullOrEmpty(d.prefabPath))
        {
            // �nce Prefabs3D klas�r�nde ara, yoksa direkt isimle dene
            prefab = Resources.Load<GameObject>("Prefabs3D/" + d.prefabPath) ??
                     Resources.Load<GameObject>(d.prefabPath);
        }

        return new CardData(
            d.id,
            d.cardName,
            d.baseHP,
            d.baseDamage,
            d.baseDEX,
            d.rarity,
            d.ability,
            d.passive,
            d.level,
            d.xp,
            d.skillCooldownMax,
            sp,
            prefab
            
        );
    }
}



