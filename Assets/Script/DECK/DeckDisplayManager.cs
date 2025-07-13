using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckDisplayManager : MonoBehaviour
{
    [Header("Sahnedeki kart panel template'i")]
    public GameObject cardPanelTemplate; // Sahnedeki, SetActive(false) olan panel

    [Header("ScrollView -> Content alan�")]
    public Transform contentParent;

    public void RefreshDeckDisplay(List<CardData> deck)
    {
        // Eski kartlar� temizle
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Her kart i�in yeni panel olu�tur
        foreach (CardData card in deck)
        {
            GameObject panel = Instantiate(cardPanelTemplate, contentParent);
            panel.SetActive(true);

            // Text ve g�rsel atamalar�
            panel.transform.Find("Image").GetComponent<Image>().sprite = card.characterSprite;
            panel.transform.Find("nameText").GetComponent<TextMeshProUGUI>().text = card.cardName;
            panel.transform.Find("levelText").GetComponent<TextMeshProUGUI>().text = "Seviye: " + card.level;
            panel.transform.Find("xpText").GetComponent<TextMeshProUGUI>().text = "XP: " + card.xp + "/100";
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        var deck = FindObjectOfType<DeckManagerObject>().fullDeck;
        if (deck != null)
            RefreshDeckDisplay(deck);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
