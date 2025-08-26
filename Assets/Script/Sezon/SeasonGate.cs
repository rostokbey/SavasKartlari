using UnityEngine;

public class SeasonGate : MonoBehaviour
{
    public void TryStartMatch(int teamSize)
    {
        if (SeasonManager.Instance?.IsEliminated() == true)
        {
            Debug.LogWarning("Bu sezon elendiniz (puan=0).");
            return;
        }
        MatchStarter.Instance?.StartMatch(teamSize, true); // SEASON = true
    }
}
