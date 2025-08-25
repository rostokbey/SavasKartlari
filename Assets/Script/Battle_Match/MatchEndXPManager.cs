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

        foreach (var c in myUsed)
        {
            cls.AddExperience(c, delta);
            Debug.Log($"[XP] {c.cardName} ({c.id}) → ΔXP: {delta}, L:{c.level}, XP:{c.xp}/{cls.XpToNextLevel(c.level)}");
        }

        // İstersen burada sonucu UI’a bildir (popup vs.)
        // MatchResultUI.Show(myTeamWon, ...);
    }

    /// <summary>Geriye uyumluluk: eski kod sadece iki parametre ile çağırıyorsa.</summary>
    public void GrantMatchRewards(bool myTeamWon, List<CardData> myUsed)
    {
        GrantMatchRewards(myTeamWon, myUsed, null);
    }
}
