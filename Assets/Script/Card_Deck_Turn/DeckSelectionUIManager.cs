// DeckSelectionUIManager.cs
// 25'lik kaydedilmiş desteleri ScrollView'da gösterir, seçim yapılınca savaşa geçer

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckSelectionUIManager : MonoBehaviour
{
    public GameObject deckButtonPrefab; // Her deste için bir UI butonu (CardSlotUI ya da özel prefab)
    public Transform contentParent;     // ScrollView > Viewport > Content objesi

    private DeckManagerObject deckManager;
    private SceneUIController sceneUI;

    void Start()
    {
        deckManager = FindObjectOfType<DeckManagerObject>();
        sceneUI = FindObjectOfType<SceneUIController>();

        if (deckManager == null || sceneUI == null)
        {
            Debug.LogError("DeckManagerObject veya SceneUIController bulunamadı!");
            return;
        }

        DisplayDeckOptions();
    }

    void DisplayDeckOptions()
    {
        if (deckManager.fullDeck.Count == 0)
        {
            Debug.LogWarning("❌ QR'dan hiç kart eklenmemiş!");
            return;
        }

        for (int i = 0; i < deckManager.fullDeck.Count; i++)
        {
            CardData card = deckManager.fullDeck[i];

            GameObject buttonObj = Instantiate(deckButtonPrefab, contentParent);
            buttonObj.transform.localScale = Vector3.one;

            CardSlotUI ui = buttonObj.GetComponent<CardSlotUI>();
            if (ui != null)
                ui.SetCardInfo(card);
        }
    }
}
