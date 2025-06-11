using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<CardData> myCards = new();
    public int selectedCardIndex = 0;
    public int currentCooldown = 0;

    public CardData GetActiveCard()
    {
        if (myCards.Count > selectedCardIndex)
            return myCards[selectedCardIndex];
        return null;
    }

    public void AddCard(CardData card)
    {
        CardData existing = myCards.Find(c => c.id == card.id);
        if (existing == null)
        {
            myCards.Add(card);
            Debug.Log("Kart eklendi: " + card.cardName);
        }
        else
        {
            existing.xp += 10;
            if (existing.xp >= 100)
            {
                existing.level++;
                existing.baseHP += 10;
                existing.baseDamage += 2;
                existing.xp = 0;
                Debug.Log("Seviye atladı: " + existing.cardName);
            }
        }
    }
    public void AddCardFromExternal(CardData yeniOluşturulanCardData)
    {
        PlayerInventory inv = FindObjectOfType<PlayerInventory>();
        if (inv != null)
            inv.AddCard(yeniOluşturulanCardData);
        else
            Debug.LogWarning("PlayerInventory bulunamadı!");
    }

    public bool CanUseSkill()
    {
        return currentCooldown <= 0;
    }

    public void ResetSkillCooldown()
    {
        var card = GetActiveCard();
        currentCooldown = card != null ? card.skillCooldownMax : 0;
    }

    public void OnTurnStart()
    {
        if (currentCooldown > 0)
            currentCooldown--;
    }
}
