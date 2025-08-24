using UnityEngine;
using System.Collections.Generic;

/// Maç bitiminde çaðýr: iki tarafýn sahaya çýkan kart listeleriyle.
public class MatchEndXPManager : MonoBehaviour
{
    public void GrantMatchRewards(bool myTeamWon, List<CardData> myUsed, List<CardData> oppUsed)
    {
        if (myUsed == null) myUsed = new List<CardData>();
        if (oppUsed == null) oppUsed = new List<CardData>();

        if (myTeamWon)
        {
            int xp = CardLevelSystem.Instance.ComputeWinXp(myUsed, oppUsed);
            foreach (var c in myUsed) CardLevelSystem.Instance.AddXP(c, xp);
        }
        else
        {
            int xp = CardLevelSystem.Instance.ComputeLossXp(myUsed, oppUsed); // negatif
            foreach (var c in myUsed) CardLevelSystem.Instance.AddXP(c, xp);
        }

        PlayerInventory.Instance?.SaveToDisk();
    }
}
