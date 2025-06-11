// SinglePlayerBattleStart.cs – Tek oyunculu test butonu (spawn karakterleri elle baþlatmak için)

using UnityEngine;
using System.Collections.Generic;

public class SinglePlayerBattleStart : MonoBehaviour
{
    public void OnStartSinglePlayerBattle()
    {
        var deck = FindObjectOfType<DeckManagerObject>();
        var battle = FindObjectOfType<BattleManager>();

        if (deck == null || battle == null)
        {
            Debug.LogError("DeckManager veya BattleManager bulunamadý!");
            return;
        }

        deck.PrepareMatchDeck();
        List<CardData> playerCards = deck.currentMatchDeck;

        // Düþman destesi ayný kartlardan oluþur (test için)
        List<CardData> enemyCards = new List<CardData>(playerCards);

        battle.SpawnCharacters(playerCards, enemyCards);
        battle.StartBattle();

        Debug.Log("Tek oyunculu test savaþý baþlatýldý.");
    }
}
