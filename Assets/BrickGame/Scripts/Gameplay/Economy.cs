using UnityEngine;

public static class Economy
{
    private const string BankKey = "BrickBank";

    public static int Bank
    {
        get { return PlayerPrefs.GetInt(BankKey, 0); }
    }

    public static void Add(int amount)
    {
        if (amount <= 0) return;
        PlayerPrefs.SetInt(BankKey, Bank + amount);
        PlayerPrefs.Save();
    }

    public static bool TrySpend(int amount)
    {
        if (Bank < amount) return false;
        PlayerPrefs.SetInt(BankKey, Bank - amount);
        PlayerPrefs.Save();
        return true;
    }
}
