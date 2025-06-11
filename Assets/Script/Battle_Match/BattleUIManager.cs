using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleUIManager : MonoBehaviour
{
    public GameObject lobbyUI;
    public GameObject deckUI;
    public GameObject cardCollectionUI;

    public Button attackButton;
    public Button skillButton;

    public Sprite testPlayerSprite; // Inspector üzerinden atayabilirsin
    public Sprite testEnemySprite;

    public void StartBattle()
    {
        // UI'leri kapat
        if (lobbyUI != null) lobbyUI.SetActive(false);
        if (deckUI != null) deckUI.SetActive(false);
        if (cardCollectionUI != null) cardCollectionUI.SetActive(false);

        // Savaş butonlarını aç
        if (attackButton != null) attackButton.gameObject.SetActive(true);
        if (skillButton != null) skillButton.gameObject.SetActive(true);

        // Test için örnek kartlar
        List<CardData> playerDeck = new List<CardData>
        {
            new CardData(
                id: "PLAYER001",
                cardName: "Savaşçı",
                baseHP: 100,
                baseDamage: 20,
                ability: "Fireball",
                passive: "Savunma",
                rarity: "Yaygın",
                level: 1,
                xp: 0,
                skillCooldownMax: 3,
                characterSprite: testPlayerSprite // Opsiyonel, null olabilir
            )
        };

        List<CardData> enemyDeck = new List<CardData>
        {
            new CardData(
                id: "ENEMY001",
                cardName: "Canavar",
                baseHP: 90,
                baseDamage: 25,
                ability: "Poison",
                passive: "Zehir",
                rarity: "Efsanevi",
                level: 1,
                xp: 0,
                skillCooldownMax: 2,
                characterSprite: testEnemySprite
            )
        };

        // Karakterleri sahneye yerleştir
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.SpawnCharacters(playerDeck, enemyDeck);
        }
        else
        {
            Debug.LogError("❌ BattleManager.Instance bulunamadı!");
        }
    }
}
