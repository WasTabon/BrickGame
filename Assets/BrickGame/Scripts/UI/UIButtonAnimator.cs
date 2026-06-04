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
        scaleTween = transform.DOScale(baseScale * 0.95f, 0.1f).SetEase(Ease.OutQuad);

        if (HapticManager.Instance != null)
        {
            HapticManager.Instance.Light();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(baseScale, 0.2f).SetEase(Ease.OutBack);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayTap();
        }
        else
        {
            Debug.LogWarning("SoundManager is null on button press!");
        }
    }
}
