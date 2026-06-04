using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AchievementsPanel : MonoBehaviour
{
    [Serializable]
    public class Row
    {
        public string key;
        public TextMeshProUGUI statusText;
    }

    public CanvasGroup backdrop;
    public RectTransform panel;
    public Button closeButton;
    public Row[] rows;

    private readonly Color unlockedColor = new Color(0.44f, 0.82f, 0.44f, 1f);
    private readonly Color lockedColor = new Color(1f, 1f, 1f, 0.35f);
    private bool ready;

    private void Start()
    {
        EnsureListeners();
    }

    private void EnsureListeners()
    {
        if (ready) return;
        ready = true;
        closeButton.onClick.AddListener(Hide);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        EnsureListeners();
        Refresh();

        backdrop.alpha = 0f;
        backdrop.blocksRaycasts = true;
        backdrop.DOFade(0.6f, 0.25f);

        panel.localScale = Vector3.zero;
        panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        backdrop.blocksRaycasts = false;
        backdrop.DOFade(0f, 0.2f);
        panel.DOScale(0f, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void Refresh()
    {
        foreach (Row row in rows)
        {
            bool unlocked = Achievements.IsUnlocked(row.key);
            row.statusText.text = unlocked ? "UNLOCKED" : "LOCKED";
            row.statusText.color = unlocked ? unlockedColor : lockedColor;
        }
    }
}
