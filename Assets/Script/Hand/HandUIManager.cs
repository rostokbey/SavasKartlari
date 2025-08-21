using System.Collections.Generic;
using UnityEngine;

public class HandUIManager : MonoBehaviour
{
    [Header("UI Refs")]
    public Transform handContent;           // Horizontal Layout -> Content
    public GameObject cardUIPrefab;         // SAVA� kart prefab� (CardUI_Battle scriptli)

    [Header("Settings")]
    public int handSize = 5;                // elde a��k g�sterilecek kart say�s�

    // ---- D��ar�dan �a��r: eldeki kartlar� �ret ----
    public void BuildHandFromDeck(List<CardData> deck)
    {
        if (handContent == null || cardUIPrefab == null || deck == null) return;

        // Temizle
        for (int i = handContent.childCount - 1; i >= 0; i--)
            Destroy(handContent.GetChild(i).gameObject);

        // Basit kar��t�rma
        var list = new List<CardData>(deck);
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }

        // ilk handSize adet a��k, sonraki 3 adet kapal� �rne�i
        int openCount = Mathf.Min(handSize, list.Count);
        int total = Mathf.Min(list.Count, handSize + 3);

        for (int i = 0; i < total; i++)
        {
            var go = Instantiate(cardUIPrefab, handContent);
            go.transform.localScale = Vector3.one;

            var ui = go.GetComponent<CardUI_Battle>(); // << SAVA� scripti
            if (ui != null)
            {
                bool faceUp = i < openCount;
                ui.SetCardData(list[i], faceUp);
            }
        }
    }

    // ---- BattleManager kolay �a��rabilsin diye sarg� ----
    public void Init(List<CardData> deck, GameObject overrideCardPrefab = null)
    {
        if (overrideCardPrefab != null)
            cardUIPrefab = overrideCardPrefab;

        BuildHandFromDeck(deck);
    }
}
