using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ResultPopup : MonoBehaviour
{
    public CanvasGroup backdrop;
    public RectTransform panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI countText;
    public Image[] stars;
    public Button retryButton;
    public Button battleButton;

    public Action onRetry;
    public Action onBattle;

    private readonly Color starOn = new Color(0.96f, 0.65f, 0.14f, 1f);
    private readonly Color starOff = new Color(1f, 1f, 1f, 0.22f);
    private bool listenersReady;

    private void Start()
    {
        EnsureListeners();
    }

    private void EnsureListeners()
    {
        if (listenersReady) return;
        listenersReady = true;
        retryButton.onClick.AddListener(() => onRetry?.Invoke());
        battleButton.onClick.AddListener(() => onBattle?.Invoke());
    }

    public void Show(int collected, int total, int starCount)
    {
        gameObject.SetActive(true);
        EnsureListeners();

        countText.text = collected + " / " + total;

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = starOff;
            stars[i].transform.localScale = Vector3.zero;
        }

        backdrop.alpha = 0f;
        backdrop.blocksRaycasts = true;
        backdrop.DOFade(0.6f, 0.25f);

        panel.localScale = Vector3.zero;
        panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

        for (int i = 0; i < stars.Length; i++)
        {
            int idx = i;
            float delay = 0.35f + i * 0.15f;

            if (idx < starCount)
            {
                stars[idx].color = starOn;
                stars[idx].transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack).SetDelay(delay);
            }
            else
            {
                stars[idx].transform.DOScale(0.85f, 0.3f).SetEase(Ease.OutQuad).SetDelay(delay);
            }
        }
    }

    public void Hide()
    {
        backdrop.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
}
