using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public float duration = 0.25f;

    private Vector3 basePos;
    private Tween shakeTween;

    private void Awake()
    {
        basePos = transform.localPosition;
    }

    public void Shake(float strength)
    {
        shakeTween?.Kill();
        transform.localPosition = basePos;
        shakeTween = transform.DOShakePosition(duration, strength, 18, 90f, false, true)
            .OnComplete(() => transform.localPosition = basePos);
    }
}
