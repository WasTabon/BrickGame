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
        SoundManager.Instance.PlayTransition();
        HapticManager.Instance.Medium();
        TransitionManager.Instance.FadeAndLoadScene("LevelSelect");
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
