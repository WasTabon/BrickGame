using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleResultPopup : MonoBehaviour
{
    public CanvasGroup backdrop;
    public RectTransform panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Button actionButton;
    public TextMeshProUGUI actionLabel;

    private bool victory;
    private bool ready;

    private void Start()
    {
        EnsureListener();
    }

    private void EnsureListener()
    {
        if (ready) return;
        ready = true;
        actionButton.onClick.AddListener(OnAction);
    }

    public void Show(bool win)
    {
        victory = win;
        gameObject.SetActive(true);
        EnsureListener();

        titleText.text = win ? "VICTORY" : "DEFEAT";
        titleText.color = win ? new Color(0.96f, 0.65f, 0.14f, 1f) : new Color(0.88f, 0.33f, 0.27f, 1f);
        subtitleText.text = win ? "Wave cleared!" : "They broke through...";
        actionLabel.text = win ? "NEXT" : "RETRY";

        backdrop.alpha = 0f;
        backdrop.blocksRaycasts = true;
        backdrop.DOFade(0.6f, 0.25f);

        panel.localScale = Vector3.zero;
        panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
    }

    private void OnAction()
    {
        if (victory) GameSession.Level++;
        TransitionManager.Instance.FadeAndLoadScene("Game");
    }
}
