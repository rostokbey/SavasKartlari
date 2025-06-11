// MatchEndXPManager.cs – Maç sonu XP ve seviye sistemi

using UnityEngine;
using System.Collections.Generic;

public class MatchEndXPManager : MonoBehaviour
{
    public int xpPerWin = 50;
    public int xpPerLoss = 20;
    public int xpToLevelUp = 100;

    public void GrantMatchRewards(bool isWinner, List<CardData> usedCards)
    {
        int xpGained = isWinner ? xpPerWin : xpPerLoss;

        foreach (var card in usedCards)
        {
            card.xp += xpGained;
            Debug.Log($"{card.cardName} kartý {xpGained} XP kazandý (Toplam XP: {card.xp})");

            if (card.xp >= xpToLevelUp)
            {
                card.level++;
                card.baseHP += 10;
                card.baseDamage += 2;
                card.xp = 0;

                Debug.Log($"{card.cardName} seviye atladý! Yeni seviye: {card.level}");
            }
        }
    }
}
