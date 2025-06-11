using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject lobbyPanel;
    public GameObject deckPanel;
    public GameObject qrPanel;
    

    void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        HideAllPanels();
        mainMenuPanel.SetActive(true);
    }

    public void ShowLobby()
    {
        HideAllPanels();
        lobbyPanel.SetActive(true);
    }

    public void ShowDeck()
    {
        HideAllPanels();
        deckPanel.SetActive(true);
    }

    public void ShowQR()
    {
        HideAllPanels();
        qrPanel.SetActive(true);
    }

  

    public void BackToMenu()
    {
        ShowMainMenu();
    }

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        deckPanel.SetActive(false);
        qrPanel.SetActive(false);
        
    }

    public void QuitGame()
    {
        Debug.Log("Oyun kapatýlýyor...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}