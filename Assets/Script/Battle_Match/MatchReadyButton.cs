// MatchReadyButton.cs (güncellenmiþ - kart adlarý loga yazýlýyor)

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
            Debug.LogError("DeckManagerObject bulunamadý.");
            return;
        }

        deck.PrepareMatchDeck();

        // Kartlarý debugla
        foreach (var card in deck.currentMatchDeck)
        {
            Debug.Log($"Seçili kart: {card.cardName}");
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
            Debug.LogError("BattleManager bulunamadý.");
        }
    }
}
