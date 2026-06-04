using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 baseScale;
    private Tween scaleTween;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(baseScale * 0.95f, 0.1f).SetEase(Ease.OutQuad).SetUpdate(true);

        if (HapticManager.Instance != null)
        {
            HapticManager.Instance.Light();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(baseScale, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayTap();
        }
    }
}
