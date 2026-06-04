using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Brick : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D body;

    private SpriteRenderer sr;
    private Color baseColor;

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
}
