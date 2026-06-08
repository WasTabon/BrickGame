using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;
    public float lifetime = 3f;
    public bool pierce;
    public bool explosive;
    public float explosionRadius = 1.4f;
    public Sprite hitSprite;
    public Color trailColor = new Color(1f, 0.65f, 0.15f, 1f);

    private Vector2 dir;
    private float timer;
    private readonly HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

    private void Awake()
    {
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.18f;
        trail.startWidth = 0.22f;
        trail.endWidth = 0f;
        trail.numCapVertices = 4;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] { new GradientColorKey(trailColor, 0f), new GradientColorKey(trailColor, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0f, 1f) });
        trail.colorGradient = g;
        trail.sortingOrder = 4;
    }

    public void Launch(Vector2 targetPos)
    {
        dir = (targetPos - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
        transform.Rotate(0f, 0f, 360f * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer > lifetime) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;
        if (hitEnemies.Contains(enemy)) return;
        hitEnemies.Add(enemy);

        if (explosive)
        {
            ExplodeAt(transform.position);
        }
        else
        {
            enemy.TakeDamage(damage);
        }

        if (ImpactManager.Instance != null)
        {
            ImpactManager.Instance.RegisterImpact(transform.position, explosive ? 9f : 6f);
        }

        Vfx.Flash(hitSprite, transform.position, new Color(1f, 0.9f, 0.5f, 1f), explosive ? 1.6f : 0.7f, 0.18f, 66);
        Vfx.Sparks(hitSprite, transform.position, new Color(1f, 0.8f, 0.3f, 1f), explosive ? 8 : 5, explosive ? 1.3f : 0.7f);

        if (!pierce)
        {
            Destroy(gameObject);
        }
    }

    private void ExplodeAt(Vector2 center)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            Enemy e = hit.GetComponent<Enemy>();
            if (e != null) e.TakeDamage(damage);
        }
    }
}
