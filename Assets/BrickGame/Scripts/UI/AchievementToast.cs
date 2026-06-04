using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class AchievementToast : MonoBehaviour
{
    public RectTransform panel;
    public TextMeshProUGUI label;

    private readonly Queue<string> queue = new Queue<string>();
    private bool showing;
    private float hiddenY;
    private float shownY;

    private void Awake()
    {
        if (panel != null)
        {
            shownY = panel.anchoredPosition.y;
            hiddenY = shownY + 220f;
            panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, hiddenY);
            panel.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        Achievements.OnUnlocked += Enqueue;
    }

    private void OnDisable()
    {
        Achievements.OnUnlocked -= Enqueue;
    }

    private void Enqueue(string name)
    {
        queue.Enqueue(name);
        if (!showing) StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        showing = true;
        while (queue.Count > 0)
        {
            string n = queue.Dequeue();
            yield return Toast(n);
        }
        showing = false;
    }

    private IEnumerator Toast(string name)
    {
        if (panel == null) yield break;

        label.text = "Unlocked: " + name;
        panel.gameObject.SetActive(true);
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, hiddenY);

        if (HapticManager.Instance != null) HapticManager.Instance.Medium();

        panel.DOAnchorPosY(shownY, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
        yield return new WaitForSecondsRealtime(2.0f);
        panel.DOAnchorPosY(hiddenY, 0.35f).SetEase(Ease.InBack).SetUpdate(true);
        yield return new WaitForSecondsRealtime(0.4f);

        panel.gameObject.SetActive(false);
    }
}
