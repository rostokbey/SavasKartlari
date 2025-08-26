using UnityEngine;
using TMPro;
using System.Text;

public class MatchResultUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text headerText;
    public TMP_Text bodyText;

    void OnEnable()
    {
        if (!panel) return;
        panel.SetActive(false);
    }

    public void Show()
    {
        if (!panel) return;

        var sb = new StringBuilder();
        foreach (var d in MatchResultBus.LastCards)
        {
            sb.AppendLine(
                $"{d.card.cardName}  L:{d.oldLevel}->{d.newLevel}  XP:{d.oldXp}->{d.newXp}"
            );
        }

        string seasonLine = "";
        if (MatchResultBus.LastWasSeason)
        {
            bool eliminated = SeasonManager.Instance?.IsEliminated() ?? false;
            int pts = SeasonManager.Instance?.GetMyPoints() ?? 0;
            seasonLine = eliminated
                ? $"\nSezon: ELENDİN (puan=0)"
                : $"\nSezon Puanın: {pts}";
        }

        if (headerText) headerText.text = MatchResultBus.LastWin ? "Zafer!" : "Mağlubiyet";
        if (bodyText) bodyText.text = sb.ToString() + seasonLine;

        panel.SetActive(true);
    }

    public void PlayAgain()
    {
        panel.SetActive(false);
        // burada tekrar akışını tetikle (örn. BattleStartAdapter.StartCasual1v1() vb.)
    }

    public void GoLobby()
    {
        panel.SetActive(false);
        // UIStateRouter.ShowLobby() vb.
    }
}
