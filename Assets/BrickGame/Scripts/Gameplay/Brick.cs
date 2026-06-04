using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Brick : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D body;

    private SpriteRenderer sr;
    private Color baseColor;
    private float lastImpactTime;

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
    }
}
