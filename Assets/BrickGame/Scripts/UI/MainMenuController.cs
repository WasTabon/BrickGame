using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    public Button playButton;
    public Button settingsButton;
    public TextMeshProUGUI brickCountText;

    private const string BankKey = "BrickBank";

    private void Start()
    {
        int bank = PlayerPrefs.GetInt(BankKey, 0);
        brickCountText.text = bank.ToString();

        playButton.onClick.AddListener(OnPlayClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);

        playButton.transform.localScale = Vector3.zero;
        playButton.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.1f);
    }

    private void OnPlayClicked()
    {
        SoundManager.Instance.PlayTransition();
        HapticManager.Instance.Medium();
        TransitionManager.Instance.FadeAndLoadScene("Game");
    }

    private void OnSettingsClicked()
    {
        settingsButton.transform.DOComplete();
        settingsButton.transform.DOPunchRotation(new Vector3(0f, 0f, 15f), 0.3f, 8, 1f);
    }
}
