using Unity.Netcode;
using UnityEngine;

public class CardPlayUI : MonoBehaviour
{
    public void OnCardClicked(CardData selectedCard)
    {
        if (!TurnManager.Instance.IsMyTurn(NetworkManager.Singleton.LocalClientId))
        {
            Debug.Log("Sýra sende deðil.");
            return;
        }

        if (FindObjectsOfType<Character>().Length >= 12)
        {
            Debug.Log("Zaten 12 kart oynandý.");
            return;
        }

        FindObjectOfType<BattleManager>()?.PlayCardServerRpc(selectedCard.id);
    }
}
