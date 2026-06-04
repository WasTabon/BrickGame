using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Brick : MonoBehaviour
{
    public enum BrickMaterial { Normal, Ice, Stone, Wood, Bomb }

    [HideInInspector] public Rigidbody2D body;
    public BrickMaterial material = BrickMaterial.Normal;

    public float explosionRadius = 2.2f;
    public float explosionForce = 9f;

    private SpriteRenderer sr;
    private Color baseColor;
    private float lastImpactTime;
    private bool exploded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
    }

    public void Highlight()
    {
        sr.DOKill();
        sr.color = baseColor;
        sr.DOColor(Color.white, 0.12f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => sr.color = baseColor);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float speed = collision.relativeVelocity.magnitude;
        if (speed < 1.5f) return;
        if (Time.time - lastImpactTime < 0.08f) return;
        lastImpactTime = Time.time;

        Vector2 point = collision.contactCount > 0
            ? collision.GetContact(0).point
            : (Vector2)transform.position;

        if (ImpactManager.Instance != null)
        {
            ImpactManager.Instance.RegisterImpact(point, speed);
        }

        if (material == BrickMaterial.Bomb && !exploded && speed >= 2.5f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        exploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            Rigidbody2D rb = hit.attachedRigidbody;
            if (rb == null || rb == body) continue;

            Vector2 dir = rb.worldCenterOfMass - (Vector2)transform.position;
            float dist = Mathf.Max(0.2f, dir.magnitude);
            rb.AddForce(dir.normalized * explosionForce * rb.mass / dist, ForceMode2D.Impulse);
        }

        sr.DOKill();
        sr.color = Color.white;
        sr.DOColor(baseColor, 0.25f);

        if (ImpactManager.Instance != null)
        {
            ImpactManager.Instance.RegisterImpact(transform.position, 12f);
        }
    }
}
