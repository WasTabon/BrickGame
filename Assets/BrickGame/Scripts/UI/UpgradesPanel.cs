using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UpgradesPanel : MonoBehaviour
{
    [Serializable]
    public class UpgradeRow
    {
        public string key;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI costText;
        public Button buyButton;
    }

    public CanvasGroup backdrop;
    public RectTransform panel;
    public TextMeshProUGUI bankText;
    public Button closeButton;
    public UpgradeRow[] rows;

    public Action onChanged;

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

        foreach (UpgradeRow row in rows)
        {
            UpgradeRow captured = row;
            captured.buyButton.onClick.AddListener(() => Buy(captured));
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        EnsureListeners();

        backdrop.alpha = 0f;
        backdrop.blocksRaycasts = true;
        backdrop.DOFade(0.6f, 0.25f);

        panel.localScale = Vector3.zero;
        panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

        RefreshAll();
    }

    public void Hide()
    {
        backdrop.blocksRaycasts = false;
        backdrop.DOFade(0f, 0.2f);
        panel.DOScale(0f, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onChanged?.Invoke();
            });
    }

    private void Buy(UpgradeRow row)
    {
        if (Upgrades.Buy(row.key))
        {
            RefreshAll();
            row.buyButton.transform.DOComplete();
            row.buyButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 6, 1f);
        }
        else
        {
            row.buyButton.transform.DOComplete();
            row.buyButton.transform.DOPunchPosition(new Vector3(12f, 0f, 0f), 0.3f, 12, 1f);
        }
    }

    private void RefreshAll()
    {
        bankText.text = Economy.Bank.ToString();
        foreach (UpgradeRow row in rows)
        {
            RefreshRow(row);
        }
    }

    private void RefreshRow(UpgradeRow row)
    {
        int level = Upgrades.GetLevel(row.key);
        row.levelText.text = "Lv " + level + "/" + Upgrades.MaxLevel;

        int cost = Upgrades.CostFor(row.key);
        if (cost < 0)
        {
            row.costText.text = "MAX";
            row.buyButton.interactable = false;
        }
        else
        {
            row.costText.text = cost.ToString();
            row.buyButton.interactable = Economy.Bank >= cost;
        }
    }
}
