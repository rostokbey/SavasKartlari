using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBattleManager : MonoBehaviour
{
    public static StartBattleManager Instance;

    public List<CardData> currentPlayerDeck;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne değişiminde kaybolmasın
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Maça girerken oyuncunun kartlarını gönder
    /// </summary>
    public void StartBattle(List<CardData> playerDeck)
    {
        if (playerDeck == null || playerDeck.Count == 0)
        {
            Debug.LogError("❌ StartBattle: Oyuncu destesi boş!");
            return;
        }

        currentPlayerDeck = new List<CardData>(playerDeck); // Maç için desteyi kaydet
        Debug.Log("✅ Savaş başlatılıyor. Kart sayısı: " + currentPlayerDeck.Count);

        // Battle sahnesine geç
        SceneManager.LoadScene("BattleScene");
    }
}
