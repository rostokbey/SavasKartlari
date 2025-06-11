using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void GoToBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }
}