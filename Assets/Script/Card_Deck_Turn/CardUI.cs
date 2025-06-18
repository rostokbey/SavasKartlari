using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CardUI : MonoBehaviour
{
    public Image characterImage;
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text strText;

    // 👇 Bu fonksiyon InventoryUI ya da InventoryPanel tarafından çağrılacak
    public void SetCardData(CardData card)
    {
        characterImage.sprite = card.characterSprite;
        nameText.text = card.cardName.Replace("_", " ");
        hpText.text = "HP: " + card.baseHP;
        strText.text = "STR: " + card.baseDamage;
    }
}