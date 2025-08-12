using System.Collections.Generic;
using UnityEngine;

public class DeckManagerObject : MonoBehaviour
{
    public static DeckManagerObject Instance;

    [Header("Deste Ayarları")]
    public int deckMaxSize = 2; // İstersen 25 yap

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



    // Veriler
    public List<CardData> fullDeck = new();
    public List<CardData> currentMatchDeck = new();
    public List<CardData> matchDeck = new();
    public List<CardData> deck1 = new();
    public List<CardData> deck2 = new();
    public List<CardData> deck3 = new();
    public List<CardData> deck4 = new();
    public List<CardData> deck5 = new();

    [System.Serializable]
    public class CharacterSprite { public string characterName; public Sprite sprite; }
    public List<CharacterSprite> characterSprites = new();

    // Yardımcılar
    public Sprite GetSpriteByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        // 1) Inspector listesinden ara
        string Norm(string s) => s.Replace("_", " ").Trim().ToLowerInvariant();
        var m = characterSprites.Find(c => Norm(c.characterName) == Norm(name));
        if (m != null && m.sprite != null) return m.sprite;

        // 2) Resources’tan dene (Assets/Resources/Cards/...)
        // Denenecek isim varyasyonları
        var tries = new[]
        {
        name,
        name.Replace("_"," "),
        name.Replace(" ","_"),
        name.ToLowerInvariant(),
        name.Replace("_"," ").ToLowerInvariant(),
        name.Replace(" ","_").ToLowerInvariant()
    };

        foreach (var t in tries)
        {
            var sp = Resources.Load<Sprite>($"Characters/{t}");
            if (sp != null) return sp;
        }

        Debug.LogWarning($"[Sprite] Bulunamadı: {name}");
        return null;
    }



    public CardData GetCardById(string id)
    {
        foreach (var c in fullDeck)
            if (c != null && c.id == id) return c;
        Debug.LogWarning("❌ Kart bulunamadı, ID: " + id);
        return null;
    }

    public void PrepareMatchDeck(List<string> selectedCardIds)
    {
        currentMatchDeck.Clear();
        if (selectedCardIds == null) return;
        foreach (var id in selectedCardIds)
        {
            var c = GetCardById(id);
            if (c != null) currentMatchDeck.Add(c);
            else Debug.LogWarning($"❌ ID {id} ile kart bulunamadı.");
        }
        Debug.Log("🧩 Maç destesi hazır: " + currentMatchDeck.Count + " kart");
    }

    // 0..4 index
    public List<CardData> GetDeckByIndex(int idx)
    {
        return idx switch
        {
            0 => deck1,
            1 => deck2,
            2 => deck3,
            3 => deck4,
            4 => deck5,
            _ => null
        };
    }

    public List<List<CardData>> GetAllDecks()
    {
        return new List<List<CardData>> { deck1, deck2, deck3, deck4, deck5 };
    }

    public bool AddToDeck(int deckIndex, CardData card, int maxCount = -1)
    {
        var deck = GetDeckByIndex(deckIndex);
        if (deck == null || card == null) { Debug.LogWarning("Geçersiz deckIndex ya da kart."); return false; }

        int limit = (maxCount > 0) ? maxCount : deckMaxSize;
        if (deck.Count >= limit)
        {
            Debug.LogWarning($"⚠ {deckIndex + 1}. deste dolu ({deck.Count}/{limit})");
            return false;
        }

        // SPRITE OTO-DOLDUR
        if (card.characterSprite == null)
        {
            var sp = GetSpriteByName(card.cardName);
            if (sp != null) { card.characterSprite = sp; Debug.Log($"[Deck] Sprite atandı: {card.cardName}"); }
            else Debug.LogWarning($"[Deck] Sprite bulunamadı: {card.cardName}");
        }

        deck.Add(card);
        Debug.Log($"✅ {card.cardName} kartı {deckIndex + 1}. desteye eklendi. ({deck.Count}/{limit})");
        return true;
    }

    public void SelectDeckForBattle(int deckIndex)
    {
        var deck = GetDeckByIndex(deckIndex);
        if (deck == null || deck.Count != deckMaxSize)
        {
            Debug.LogWarning($"⚠ Lütfen {deckMaxSize} kartlık bir deste seçin.");
            return;
        }
        currentMatchDeck = deck;
        Debug.Log($"🎯 Savaş için deste seçildi! -> Deste {deckIndex + 1} ({deck.Count}/{deckMaxSize})");
    }

    public List<CardData> GetSelectedCards() => currentMatchDeck;
}
