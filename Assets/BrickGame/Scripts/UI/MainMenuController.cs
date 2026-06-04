using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    public Button playButton;
    public Button settingsButton;
    public Button upgradesButton;
    public TextMeshProUGUI brickCountText;
    public UpgradesPanel upgradesPanel;
    public SettingsPanel settingsPanel;
    public Button dailyButton;
    public TextMeshProUGUI dailyLabel;
    public Button achievementsButton;
    public AchievementsPanel achievementsPanel;

    private int shownBank;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);

        if (upgradesButton != null)
        {
            upgradesButton.onClick.AddListener(OnUpgradesClicked);
        }

        if (upgradesPanel != null)
        {
            upgradesPanel.onChanged = RefreshBank;
        }

        if (dailyButton != null) dailyButton.onClick.AddListener(OnDailyClicked);
        if (achievementsButton != null) achievementsButton.onClick.AddListener(OnAchievementsClicked);

        if (dailyLabel != null)
        {
            dailyLabel.text = DailyChallenge.IsTodayDone() ? "DAILY  ✓" : "DAILY";
        }

        AnimateBank();

        playButton.transform.localScale = Vector3.zero;
        playButton.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.1f);
    }

    private void AnimateBank()
    {
        int bank = Economy.Bank;
        shownBank = 0;
        brickCountText.text = "0";
        DOTween.To(() => shownBank, x => { shownBank = x; brickCountText.text = shownBank.ToString(); }, bank, 0.6f)
            .SetEase(Ease.OutQuad);
    }

    private void RefreshBank()
    {
        brickCountText.text = Economy.Bank.ToString();
    }

    private void OnPlayClicked()
    {
        GameSession.IsDaily = false;
        SoundManager.Instance.PlayTransition();
        HapticManager.Instance.Medium();
        TransitionManager.Instance.FadeAndLoadScene("LevelSelect");
    }

    private void OnDailyClicked()
    {
        GameSession.IsDaily = true;
        GameSession.CollectedBricks = 0;
        SoundManager.Instance.PlayTransition();
        HapticManager.Instance.Medium();
        TransitionManager.Instance.FadeAndLoadScene("Game");
    }

    private void OnAchievementsClicked()
    {
        if (achievementsPanel != null) achievementsPanel.Show();
    }

    private void OnUpgradesClicked()
    {
        upgradesPanel.Show();
    }

    private void OnSettingsClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.Show();
        }
    }
}
