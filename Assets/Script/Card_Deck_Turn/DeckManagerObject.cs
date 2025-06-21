using System.Collections.Generic;
using UnityEngine;

public class DeckManagerObject : MonoBehaviour
{
    public List<CardData> fullDeck = new();
    public List<CardData> currentMatchDeck = new();
    public List<CardData> matchDeck = new List<CardData>();  // Seçilen 25'lik savaş destesini tutar

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
}
