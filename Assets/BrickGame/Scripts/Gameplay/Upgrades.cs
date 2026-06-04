using UnityEngine;

public static class Upgrades
{
    public const string Force = "Upg_Force";
    public const string Length = "Upg_Length";
    public const string Speed = "Upg_Speed";
    public const string Magnet = "Upg_Magnet";

    public const int MaxLevel = 5;

    public static int GetLevel(string key)
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(key, 0), 0, MaxLevel);
    }

    public static int CostFor(string key)
    {
        int level = GetLevel(key);
        if (level >= MaxLevel) return -1;
        return 50 + level * 50;
    }

    public static bool Buy(string key)
    {
        if (GetLevel(key) >= MaxLevel) return false;

        int cost = CostFor(key);
        if (!Economy.TrySpend(cost)) return false;

        PlayerPrefs.SetInt(key, GetLevel(key) + 1);
        PlayerPrefs.Save();
        return true;
    }

    public static float ForceBonus()
    {
        return GetLevel(Force) * 8f;
    }

    public static float DurationBonus()
    {
        return GetLevel(Length) * 0.15f;
    }

    public static float RampReduction()
    {
        return GetLevel(Speed) * 0.06f;
    }

    public static float MagnetBonus()
    {
        return GetLevel(Magnet) * 0.25f;
    }
}
