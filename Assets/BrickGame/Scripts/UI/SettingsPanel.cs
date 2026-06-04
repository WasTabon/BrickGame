using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SettingsPanel : MonoBehaviour
{
    public CanvasGroup backdrop;
    public RectTransform panel;
    public Button closeButton;
    public Button sfxButton;
    public TextMeshProUGUI sfxLabel;
    public Button musicButton;
    public TextMeshProUGUI musicLabel;
    public Button hapticButton;
    public TextMeshProUGUI hapticLabel;

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
        sfxButton.onClick.AddListener(ToggleSfx);
        musicButton.onClick.AddListener(ToggleMusic);
        hapticButton.onClick.AddListener(ToggleHaptic);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        EnsureListeners();
        Refresh();

        backdrop.alpha = 0f;
        backdrop.blocksRaycasts = true;
        backdrop.DOFade(0.6f, 0.25f).SetUpdate(true);

        panel.localScale = Vector3.zero;
        panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void Hide()
    {
        backdrop.blocksRaycasts = false;
        backdrop.DOFade(0f, 0.2f).SetUpdate(true);
        panel.DOScale(0f, 0.2f).SetEase(Ease.InBack).SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void ToggleSfx()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetSfxMuted(!SoundManager.Instance.IsSfxMuted());
        }
        Refresh();
    }

    private void ToggleMusic()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicMuted(!SoundManager.Instance.IsMusicMuted());
        }
        Refresh();
    }

    private void ToggleHaptic()
    {
        if (HapticManager.Instance != null)
        {
            HapticManager.Instance.SetEnabled(!HapticManager.Instance.IsEnabled());
        }
        Refresh();
    }

    private void Refresh()
    {
        bool sfxOn = SoundManager.Instance == null || !SoundManager.Instance.IsSfxMuted();
        bool musicOn = SoundManager.Instance == null || !SoundManager.Instance.IsMusicMuted();
        bool hapticOn = HapticManager.Instance == null || HapticManager.Instance.IsEnabled();

        sfxLabel.text = "SFX: " + (sfxOn ? "ON" : "OFF");
        musicLabel.text = "Music: " + (musicOn ? "ON" : "OFF");
        hapticLabel.text = "Haptics: " + (hapticOn ? "ON" : "OFF");
    }
}
