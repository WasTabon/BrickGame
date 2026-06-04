using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PausePanel : MonoBehaviour
{
    public CanvasGroup backdrop;
    public RectTransform panel;
    public Button resumeButton;
    public Button restartButton;
    public Button menuButton;
    public Button settingsButton;
    public SettingsPanel settings;
    public DemolitionController demolition;

    private bool paused;
    private bool ready;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        EnsureListeners();
    }

    private void EnsureListeners()
    {
        if (ready) return;
        ready = true;

        resumeButton.onClick.AddListener(Resume);
        restartButton.onClick.AddListener(Restart);
        menuButton.onClick.AddListener(Menu);
        settingsButton.onClick.AddListener(() => settings.Show());
    }

    public void Pause()
    {
        if (paused) return;
        EnsureListeners();
        paused = true;
        Time.timeScale = 0f;

        if (demolition != null) demolition.enabled = false;

        gameObject.SetActive(true);
        backdrop.alpha = 0f;
        backdrop.blocksRaycasts = true;
        backdrop.DOFade(0.6f, 0.25f).SetUpdate(true);
        panel.localScale = Vector3.zero;
        panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void Resume()
    {
        paused = false;
        Time.timeScale = 1f;

        if (demolition != null) demolition.enabled = true;

        backdrop.blocksRaycasts = false;
        backdrop.DOFade(0f, 0.2f).SetUpdate(true);
        panel.DOScale(0f, 0.2f).SetEase(Ease.InBack).SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        TransitionManager.Instance.FadeAndLoadScene("Game");
    }

    private void Menu()
    {
        Time.timeScale = 1f;
        TransitionManager.Instance.FadeAndLoadScene("MainMenu");
    }
}
