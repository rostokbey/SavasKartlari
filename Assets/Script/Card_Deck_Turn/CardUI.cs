using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CardUI : MonoBehaviour
{
    public Image portraitImage;
    public Image healthBarFill;
    public Text levelText;
    public Image[] skillIcons;

    private int maxHealth = 100;

    public void SetData(Sprite portrait, int level, Sprite[] skills, int hp)
    {
        portraitImage.sprite = portrait;
        levelText.text = "Lvl " + level;
        maxHealth = hp;

        for (int i = 0; i < skillIcons.Length; i++)
        {
            skillIcons[i].sprite = skills[i];
            skillIcons[i].gameObject.SetActive(true);
        }

        UpdateHealth(hp);
    }

    public void UpdateHealth(int currentHealth)
    {
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }
}
