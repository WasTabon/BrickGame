using UnityEngine;

public static class SaveSystem
{
    private const string UnlockedKey = "UnlockedLevel";
    private const string StarsKeyPrefix = "Stars_";

    public static int UnlockedLevel
    {
        get { return Mathf.Max(1, PlayerPrefs.GetInt(UnlockedKey, 1)); }
    }

    public static int GetStars(int level)
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(StarsKeyPrefix + level, 0), 0, 3);
    }

    public static void SetStars(int level, int stars)
    {
        stars = Mathf.Clamp(stars, 0, 3);
        if (stars > GetStars(level))
        {
            PlayerPrefs.SetInt(StarsKeyPrefix + level, stars);
        }
        PlayerPrefs.Save();
    }

    public static void UnlockLevel(int level)
    {
        if (level > PlayerPrefs.GetInt(UnlockedKey, 1))
        {
            PlayerPrefs.SetInt(UnlockedKey, level);
            PlayerPrefs.Save();
        }
    }

    public static void CompleteLevel(int level, int stars)
    {
        SetStars(level, stars);
        UnlockLevel(level + 1);
    }
}
