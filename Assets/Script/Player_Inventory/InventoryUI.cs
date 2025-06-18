
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Referansları")]
    public GameObject cardUIPrefab;      // 💡 CardUI prefabını buraya sürükle
    public Transform contentArea;        // 💡 ScrollView > Viewport > Content objesini buraya sürükle

    private void OnEnable()
    {
        RefreshInventory();
    }

    public void RefreshInventory()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("❌ PlayerInventory.Instance bulunamadı!");
            return;
        }

        // 🧹 Önce içerikleri temizle
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // 🔁 Her bir kart için prefab oluştur
        foreach (var card in PlayerInventory.Instance.myCards)
        {
            GameObject cardUIObj = Instantiate(cardUIPrefab, contentArea);
            CardUI cardUI = cardUIObj.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.SetCardData(card);
            }
            else
            {
                Debug.LogWarning("CardUI componenti prefab içinde eksik!");
            }
        }
    }
}
