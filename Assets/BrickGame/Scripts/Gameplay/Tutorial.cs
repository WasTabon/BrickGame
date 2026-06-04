using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    public SpriteRenderer ring;
    public SpriteRenderer ripple;
    public SpriteRenderer arrow;
    public Vector3 tapWorld = new Vector3(2.5f, 0.6f, 0f);

    private bool dismissed;
    private Vector3 rippleBase;

    private void Start()
    {
        if (GameSession.Level != 1 || PlayerPrefs.GetInt("TutDone", 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        ring.transform.position = tapWorld;
        ripple.transform.position = tapWorld;

        ring.transform.DOScale(ring.transform.localScale * 1.25f, 0.7f)
            .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        rippleBase = ripple.transform.localScale;
        ripple.transform.localScale = rippleBase * 0.4f;
        SetAlpha(ripple, 0.85f);

        Sequence seq = DOTween.Sequence();
        seq.Append(ripple.transform.DOScale(rippleBase * 1.3f, 0.9f).SetEase(Ease.OutQuad));
        seq.Join(ripple.DOFade(0f, 0.9f));
        seq.AppendCallback(() =>
        {
            ripple.transform.localScale = rippleBase * 0.4f;
            SetAlpha(ripple, 0.85f);
        });
        seq.SetLoops(-1);

        if (arrow != null)
        {
            float baseX = arrow.transform.position.x;
            arrow.transform.DOMoveX(baseX - 0.6f, 0.8f)
                .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void Update()
    {
        if (dismissed) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Dismiss();
    }

    private void Dismiss()
    {
        dismissed = true;
        PlayerPrefs.SetInt("TutDone", 1);
        PlayerPrefs.Save();

        FadeOut(ring);
        FadeOut(ripple);
        if (arrow != null) FadeOut(arrow);

        Invoke(nameof(Disable), 0.4f);
    }

    private void FadeOut(SpriteRenderer sr)
    {
        if (sr == null) return;
        sr.transform.DOKill();
        sr.DOKill();
        sr.DOFade(0f, 0.3f);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private void SetAlpha(SpriteRenderer sr, float a)
    {
        Color c = sr.color;
        c.a = a;
        sr.color = c;
    }
}
