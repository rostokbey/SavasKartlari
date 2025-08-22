using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Cards")]
    public List<CardData> myCards = new List<CardData>();

    // Profil kimliği: LocalLogin.cs buraya yazar/okur
    public static string CurrentProfileId { get; set; } = "default";

    // ---- Unity lifecycle ----
    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadFromDisk(); // oyuna girerken yükle
    }

    void OnApplicationQuit() => SaveToDisk();

    // ---- API ----
    public void AddCard(CardData card)
    {
        if (card == null) return;

        // Aynı ID varsa kopya eklemeyelim (istersen kaldır)
        if (myCards.Exists(c => c.id == card.id))
        {
            Debug.Log($"[Inventory] Kart zaten var: {card.id}");
            return;
        }

        myCards.Add(card);
        Debug.Log($"[Inventory] Kart eklendi: {card.cardName}");

        SaveToDisk(); // istersen yoruma al
    }

    public bool RemoveCard(string cardId)
    {
        int idx = myCards.FindIndex(c => c.id == cardId);
        if (idx >= 0)
        {
            myCards.RemoveAt(idx);
            SaveToDisk();
            return true;
        }
        return false;
    }

    // ---- Persist ----
    public void SaveToDisk()
    {
        var data = new InventorySaveData();
        foreach (var c in myCards)
            data.cards.Add(CardDataMapper.ToDTO(c));

        SaveSystem.SaveInventory(CurrentProfileId, data);
    }

    public void LoadFromDisk()
    {
        myCards.Clear();
        var data = SaveSystem.LoadInventory(CurrentProfileId);
        foreach (var dto in data.cards)
            myCards.Add(CardDataMapper.FromDTO(dto));

        Debug.Log($"[Inventory] Yüklendi: {myCards.Count} kart (profil: {CurrentProfileId})");
    }

    // ==========================================================
    // ====== Eski projeden kalan İSİMLER için "stub" metodlar ==
    // Bu metodlar başka scriptlerde referanslandığı için derleme
    // hatasını kesmek üzere eklendi. Oyun mekaniğine göre
    // istersen gerçek mantığı taşıyabilirsin.
    // ==========================================================

    // Örn: Aktif kart mantığın yoksa ilk kartı döndür.
    public CardData GetActiveCard()
    {
        return (myCards.Count > 0) ? myCards[0] : null;
    }

    // Örn: Her zaman kullanılabilir dön (mekaniğe bağla istersen)
    public bool CanUseSkill()
    {
        return true;
    }

    // Örn: hepsinin cooldown’unu sıfırla (eğer kullanıyorsan)
    public void ResetSkillCooldown()
    {
        // Eğer CardData’da per-card cooldown tutuyorsan burada sıfırla.
        Debug.Log("[Inventory] ResetSkillCooldown (stub)");
    }

    // Tur başlangıcında çağrılıyorsa, gerek varsa buraya yaz
    public void OnTurnStart()
    {
        Debug.Log("[Inventory] OnTurnStart (stub)");
    }
}
