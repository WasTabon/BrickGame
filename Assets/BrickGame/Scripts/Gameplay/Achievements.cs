using System;
using UnityEngine;

public static class Achievements
{
    public struct Def
    {
        public string key;
        public string name;
        public string desc;
    }

    public static readonly Def[] All =
    {
        new Def { key = "first_clear", name = "First Demolition", desc = "Complete level 1" },
        new Def { key = "reach_10", name = "Halfway There", desc = "Unlock level 10" },
        new Def { key = "three_star_5", name = "Perfectionist", desc = "Earn 3 stars on 5 levels" },
        new Def { key = "one_pull", name = "One Pull Wonder", desc = "3 stars with a single throw" },
        new Def { key = "combo_5", name = "Combo Master", desc = "Reach combo x5" },
        new Def { key = "daily_done", name = "Daily Grinder", desc = "Complete a daily challenge" },
    };

    public static event Action<string> OnUnlocked;

    public static bool IsUnlocked(string key)
    {
        return PlayerPrefs.GetInt("Ach_" + key, 0) == 1;
    }

    public static void Unlock(string key)
    {
        if (IsUnlocked(key)) return;
        PlayerPrefs.SetInt("Ach_" + key, 1);
        PlayerPrefs.Save();
        OnUnlocked?.Invoke(NameFor(key));
    }

    public static string NameFor(string key)
    {
        foreach (Def d in All)
        {
            if (d.key == key) return d.name;
        }
        return key;
    }

    public static void CheckThreeStarFive()
    {
        int c = 0;
        for (int l = 1; l <= 30; l++)
        {
            if (SaveSystem.GetStars(l) >= 3) c++;
        }
        if (c >= 5) Unlock("three_star_5");
    }
}
