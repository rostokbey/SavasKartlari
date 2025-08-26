using UnityEngine;
using TMPro;
using System.Text;

public class SeasonStandingsUI : MonoBehaviour
{
    public TMP_Text standingsText;

    public void Refresh()
    {
        var sm = SeasonManager.Instance;
        if (sm == null || standingsText == null) return;

        var list = sm.BuildStandings();
        var sb = new StringBuilder();

        int max = Mathf.Min(10, list.Count);
        for (int i = 0; i < max; i++)
        {
            var s = list[i];
            sb.AppendLine(
                $"{i + 1}. {s.profileId} — {s.points}p  (W{s.wins}/L{s.losses})  M:{s.medals} K:{s.cups} Y:{s.stars}"
            );
        }
        standingsText.text = sb.Length > 0 ? sb.ToString() : "Henüz kayýt yok.";
    }

    void OnEnable() => Refresh();
}
