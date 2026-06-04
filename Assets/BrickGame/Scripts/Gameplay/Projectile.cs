using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;
    public float lifetime = 3f;

    private Vector2 dir;
    private float timer;

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

        enemy.TakeDamage(damage);

        if (ImpactManager.Instance != null)
        {
            ImpactManager.Instance.RegisterImpact(transform.position, 6f);
        }

        Destroy(gameObject);
    }
}
