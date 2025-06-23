using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBattleManager : MonoBehaviour
{
    public static StartBattleManager Instance;

    public List<CardData> selectedMatchCards = new();  // Oyuncunun seçtiği kartlar
    public List<CardData> enemyMatchCards = new();     // Düşmanın kartları

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Sahne değişiminde silinmesin
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartBattle()
    {
        // DeckManagerObject'ten seçilen kartları al
        DeckManagerObject deckManager = FindObjectOfType<DeckManagerObject>();

        if (deckManager == null)
        {
            Debug.LogError("❌ DeckManagerObject sahnede bulunamadı.");
            return;
        }

        if (deckManager.currentMatchDeck.Count == 0)
        {
            Debug.LogError("❌ Savaş başlamadan önce kart seçilmeli.");
            return;
        }

        selectedMatchCards = new List<CardData>(deckManager.currentMatchDeck);

        Debug.Log($"✅ StartBattleManager: {selectedMatchCards.Count} oyuncu kartı yüklendi.");

        // Geçici düşman kartları üret (ileride eşleşen rakipten gelecek)
        if (enemyMatchCards.Count == 0)
        {
            foreach (CardData card in selectedMatchCards)
            {
                CardData clone = card.Clone(); // Clone metodu olmalı (deep copy)
                clone.cardName += "_Enemy";
                enemyMatchCards.Add(clone);
            }

            Debug.Log($"🟥 StartBattleManager: {enemyMatchCards.Count} düşman kartı üretildi.");
        }

        // Savaş sahnesine geç
        SceneManager.LoadScene("BattleScene");
    }
}
