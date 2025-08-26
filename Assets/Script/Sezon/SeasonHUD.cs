using UnityEngine;
using TMPro;
using System;

public class SeasonHUD : MonoBehaviour
{
    public TMP_Text pointsText;
    public TMP_Text streakText;
    public TMP_Text timeLeftText;

    float nextUpdate;

    void Update()
    {
        if (Time.time < nextUpdate) return;
        nextUpdate = Time.time + 1f;

        var sm = SeasonManager.Instance;
        if (sm == null) return;

        int points = sm.GetMyPoints();
        bool eliminated = sm.IsEliminated();
        int streak = sm.GetMyLossStreak();       // SeasonManager’da küçük getter var
        TimeSpan left = sm.GetTimeLeft();

        if (pointsText) pointsText.text = eliminated ? $"Puan: {points} (Elenmiş)" : $"Puan: {points}";
        if (streakText) streakText.text = $"Yenilgi Serisi: {streak}";
        if (timeLeftText)
        {
            string t = left.TotalSeconds <= 0 ? "Sezon bitti" : $"{left.Days}g {left.Hours}s {left.Minutes}d";
            timeLeftText.text = $"Kalan: {t}";
        }
    }
}
