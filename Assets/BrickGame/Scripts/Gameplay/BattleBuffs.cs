using System.Collections.Generic;

public enum BuffType
{
    DoubleDamage,
    RapidFire,
    ExtraAmmo,
    TwinShot,
    Pierce,
    BigShells,
    SlowEnemies
}

public static class BattleBuffs
{
    public static BuffType Selected;
    public static bool HasSelection;

    public struct BuffInfo
    {
        public BuffType type;
        public string name;
        public string desc;
    }

    public static readonly BuffInfo[] All =
    {
        new BuffInfo { type = BuffType.DoubleDamage, name = "DOUBLE DAMAGE", desc = "x2 damage per shot" },
        new BuffInfo { type = BuffType.RapidFire, name = "RAPID FIRE", desc = "Fire twice as fast" },
        new BuffInfo { type = BuffType.ExtraAmmo, name = "EXTRA AMMO", desc = "+60% attacks" },
        new BuffInfo { type = BuffType.TwinShot, name = "TWIN SHOT", desc = "Hit two targets at once" },
        new BuffInfo { type = BuffType.Pierce, name = "PIERCE", desc = "Shots pass through enemies" },
        new BuffInfo { type = BuffType.BigShells, name = "BIG SHELLS", desc = "Explosive area damage" },
        new BuffInfo { type = BuffType.SlowEnemies, name = "SLOW FIELD", desc = "Enemies move at half speed" },
    };

    public static void Reset()
    {
        HasSelection = false;
    }

    public static void Choose(BuffType type)
    {
        Selected = type;
        HasSelection = true;
    }

    public static List<BuffInfo> PickThree()
    {
        List<BuffInfo> pool = new List<BuffInfo>(All);
        List<BuffInfo> result = new List<BuffInfo>();
        System.Random r = new System.Random();
        int take = UnityEngine.Mathf.Min(3, pool.Count);
        for (int i = 0; i < take; i++)
        {
            int idx = r.Next(pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return result;
    }

    public static string NameFor(BuffType type)
    {
        foreach (BuffInfo b in All)
        {
            if (b.type == type) return b.name;
        }
        return type.ToString();
    }
}
