using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class CardCollectionManager : MonoBehaviour
{
    public GameObject cardSlotPrefab;
    public Transform contentParent;

    public Dropdown filterDropdown;
    public Dropdown sortDropdown;

    private List<CardData> allCards = new();

    public void Show()
    {
        gameObject.SetActive(true);
        allCards = FindObjectOfType<PlayerInventory>().myCards;

        // Filtreyi tamamen kaldırıp doğrudan göster:
        RefreshCollection(allCards);
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // CardCollectionManager.cs içinde:
    
    public Transform cardGridParent; // CardGrid'e bağla

    public void ShowCards(List<CardData> cards)
    {
        foreach (Transform child in cardGridParent)
            Destroy(child.gameObject);

        foreach (CardData card in cards)
        {
            GameObject slot = Instantiate(cardSlotPrefab, cardGridParent);
            slot.GetComponent<CardSlotUI>().SetCardInfo(card);
        }
    }
   

 
    

    void RefreshCollection(List<CardData> cards)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (CardData card in cards)
        {
            GameObject slot = Instantiate(cardSlotPrefab, contentParent);
            var ui = slot.GetComponent<CardSlotUI>();
            ui.SetCardInfo(card);
            ui.OnCardClicked = () => ShowCardDetail(card); // tıklama ile detay
        }
    }

    public GameObject detailPanel;
    public Text detailText;

    void ShowCardDetail(CardData card)
    {
        detailText.text = $"{card.cardName}\nHP: {card.baseHP}\nDMG: {card.baseDamage}\nSeviye: {card.level}\nXP: {card.xp}";
        detailPanel.SetActive(true);
    }

    public void AddToDeck(CardData card)
    {
        FindObjectOfType<DeckManagerObject>()?.AddToDeck(card);
        Debug.Log(card.cardName); // ✅
    }
}
