// SinglePlayerBattleStart.cs � Tek oyunculu test butonu (spawn karakterleri elle ba�latmak i�in)

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
            Debug.LogError("DeckManager veya BattleManager bulunamad�!");
            return;
        }

        deck.PrepareMatchDeck();
        List<CardData> playerCards = deck.currentMatchDeck;

        // D��man destesi ayn� kartlardan olu�ur (test i�in)
        List<CardData> enemyCards = new List<CardData>(playerCards);

        battle.SpawnCharacters(playerCards, enemyCards);
        battle.StartBattle();

        Debug.Log("Tek oyunculu test sava�� ba�lat�ld�.");
    }
}
