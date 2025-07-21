using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardCollectionManager : MonoBehaviour
{
    public GameObject cardPrefab; // CardUI prefab
    public Transform scrollViewContent; // ScrollView > Viewport > Content

    void Start()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogError("PlayerInventory bulunamadı!");
            return;
        }

        foreach (CardData card in inventory.myCards)
        {
            GameObject cardGO = Instantiate(cardPrefab, scrollViewContent);
            cardGO.transform.localScale = Vector3.one;

            CardUIController controller = cardGO.GetComponent<CardUIController>();
            controller?.DisplayCard(card);

        }
    }
}
