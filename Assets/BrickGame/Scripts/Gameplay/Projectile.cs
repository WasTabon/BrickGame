using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;
    public float lifetime = 3f;
    public bool pierce;
    public bool explosive;
    public float explosionRadius = 1.4f;

    private Vector2 dir;
    private float timer;
    private readonly HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

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
