using UnityEngine;
using TMPro;
using DG.Tweening;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    public TextMeshProUGUI comboText;
    public float comboWindow = 1.3f;

    private int combo;
    private float lastTime;
    private TMP_FontAsset font;
    private CameraShake shake;

    private readonly Color popupColor = new Color(0.96f, 0.65f, 0.14f, 1f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        font = TMP_Settings.defaultFontAsset;

        if (comboText != null)
        {
            comboText.alpha = 0f;
        }
    }

    private void Start()
    {
        if (Camera.main != null) shake = Camera.main.GetComponent<CameraShake>();
    }

    public void RegisterCollect(Vector3 pos)
    {
        if (Time.time - lastTime <= comboWindow) combo++;
        else combo = 1;
        lastTime = Time.time;

        SpawnPopup(pos);
        UpdateComboUI();

        if (combo >= 5) Achievements.Unlock("combo_5");

        if (shake != null && combo >= 3)
        {
            shake.Shake(Mathf.Min(0.05f + combo * 0.012f, 0.22f));
        }

        CancelInvoke(nameof(EndCombo));
        Invoke(nameof(EndCombo), comboWindow);
    }

    private void EndCombo()
    {
        combo = 0;
        if (comboText != null)
        {
            DOTween.To(() => comboText.alpha, a => comboText.alpha = a, 0f, 0.3f);
        }
    }

    private void UpdateComboUI()
    {
        if (comboText == null) return;

        if (combo >= 2)
        {
            comboText.text = "Combo x" + combo;
            comboText.alpha = 1f;
            comboText.transform.DOComplete();
            comboText.transform.localScale = Vector3.one;
            comboText.transform.DOPunchScale(Vector3.one * 0.25f, 0.3f, 6, 1f);
        }
    }

    private void SpawnPopup(Vector3 pos)
    {
        GameObject go = new GameObject("CollectPopup");
        go.transform.position = pos + Vector3.up * 0.3f;
        go.transform.localScale = Vector3.one * 0.5f;

        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = "+1";
        tmp.fontSize = 10f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = popupColor;
        tmp.sortingOrder = 100;
        if (font != null) tmp.font = font;

        RectTransform rt = tmp.rectTransform;
        rt.sizeDelta = new Vector2(3f, 1.5f);

        go.transform.DOMoveY(pos.y + 1.6f, 0.7f).SetEase(Ease.OutQuad);
        DOTween.To(() => tmp.alpha, a => tmp.alpha = a, 0f, 0.7f).SetDelay(0.25f);

        Destroy(go, 1f);
    }
}
