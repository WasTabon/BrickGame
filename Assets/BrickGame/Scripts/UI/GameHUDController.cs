using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameHUDController : MonoBehaviour
{
    public TextMeshProUGUI countText;

    private int total;
    private int shown;
    private Tween countTween;

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
}
