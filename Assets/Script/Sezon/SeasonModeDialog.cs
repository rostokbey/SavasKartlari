// Assets/Script/UI/SeasonModeDialog.cs
using UnityEngine;

public class SeasonModeDialog : MonoBehaviour
{
    public GameObject panel;  // Bu scriptin baðlý olduðu Panel objesini buraya sürükle

    public void Open() { if (panel) panel.SetActive(true); }
    public void Close() { if (panel) panel.SetActive(false); }

    public void Play1v1() { Close(); MatchStarter.Instance?.StartMatch(1, true); }
    public void Play2v2() { Close(); MatchStarter.Instance?.StartMatch(2, true); }
    public void Play3v3() { Close(); MatchStarter.Instance?.StartMatch(3, true); }
}
