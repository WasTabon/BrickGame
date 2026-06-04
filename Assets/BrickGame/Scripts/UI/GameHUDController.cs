using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameHUDController : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public TextMeshProUGUI levelText;
    public Button collectButton;

    private int total;
    private int shown;
    private Tween countTween;

    public void SetLevel(int level)
    {
        if (levelText != null) levelText.text = "Level " + level;
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
            collectButton.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack);
        }
        else
        {
            collectButton.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack)
                .OnComplete(() => collectButton.gameObject.SetActive(false));
        }
    }
}
