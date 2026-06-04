using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CollectionPit : MonoBehaviour
{
    public int totalBricks;

    public event Action<int> OnCountChanged;
    public event Action<Vector2> OnBrickEntered;

    private readonly HashSet<Brick> counted = new HashSet<Brick>();
    private int collected;

    public void Collect(Brick brick)
    {
        if (brick == null) return;
        if (!counted.Add(brick)) return;

        collected++;
        Vector2 pos = brick.transform.position;

        OnCountChanged?.Invoke(collected);
        OnBrickEntered?.Invoke(pos);

        if (ComboManager.Instance != null) ComboManager.Instance.RegisterCollect(pos);

        Consume(brick);
    }

    private void Consume(Brick brick)
    {
        if (brick.body != null) brick.body.simulated = false;

        Collider2D col = brick.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        SpriteRenderer sr = brick.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.DOKill();
            sr.DOFade(0f, 0.28f);
        }

        brick.transform.DOKill();
        brick.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);

        Destroy(brick.gameObject, 0.32f);
    }

    public int CurrentCount()
    {
        return collected;
    }
}
