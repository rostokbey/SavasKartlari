using System.Collections.Generic;
using UnityEngine;

public class DeckManagerObject : MonoBehaviour
{
    public List<CardData> fullDeck = new();
    public List<CardData> currentMatchDeck = new();
    public List<CardData> matchDeck = new List<CardData>();
    public List<CardData> deck1 = new();
    public List<CardData> deck2 = new();
    public List<CardData> deck3 = new();
    public List<CardData> deck4 = new();
    public List<CardData> deck5 = new();

    [System.Serializable]
    public class CharacterSprite
    {
        public string characterName;
        public Sprite sprite;
    }

    public List<CharacterSprite> characterSprites;

    public Sprite GetSpriteByName(string name)
    {
        var match = characterSprites.Find(c => c.characterName == name);
        return match != null ? match.sprite : null;
    }

    public CardData GetCardById(string id)
    {
        foreach (var card in fullDeck)
        {
            if (card.id == id)
                return card;
        }

        Debug.LogWarning("❌ Kart bulunamadı, ID: " + id);
        return null;
    }

    public void PrepareMatchDeck(List<string> selectedCardIds)
    {
        currentMatchDeck.Clear();

        foreach (string id in selectedCardIds)
        {
            CardData card = GetCardById(id);
            if (card != null)
                currentMatchDeck.Add(card);
            else
                Debug.LogWarning($"❌ ID {id} ile kart bulunamadı.");
        }

        Debug.Log("🧩 Maç destesi hazır: " + currentMatchDeck.Count + " kart");
    }

    public void SelectDeckForBattle(int deckIndex)
    {
        List<CardData> selectedDeck = deckIndex switch
        {
            0 => deck1,
            1 => deck2,
            2 => deck3,
            3 => deck4,
            4 => deck5,
            _ => null
        };

        if (selectedDeck == null || selectedDeck.Count != 25)
        {
            Debug.LogWarning("⚠ Lütfen 25 kartlık bir deste seçin.");
            return;
        }

        currentMatchDeck = selectedDeck;
        Debug.Log("🎯 Savaş için deste seçildi!");
    }


    public List<CardData> GetSelectedCards()
    {
        return currentMatchDeck;
    }
}
