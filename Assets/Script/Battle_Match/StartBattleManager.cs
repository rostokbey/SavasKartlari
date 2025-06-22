

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBattleManager : MonoBehaviour
{
    public static StartBattleManager Instance;

    [HideInInspector]
    public List<CardData> selectedMatchCards = new();

    void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne geçişlerinde yok olmasın
        }
        else
        {
            Destroy(gameObject); // Zaten varsa yenisini sil
        }
    }

    public void SetSelectedCards(List<CardData> cards)
    {
        selectedMatchCards = new List<CardData>(cards);
    }

    public List<CardData> GetSelectedCards()
    {
        return selectedMatchCards;
    }
    public void StartBattle()
    {
        var deckManager = FindObjectOfType<DeckManagerObject>();
        if (deckManager == null || deckManager.currentMatchDeck.Count == 0)
        {
            Debug.LogError("❌ Maç destesi boş veya DeckManagerObject bulunamadı.");
            return;
        }

        selectedMatchCards = new List<CardData>(deckManager.currentMatchDeck);
        SceneManager.LoadScene("BattleScene");
    }


}
