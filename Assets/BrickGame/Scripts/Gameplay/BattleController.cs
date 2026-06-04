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

    public int debugDefaultAmmo = 20;
    public int enemyCount = 8;
    public int enemyHp = 2;
    public float enemySpeed = 0.7f;
    public float baseLineX = -2.4f;
    public float spawnStartX = 5f;
    public float spawnStepX = 1.4f;
    public float enemyY = -1.7f;
    public float fireInterval = 0.4f;

    private int ammo;
    private int aliveEnemies;
    private bool battleOver;
    private readonly List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        if (cam == null) cam = Camera.main;

        ammo = GameSession.CollectedBricks > 0 ? GameSession.CollectedBricks : debugDefaultAmmo;

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

            Enemy target = NearestVisibleToBase();
            if (target == null) continue;

            Fire(target);
        }
    }

    private Enemy NearestVisibleToBase()
    {
        Enemy nearest = null;
        float best = float.MaxValue;
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            if (!IsOnScreen(enemy.transform.position)) continue;
            if (enemy.transform.position.x < best)
            {
                best = enemy.transform.position.x;
                nearest = enemy;
            }
        }
        return nearest;
    }

    private bool IsOnScreen(Vector3 worldPos)
    {
        Vector3 vp = cam.WorldToViewportPoint(worldPos);
        return vp.z > 0f && vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f;
    }

    private void Fire(Enemy target)
    {
        ammo--;
        hud.SetAmmo(ammo);

        Projectile projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        projectile.Launch(target.transform.position);

        if (cannon != null)
        {
            cannon.DOKill();
            cannon.DOPunchPosition(new Vector3(-0.2f, 0f, 0f), 0.2f, 5, 1f);
        }
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