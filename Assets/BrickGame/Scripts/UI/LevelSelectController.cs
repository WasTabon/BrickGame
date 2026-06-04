using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectController : MonoBehaviour
{
    [Serializable]
    public class LevelButton
    {
        public int level;
        public Button button;
        public TextMeshProUGUI numberText;
        public Image[] stars;
        public GameObject lockOverlay;
    }

    public LevelButton[] buttons;
    public Button backButton;

    private readonly Color starOn = new Color(0.96f, 0.65f, 0.14f, 1f);
    private readonly Color starOff = new Color(1f, 1f, 1f, 0.22f);

    private void Start()
    {
        int unlocked = SaveSystem.UnlockedLevel;

        foreach (LevelButton lb in buttons)
        {
            LevelButton captured = lb;
            bool locked = captured.level > unlocked;

            captured.numberText.text = captured.level.ToString();
            captured.button.interactable = !locked;
            if (captured.lockOverlay != null) captured.lockOverlay.SetActive(locked);

            int stars = SaveSystem.GetStars(captured.level);
            for (int i = 0; i < captured.stars.Length; i++)
            {
                captured.stars[i].color = i < stars ? starOn : starOff;
            }

            captured.button.onClick.AddListener(() => Play(captured.level));
        }

        backButton.onClick.AddListener(Back);
    }

    private void Play(int level)
    {
        GameSession.Level = level;
        GameSession.CollectedBricks = 0;
        GameSession.IsDaily = false;
        TransitionManager.Instance.FadeAndLoadScene("Game");
    }

    private void Back()
    {
        TransitionManager.Instance.FadeAndLoadScene("MainMenu");
    }
}
