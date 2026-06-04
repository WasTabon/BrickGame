using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Physics2D.velocityIterations = 16;
        Physics2D.positionIterations = 8;
    }
}
