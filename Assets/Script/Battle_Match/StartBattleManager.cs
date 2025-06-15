// StartBattleManager.cs
// Seçilen desteyi kullanarak savaşı başlatır (tek oyunculu demo için uygundur)

using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class StartBattleManager : MonoBehaviour
{
    private DeckManagerObject deckManager;
    private BattleManager battleManager;

    void Start()
    {
        deckManager = FindObjectOfType<DeckManagerObject>();
        battleManager = FindObjectOfType<BattleManager>();

        if (deckManager == null || battleManager == null)
        {
            Debug.LogError("DeckManager veya BattleManager bulunamadı!");
            return;
        }
    }

    public void StartBattleFromDeck()
    {
        if (deckManager.currentMatchDeck == null || deckManager.currentMatchDeck.Count == 0)
        {
            Debug.LogWarning("Seçili maç destesi boş. Savaşa geçilemiyor.");
            return;
        }

        List<CardData> playerCards = new List<CardData>(deckManager.currentMatchDeck);
        List<CardData> enemyCards = new List<CardData>(deckManager.currentMatchDeck); // demo için kendi destesini kopyalıyoruz

        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Savaş başlatmak için sunucu olman gerekiyor.");
            return;
        }

        battleManager.SpawnCharacters(playerCards, enemyCards);
        battleManager.StartBattle();

        Debug.Log("✅ Savaş başlatıldı (tek oyunculu demo). Karakterler sahaya dizildi.");
    }
}
