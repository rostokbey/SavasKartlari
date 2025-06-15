using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManagerObject : MonoBehaviour
{
    public List<CardData> fullDeck = new();      // 25 kartlık tam deste
    public List<CardData> currentMatchDeck = new(); // Bu maçta kullanılabilir 12 kart

    public int maxDeckSize = 25;
    public int matchCardLimit = 12;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // 🌟 Sahne geçişinde silinmesin
    }

    public bool AddToDeck(CardData card)
    {
        if (fullDeck.Count >= maxDeckSize)
        {
            Debug.Log("Deste dolu (25 kart)!");
            return false;
        }

        if (!fullDeck.Exists(c => c.id == card.id))
        {
            fullDeck.Add(card);
            Debug.Log("Kart eklendi: " + card.cardName);

            return true;
        }

        Debug.Log("Kart zaten destede!");
        return false;
    }

    public void PrepareMatchDeck()
    {
        currentMatchDeck.Clear();

        if (fullDeck.Count < 12)
        {
            Debug.LogWarning("12'den az kart var!");
            currentMatchDeck.AddRange(fullDeck);
        }
        else
        {
            currentMatchDeck.AddRange(fullDeck.Take(12));
        }

        Debug.Log($"Maç destesi hazır: {currentMatchDeck.Count} kart");
    }

    public List<List<CardData>> savedDecks = new(); // Çoklu 25'lik deste

    public void SaveDeck(List<CardData> newDeck)
    {
        if (newDeck.Count == 25)
            savedDecks.Add(new List<CardData>(newDeck));
    }


    public bool IsCardInMatchDeck(string cardId)
    {
        return currentMatchDeck.Exists(c => c.id == cardId);
    }

    public CardData GetCardById(string cardId)
    {
        return currentMatchDeck.Find(c => c.id == cardId);
    }

    public void SelectDeck(int index)
    {
        if (index >= 0 && index < savedDecks.Count)
        {
            currentMatchDeck = new List<CardData>(savedDecks[index]);
            Debug.Log($"🎯 {index}. deste seçildi. Kart sayısı: {currentMatchDeck.Count}");
        }
        else
        {
            Debug.LogWarning("❌ Geçersiz deste indexi!");
        }
    }
}
