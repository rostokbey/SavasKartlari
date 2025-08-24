using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Cards")]
    public List<CardData> myCards = new List<CardData>();

    public static string CurrentProfileId { get; set; } = "DEFAULT";

    [SerializeField] float autoSaveInterval = 210f;
    Coroutine autoSaveCo;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadFromDisk();                    // oyuna girerken yükle
        if (autoSaveCo == null) autoSaveCo = StartCoroutine(AutoSaveLoop());
    }

    IEnumerator AutoSaveLoop()
    {
        var w = new WaitForSeconds(autoSaveInterval);
        while (true)
        {
            yield return w;
            SaveToDisk();
        }
    }

    void OnApplicationQuit() => SaveToDisk();
    void OnApplicationPause(bool pause) { if (pause) SaveToDisk(); }
    void OnApplicationFocus(bool hasFocus) { if (!hasFocus) SaveToDisk(); }
    void OnDestroy() => SaveToDisk();

    // ----------------------------------------------------------
    //  Envanter API
    // ----------------------------------------------------------

    public void AddCard(CardData card, int duplicateScanXp = 100)
    {
        if (card == null) return;

        // Aynı id'li kart daha önce eklenmişse → kopya yerine XP ver
        var existing = myCards.Find(c => c.id == card.id);
        if (existing != null)
        {
            CardLevelSystem.Instance?.AddExperience(existing, duplicateScanXp);
            SaveToDisk();
            Debug.Log($"[Inventory] Kopya QR: {existing.cardName} +{duplicateScanXp} XP verildi.");
            return;
        }

        // Yeni kart
        myCards.Add(card);
        SaveToDisk();
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

    // ----------------------------------------------------------
    //  Save / Load
    // ----------------------------------------------------------

    public void SaveToDisk()
    {
        // InventorySaveData { List<CardDTO> cards = new(); }
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
        {
            var c = CardDataMapper.FromDTO(dto);

            // Liste sprite’ını diskte çöz
            c.characterSprite = CardArtResolver.GetSprite(c, QRDataManager.Instance?.defaultListSprite);

            // Seviye/rol bufflarını uygula (varsa)
            CardLevelSystem.Instance?.ApplyToCard(c);

            myCards.Add(c);
        }


        Debug.Log($"[Inventory] Yüklendi: {myCards.Count} kart (profil: {CurrentProfileId})");
    }

    // İsteğe bağlı yardımcı: ilk kartı "aktif" say
    public CardData GetActiveCard() => (myCards.Count > 0) ? myCards[0] : null;

    public void ResetSkillCooldown() { /* şimdilik stub */ }
    public void OnTurnStart() { /* şimdilik stub */ }
}
