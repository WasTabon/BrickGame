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

    public AudioClip fanfare;

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

        if (starCount >= 3)
        {
            Celebrate();
        }
    }

    private void Celebrate()
    {
        if (fanfare != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySfx(fanfare, 1f, 0.9f);
        }

        Sprite sq = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f));
        Color[] colors =
        {
            new Color(0.96f, 0.65f, 0.14f),
            new Color(0.29f, 0.56f, 0.89f),
            new Color(0.44f, 0.82f, 0.44f),
            new Color(0.90f, 0.30f, 0.40f),
            new Color(0.85f, 0.85f, 0.95f)
        };

        for (int i = 0; i < 28; i++)
        {
            GameObject piece = new GameObject("Confetti", typeof(RectTransform));
            piece.transform.SetParent(panel.parent, false);
            UnityEngine.UI.Image img = piece.AddComponent<UnityEngine.UI.Image>();
            img.sprite = sq;
            img.color = colors[i % colors.Length];
            img.raycastTarget = false;

            RectTransform prt = img.rectTransform;
            prt.sizeDelta = new Vector2(UnityEngine.Random.Range(18f, 34f), UnityEngine.Random.Range(18f, 34f));
            float startX = UnityEngine.Random.Range(-400f, 400f);
            prt.anchoredPosition = new Vector2(startX, 900f);
            prt.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));

            float fall = UnityEngine.Random.Range(1.1f, 2.0f);
            float drift = UnityEngine.Random.Range(-160f, 160f);
            prt.DOAnchorPos(new Vector2(startX + drift, -1100f), fall).SetEase(Ease.InQuad).SetDelay(UnityEngine.Random.Range(0f, 0.3f));
            prt.DORotate(new Vector3(0f, 0f, UnityEngine.Random.Range(-360f, 360f)), fall, RotateMode.FastBeyond360);
            DOTween.To(() => img.color.a, a => { Color c = img.color; c.a = a; img.color = c; }, 0f, fall).SetEase(Ease.InQuad);

            Destroy(piece, fall + 0.5f);
        }
    }

    public void Hide()
    {
        backdrop.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
}
