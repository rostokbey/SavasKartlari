using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSceneLoader : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayedSpawn());
    }

    IEnumerator DelayedSpawn()
    {
        // Sahne otursun diye biraz bekle
        yield return new WaitForSeconds(0.5f);

        if (StartBattleManager.Instance != null && StartBattleManager.Instance.selectedMatchCards.Count > 0)
        {
            BattleManager.Instance.SpawnPlayerCards(StartBattleManager.Instance.selectedMatchCards);
        }
        else
        {
            Debug.LogError("❌ Savaşa ait kartlar bulunamadı.");
        }
    }
}
