// MatchReadyButton.cs (g�ncellenmi� - kart adlar� loga yaz�l�yor)

using UnityEngine;
using System.Linq;
using Unity.Collections;

public class MatchReadyButton : MonoBehaviour
{
    public void OnReadyClicked()
    {
        var deck = FindObjectOfType<DeckManagerObject>();
        if (deck == null)
        {
            Debug.LogError("DeckManagerObject bulunamad�.");
            return;
        }

        deck.PrepareMatchDeck();

        // Kartlar� debugla
        foreach (var card in deck.currentMatchDeck)
        {
            Debug.Log($"Se�ili kart: {card.cardName}");
        }

        FixedString128Bytes[] cardIdArray = deck.currentMatchDeck
            .Select(card => (FixedString128Bytes)card.id)
            .ToArray();

        var battleManager = FindObjectOfType<BattleManager>();
        if (battleManager != null)
        {
            battleManager.SubmitDeckServerRpc(cardIdArray);
        }
        else
        {
            Debug.LogError("BattleManager bulunamad�.");
        }
    }
}
