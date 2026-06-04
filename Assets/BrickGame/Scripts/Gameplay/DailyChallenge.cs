using System;
using UnityEngine;

public static class DailyChallenge
{
    private const string DoneKey = "DailyDone";
    private const string StarsKey = "DailyStars";

    public static string TodayString
    {
        get { return DateTime.Now.ToString("yyyyMMdd"); }
    }

    public static int TodaySeed()
    {
        DateTime n = DateTime.Now;
        return n.Year * 10000 + n.Month * 100 + n.Day;
    }

    public static bool IsTodayDone()
    {
        return PlayerPrefs.GetString(DoneKey, "") == TodayString;
    }

    public static int GetTodayStars()
    {
        return IsTodayDone() ? PlayerPrefs.GetInt(StarsKey, 0) : 0;
    }

    public static void MarkDone(int stars)
    {
        string t = TodayString;
        int prev = PlayerPrefs.GetString(DoneKey, "") == t ? PlayerPrefs.GetInt(StarsKey, 0) : 0;
        PlayerPrefs.SetString(DoneKey, t);
        PlayerPrefs.SetInt(StarsKey, Mathf.Max(prev, stars));
        PlayerPrefs.Save();
    }

    public static LevelDef Generate(int seed)
    {
        System.Random r = new System.Random(seed);
        LevelDef d = new LevelDef();

        d.rows = r.Next(18, 27);
        d.columns = r.Next(2, 4);
        d.pitCount = r.Next(0, 2) == 0 ? 1 : 2;

        if (r.Next(0, 2) == 0) d.stoneEvery = r.Next(5, 8);
        if (r.Next(0, 2) == 0) d.woodEvery = r.Next(5, 9);
        if (r.Next(0, 3) == 0) d.bombEvery = r.Next(9, 13);
        if (r.Next(0, 3) == 0) d.iceRows = r.Next(2, 5);
        if (r.Next(0, 3) == 0) d.brittleEvery = r.Next(7, 11);
        if (r.Next(0, 3) == 0) d.stickyEvery = r.Next(5, 9);

        if (r.Next(0, 2) == 0)
        {
            d.pitMoveAmplitude = 1.0f + (float)r.NextDouble();
            d.pitMoveSpeed = 1.0f + (float)r.NextDouble();
        }

        if (r.Next(0, 2) == 0) d.maxThrows = r.Next(4, 8);

        return d;
    }
}
