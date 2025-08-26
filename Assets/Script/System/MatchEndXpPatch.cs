using System.Collections.Generic;
using UnityEngine;

public static class MatchEndXpPatch
{
    /// <summary>
    /// XP�ler da��t�ld�ktan sonra �a��r. Season ma��ysa SeasonManager puan�n� tek kez i�ler.
    /// </summary>
    public static void AfterXpApplied(bool myTeamWon, IList<CardData> myUsed)
    {
        // (�stersen burada result snapshot�� haz�rlay�p MatchResultBus�a yazabilirsin)
        if (MatchContext.LastMatchWasSeason)
        {
            int teamSize = MatchContext.LastMatchTeamSize > 0 ? MatchContext.LastMatchTeamSize : 1;
            SeasonManager.Instance?.OnMatchFinished(myTeamWon, teamSize);
        }

        Debug.Log($"[MatchEndXpPatch] Done. Season={MatchContext.LastMatchWasSeason}, TeamSize={MatchContext.LastMatchTeamSize}");
    }
}

