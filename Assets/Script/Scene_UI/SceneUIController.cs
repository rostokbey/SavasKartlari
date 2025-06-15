using UnityEngine;

public class SceneUIController : MonoBehaviour
{
    public enum TargetPanel
    {
        Lobby,
        QR,
        DeckBuilder,
        DeckSelect,
        BattleUI,
        Result
    }

    public GameObject lobbyPanel;
    public GameObject qrPanel;
    public GameObject deckBuilderPanel;
    public GameObject deckSelectPanel;
    public GameObject battleUIPanel;
    public GameObject resultPanel;

    void Start()
    {
        ShowOnly(TargetPanel.Lobby);
    }

    public void ShowOnly(TargetPanel target)
    {
        lobbyPanel.SetActive(target == TargetPanel.Lobby);
        qrPanel.SetActive(target == TargetPanel.QR);
        deckBuilderPanel.SetActive(target == TargetPanel.DeckBuilder);
        deckSelectPanel.SetActive(target == TargetPanel.DeckSelect);
        battleUIPanel.SetActive(target == TargetPanel.BattleUI);
        resultPanel.SetActive(target == TargetPanel.Result);
    }
}
