using UnityEngine;
using DG.Tweening;

public class AttachPoint : MonoBehaviour
{
    public Rigidbody2D targetBody;
    public Vector2 offset;

    private Vector3 baseScale;
    private Tween pulseTween;
    private bool selected;

    private void Awake()
    {
        baseScale = transform.localScale;
        StartPulse();
    }

    private void LateUpdate()
    {
        if (targetBody != null)
        {
            transform.position = (Vector3)(targetBody.position + offset);
        }
    }

    private void StartPulse()
    {
        pulseTween = transform.DOScale(baseScale * 1.25f, 0.6f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void Select()
    {
        if (selected) return;
        selected = true;
        pulseTween?.Kill();
        transform.localScale = baseScale;
        transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 6, 1f);
    }

    public void ResetState()
    {
        selected = false;
        transform.DOKill();
        transform.localScale = baseScale;
        StartPulse();
    }
}
