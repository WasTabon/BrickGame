using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleController : MonoBehaviour
{
    public Camera cam;
    public Transform muzzle;
    public Transform cannon;
    public GameObject enemyPrefab;
    public Projectile projectilePrefab;
    public BattleHUD hud;
    public BattleResultPopup popup;
    public BattleBuffPicker buffPicker;
    public MuzzleFlash muzzleFlash;

    public int debugDefaultAmmo = 20;
    public int enemyCount = 8;
    public int enemyHp = 2;
    public float enemySpeed = 0.7f;
    public float baseLineX = -2.4f;
    public float spawnStartX = 5f;
    public float spawnStepX = 1.4f;
    public float enemyY = -1.7f;
    public float fireInterval = 0.4f;
    public int projectileDamage = 1;

    private int ammo;
    private int aliveEnemies;
    private bool battleOver;
    private readonly List<Enemy> enemies = new List<Enemy>();

    private int damageMul = 1;
    private bool twinShot;
    private bool pierce;
    private bool explosive;

    private void Start()
    {
        if (cam == null) cam = Camera.main;

        BattleBuffs.Reset();

        if (buffPicker != null)
        {
            buffPicker.Show(OnBuffChosen);
        }
        else
        {
            BeginBattle();
        }
    }

    private void OnBuffChosen(BuffType type)
    {
        BeginBattle();
    }

    private void BeginBattle()
    {
        ammo = GameSession.CollectedBricks > 0 ? GameSession.CollectedBricks : debugDefaultAmmo;

        if (BattleBuffs.HasSelection)
        {
            switch (BattleBuffs.Selected)
            {
                case BuffType.DoubleDamage: damageMul = 2; break;
                case BuffType.RapidFire: fireInterval *= 0.5f; break;
                case BuffType.ExtraAmmo: ammo = Mathf.RoundToInt(ammo * 1.6f); break;
                case BuffType.TwinShot: twinShot = true; break;
                case BuffType.Pierce: pierce = true; break;
                case BuffType.BigShells: explosive = true; break;
                case BuffType.SlowEnemies: enemySpeed *= 0.5f; break;
            }
        }

        SpawnWave();
        hud.SetAmmo(ammo);
        hud.SetEnemies(aliveEnemies);

        StartCoroutine(FireLoop());
    }

    private void SpawnWave()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 pos = new Vector3(spawnStartX + i * spawnStepX, enemyY, 0f);
            Enemy enemy = Instantiate(enemyPrefab, pos, Quaternion.identity).GetComponent<Enemy>();
            enemy.hp = enemyHp;
            enemy.speed = enemySpeed;
            enemy.baseLineX = baseLineX;
            enemy.OnDied += OnEnemyDied;
            enemy.OnReachedBase += OnBaseHit;
            enemies.Add(enemy);
        }
        aliveEnemies = enemyCount;
    }

    private IEnumerator FireLoop()
    {
        while (!battleOver)
        {
            yield return new WaitForSeconds(fireInterval);
            if (battleOver) yield break;
            if (ammo <= 0) continue;

            List<Enemy> targets = GetVisibleTargets(twinShot ? 2 : 1);
            if (targets.Count == 0) continue;

            ammo--;
            hud.SetAmmo(ammo);

            foreach (Enemy t in targets)
            {
                Fire(t);
            }
        }
    }

    private List<Enemy> GetVisibleTargets(int count)
    {
        List<Enemy> visible = new List<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            if (!IsOnScreen(enemy.transform.position)) continue;
            visible.Add(enemy);
        }
        visible.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        if (visible.Count > count) visible.RemoveRange(count, visible.Count - count);
        return visible;
    }

    private bool IsOnScreen(Vector3 worldPos)
    {
        Vector3 vp = cam.WorldToViewportPoint(worldPos);
        return vp.z > 0f && vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f;
    }

    private void Fire(Enemy target)
    {
        Projectile projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        projectile.damage = projectileDamage * damageMul;
        projectile.pierce = pierce;
        projectile.explosive = explosive;
        projectile.Launch(target.transform.position);

        if (cannon != null)
        {
            cannon.DOKill();
            cannon.DOPunchPosition(new Vector3(-0.2f, 0f, 0f), 0.2f, 5, 1f);
        }

        if (muzzleFlash != null) muzzleFlash.Flash();
    }

    private void OnEnemyDied(Enemy enemy)
    {
        enemies.Remove(enemy);
        aliveEnemies--;
        hud.SetEnemies(aliveEnemies);

        if (aliveEnemies <= 0 && !battleOver)
        {
            EndBattle(true);
        }
    }

    private void OnBaseHit(Enemy enemy)
    {
        if (battleOver) return;
        EndBattle(false);
    }

    private void EndBattle(bool victory)
    {
        battleOver = true;
        popup.Show(victory);
    }
}
