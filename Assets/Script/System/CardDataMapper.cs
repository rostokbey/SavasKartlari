public static class CardDataMapper
{
    // CardData -> CardDTO
    public static CardDTO ToDTO(CardData c)
    {
        var dto = new CardDTO
        {
            id = c.id,
            name = c.cardName,
            rarity = c.rarity,

            baseHP = c.baseHP,
            baseDamage = c.baseDamage,
            baseDex = c.baseDex,

            hp = c.hp,
            dex = c.dex,

            ability = c.ability,
            passive = c.passive,

            level = c.level,
            xp = c.xp,
            skillCooldownMax = c.skillCooldownMax,

            prefab = c.prefab,

            className = c.className,
            race = c.race,
            combo = c.combo
        };
        return dto;
    }

    // CardDTO -> CardData
    public static CardData FromDTO(CardDTO dto)
    {
        var c = new CardData
        {
            id = dto.id,
            cardName = dto.name,
            rarity = dto.rarity,

            baseHP = dto.baseHP,
            baseDamage = dto.baseDamage,
            baseDex = dto.baseDex,

            // dto.hp/dex yoksa base'e düþ
            hp = (dto.hp > 0) ? dto.hp : dto.baseHP,
            dex = (dto.dex > 0) ? dto.dex : dto.baseDex,

            ability = dto.ability,
            passive = dto.passive,

            level = dto.level,
            xp = dto.xp,
            skillCooldownMax = dto.skillCooldownMax,

            prefab = dto.prefab,

            className = dto.className,
            race = dto.race,
            combo = dto.combo,

            // runtime'da çözülecek
            characterSprite = null,
            characterPrefab3D = null
        };
        return c;
    }
}
