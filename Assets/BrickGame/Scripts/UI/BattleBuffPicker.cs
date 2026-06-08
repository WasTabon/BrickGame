using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleBuffPicker : MonoBehaviour
{
    [Serializable]
    public class Card
    {
        public Button button;
        public RectTransform root;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descText;
    }

    public CanvasGroup backdrop;
    public RectTransform panel;
    public TextMeshProUGUI title;
    public Card[] cards;

    private Action<BuffType> onChosen;
    private BuffType[] current;

    public void Show(Action<BuffType> callback)
    {
        onChosen = callback;
        gameObject.SetActive(true);

        var picks = BattleBuffs.PickThree();
        current = new BuffType[picks.Count];

        for (int i = 0; i < cards.Length; i++)
        {
            if (i < picks.Count)
            {
                current[i] = picks[i].type;
                cards[i].nameText.text = picks[i].name;
                cards[i].descText.text = picks[i].desc;
                cards[i].root.gameObject.SetActive(true);

                int idx = i;
                cards[i].button.onClick.RemoveAllListeners();
                cards[i].button.onClick.AddListener(() => Choose(idx));

                cards[i].root.localScale = Vector3.zero;
                cards[i].root.DOScale(1f, 0.35f).SetEase(Ease.OutBack).SetDelay(0.1f + i * 0.08f).SetUpdate(true);
            }
            else
            {
                cards[i].root.gameObject.SetActive(false);
            }
        }

        backdrop.alpha = 0f;
        backdrop.blocksRaycasts = true;
        backdrop.DOFade(0.75f, 0.25f).SetUpdate(true);
    }

    private void Choose(int index)
    {
        BattleBuffs.Choose(current[index]);

        if (HapticManager.Instance != null) HapticManager.Instance.Medium();

        backdrop.blocksRaycasts = false;
        backdrop.DOFade(0f, 0.2f).SetUpdate(true);
        panel.DOScale(0f, 0.2f).SetEase(Ease.InBack).SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onChosen?.Invoke(current[index]);
            });
    }
}
