using UnityEngine;
using UnityEngine.UI;

public class CardUI_Battle : MonoBehaviour
{
    public Image frontImage;
    public Image backImage;

    private CardData data;

    public void SetCardData(CardData d, bool faceUp)
    {
        data = d;
        if (frontImage) frontImage.sprite = d.characterSprite;
        SetFace(faceUp);
    }

    public void SetFace(bool faceUp)
    {
        if (frontImage) frontImage.enabled = faceUp;
        if (backImage) backImage.enabled = !faceUp;
    }
}
