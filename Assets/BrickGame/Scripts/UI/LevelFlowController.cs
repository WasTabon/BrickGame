using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelFlowController : MonoBehaviour
{
    public CollectionPit pit;
    public ResultPopup popup;
    public GameHUDController hud;
    public DemolitionController demolition;
    public Button collectButton;

    public float settleDelay = 0.6f;

    private bool collected;

    private void Start()
    {
        collectButton.onClick.AddListener(OnCollect);
        popup.onRetry = Retry;
        popup.onBattle = Battle;
    }

    private void OnCollect()
    {
        if (collected) return;
        collected = true;
        StartCoroutine(CollectRoutine());
    }

    private IEnumerator CollectRoutine()
    {
        demolition.enabled = false;
        hud.SetCollectVisible(false);

        yield return new WaitForSeconds(settleDelay);

        int count = pit.CurrentCount();
        int total = pit.totalBricks;
        int stars = ComputeStars(count, total);

        popup.Show(count, total, stars);
    }

    private int ComputeStars(int count, int total)
    {
        if (total <= 0) return 0;
        float p = (float)count / total;
        if (p >= 0.9f) return 3;
        if (p >= 0.7f) return 2;
        if (p >= 0.5f) return 1;
        return 0;
    }

    private void Retry()
    {
        TransitionManager.Instance.FadeAndLoadScene("Game");
    }

    private void Battle()
    {
        TransitionManager.Instance.FadeAndLoadScene("Game");
    }
}
