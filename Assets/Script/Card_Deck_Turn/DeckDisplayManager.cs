using System.Collections.Generic;
using UnityEngine;

public class DeckDisplayManager : MonoBehaviour
{
    public GameObject cardSlotPrefab;
    public Transform contentParent;

    public void RefreshDeckDisplay(List<CardData> deck)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (CardData card in deck)
        {
            GameObject slot = Instantiate(cardSlotPrefab, contentParent);
            slot.GetComponent<CardSlotUI>().SetCardInfo(card);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        var deck = FindObjectOfType<DeckManagerObject>().currentMatchDeck;
        if (deck != null)
            RefreshDeckDisplay(deck);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
