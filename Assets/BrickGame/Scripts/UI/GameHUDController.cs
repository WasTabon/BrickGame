using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameHUDController : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public TextMeshProUGUI levelText;
    public Button collectButton;
    public TextMeshProUGUI throwsText;

    private int total;
    private int shown;
    private Tween countTween;
    private Tween collectPulse;

    private void Start()
    {
        if (collectButton != null && collectButton.gameObject.activeSelf)
        {
            StartCollectPulse();
        }
    }

    public void SetLevel(int level)
    {
        if (levelText != null) levelText.text = "Level " + level;
    }

    public void SetThrows(int maxThrows)
    {
        if (throwsText == null) return;
        if (maxThrows <= 0)
        {
            throwsText.gameObject.SetActive(false);
            return;
        }
        throwsText.gameObject.SetActive(true);
        throwsText.text = "Throws: " + maxThrows;
    }

    public void SetThrowsLeft(int left)
    {
        if (throwsText == null || !throwsText.gameObject.activeSelf) return;
        throwsText.text = "Throws: " + Mathf.Max(0, left);
        throwsText.transform.DOComplete();
        throwsText.transform.DOPunchScale(Vector3.one * 0.18f, 0.25f, 6, 1f);
    }

    public void SetTotal(int value)
    {
        total = value;
        Refresh();
    }

    public void SetCount(int value)
    {
        countTween?.Kill();
        countTween = DOTween.To(() => shown, x => { shown = x; Refresh(); }, value, 0.25f)
            .SetEase(Ease.OutQuad);
    }

    private void Refresh()
    {
        countText.text = "Bricks: " + shown + " / " + total;
    }

    public void SetCollectVisible(bool visible)
    {
        if (collectButton == null) return;

        if (visible)
        {
            collectButton.gameObject.SetActive(true);
            collectButton.transform.localScale = Vector3.zero;
            collectButton.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack)
                .OnComplete(StartCollectPulse);
        }
        else
        {
            collectPulse?.Kill();
            collectButton.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack)
                .OnComplete(() => collectButton.gameObject.SetActive(false));
        }
    }

    private void StartCollectPulse()
    {
        collectPulse?.Kill();
        collectButton.transform.localScale = Vector3.one;
        collectPulse = collectButton.transform.DOScale(1.06f, 0.8f)
            .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
