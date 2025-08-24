
using UnityEngine;

public class ForceOrientation : MonoBehaviour
{
    [Tooltip("Sahnede yatay m� olsun?")]
    public bool landscape;

    void OnEnable()
    {
        Screen.autorotateToPortrait = !landscape;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = landscape;
        Screen.autorotateToLandscapeRight = landscape;

        Screen.orientation = landscape ? ScreenOrientation.LandscapeLeft
                                       : ScreenOrientation.Portrait;
    }

    void OnDisable()
    {
        // Sahneden ��karken geri AutoRotation'a d�n
        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}
