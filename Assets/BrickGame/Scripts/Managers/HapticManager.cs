using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance;

    private const string HapticKey = "HapticEnabled";
    private bool hapticEnabled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        hapticEnabled = PlayerPrefs.GetInt(HapticKey, 1) == 1;
    }

    public void Light()
    {
        Vibrate();
    }

    public void Medium()
    {
        Vibrate();
    }

    public void Heavy()
    {
        Vibrate();
    }

    private void Vibrate()
    {
        if (!hapticEnabled) return;

#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public void SetEnabled(bool enabled)
    {
        hapticEnabled = enabled;
        PlayerPrefs.SetInt(HapticKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsEnabled()
    {
        return hapticEnabled;
    }
}
