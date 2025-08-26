// Assets/Script/UI/UIStateRouter.cs
using System.Collections.Generic;
using UnityEngine;

public class UIStateRouter : MonoBehaviour
{
    public GameObject lobbyPanel;
    public GameObject qrPanel;
    public GameObject deckBuilderPanel;
    public GameObject deckSelectPanel;
    public GameObject seasonPanel;
    public GameObject friendsPanel;
    public GameObject clanPanel;
    public GameObject resultsPanel;

    Dictionary<string, GameObject> _map;

    void Awake()
    {
        _map = new Dictionary<string, GameObject> {
            { "Lobby", lobbyPanel },
            { "QR", qrPanel },
            { "DeckBuilder", deckBuilderPanel },
            { "DeckSelect", deckSelectPanel },
            { "Season", seasonPanel },
            { "Friends", friendsPanel },
            { "Clan", clanPanel },
            { "Results", resultsPanel },
        };
        Show("Lobby");
    }

    public void Show(string key)
    {
        foreach (var kv in _map)
            if (kv.Value) kv.Value.SetActive(false);

        if (_map.TryGetValue(key, out var go) && go)
            go.SetActive(true);
    }

    // Bottom Nav butonlarýnda bu metodlarý çaðýrmasý rahat
    public void ShowLobby() => Show("Lobby");
    public void ShowQR() => Show("QR");
    public void ShowDeckBuilder() => Show("DeckBuilder");
    public void ShowDeckSelect() => Show("DeckSelect");
    public void ShowSeason() => Show("Season");
    public void ShowFriends() => Show("Friends");
    public void ShowClan() => Show("Clan");
    public void ShowResults() => Show("Results");
}
