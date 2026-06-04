using System;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public int hp = 2;
    public float speed = 0.7f;
    public float baseLineX = -2.4f;

    public Action<Enemy> OnDied;
    public Action<Enemy> OnReachedBase;

    private bool dead;
    private SpriteRenderer sr;
    private Color baseColor;
    private Vector3 baseScale;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (dead) return;

        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= baseLineX)
        {
            dead = true;
            OnReachedBase?.Invoke(this);
        }
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;

        hp -= amount;

        sr.DOKill();
        sr.color = Color.white;
        sr.DOColor(baseColor, 0.15f);

        transform.DOKill();
        transform.localScale = baseScale;
        transform.DOPunchScale(baseScale * 0.2f, 0.2f, 6, 1f);

        if (hp <= 0) Die();
    }

    private void Die()
    {
        dead = true;
        OnDied?.Invoke(this);

        transform.DOKill();
        sr.DOKill();
        transform.DOScale(0f, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}
