using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    private CanvasGroup fadeGroup;
    private const float FadeDuration = 0.3f;
    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildFadeOverlay();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void BuildFadeOverlay()
    {
        GameObject canvasGo = new GameObject("FadeCanvas");
        canvasGo.transform.SetParent(transform);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGo.AddComponent<GraphicRaycaster>();

        GameObject imageGo = new GameObject("FadeImage");
        imageGo.transform.SetParent(canvasGo.transform, false);
        Image image = imageGo.AddComponent<Image>();
        image.color = new Color(0.101f, 0.101f, 0.18f, 1f);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        fadeGroup = imageGo.AddComponent<CanvasGroup>();
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
    }

    public void FadeAndLoadScene(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        fadeGroup.blocksRaycasts = true;
        fadeGroup.DOFade(1f, FadeDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fadeGroup.alpha = 1f;
        fadeGroup.DOFade(0f, FadeDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            fadeGroup.blocksRaycasts = false;
            isTransitioning = false;
        });
    }
}
