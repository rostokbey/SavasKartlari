using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManagerObject : MonoBehaviour
{
    public List<CardData> fullDeck = new();      // 25 kartlýk tam deste
    public List<CardData> currentMatchDeck = new(); // Bu maçta kullanýlabilir 12 kart

    public int maxDeckSize = 25;
    public int matchCardLimit = 12;

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

        if (fullDeck.Count < 2)
        {
            Debug.LogWarning("2'den az kart var!");
            currentMatchDeck.AddRange(fullDeck);
        }
        else
        {
            currentMatchDeck.AddRange(fullDeck.Take(12));
        }

        Debug.Log($"Maç destesi hazýr: {currentMatchDeck.Count} kart");
    }

    public bool IsCardInMatchDeck(string cardId)
    {
        return currentMatchDeck.Exists(c => c.id == cardId);
    }

    public CardData GetCardById(string cardId)
    {
        return currentMatchDeck.Find(c => c.id == cardId);
    }
}
