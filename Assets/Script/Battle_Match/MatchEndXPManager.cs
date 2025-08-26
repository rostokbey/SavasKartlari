using System.Collections.Generic;
using UnityEngine;

public class MatchEndXPManager : MonoBehaviour
{
    /// <summary>
    /// Esas imza: hangi takım kazandı, bizim takımda kullanılan kartlar, rakipte kullanılan kartlar.
    /// </summary>
    public void GrantMatchRewards(bool myTeamWon, List<CardData> myUsed, List<CardData> oppUsed)
    {
        if (myUsed == null || myUsed.Count == 0) return;

        var cls = CardLevelSystem.Instance;
        if (cls == null) { Debug.LogWarning("CardLevelSystem bulunamadı."); return; }

        int delta = myTeamWon
            ? cls.ComputeWinXp(myUsed, oppUsed)      // pozitif
            : cls.ComputeLossXp(myUsed, oppUsed);    // negatif

        // XP dağıtımı
        foreach (var c in myUsed)
        {
            cls.AddExperience(c, delta);
            Debug.Log($"[XP] {c.cardName} → Δ:{delta}, L:{c.level}, XP:{c.xp}/{cls.XpToNextLevel(c.level)}");
        }

        // Sezon puanı (yalnızca sezon maçında, 1 kez)
        if (MatchStarter.LastMatchWasSeason)
        {
            int teamSize = MatchStarter.LastMatchTeamSize > 0 ? MatchStarter.LastMatchTeamSize : 1;
            SeasonManager.Instance?.OnMatchFinished(myTeamWon, teamSize);
        }

    }
    /// <summary>Geriye uyumluluk: eski kod sadece iki parametre ile çağırıyorsa.</summary>
    public void GrantMatchRewards(bool myTeamWon, List<CardData> myUsed)
    {
        GrantMatchRewards(myTeamWon, myUsed, null);
    }
}
