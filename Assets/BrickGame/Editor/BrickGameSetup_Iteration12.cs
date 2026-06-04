using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BrickGameSetup_Iteration12
{
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";
    private const string LevelSelectPath = "Assets/BrickGame/Scenes/LevelSelect.unity";
    private const string TapPath = "Assets/BrickGame/Audio/sfx_tap.wav";
    private const string TransitionPath = "Assets/BrickGame/Audio/sfx_transition.wav";
    private const string StarPath = "Assets/BrickGame/Textures/star.png";

    private const int LevelCount = 15;

    private static readonly Color ColorAccent = HexColor("F5A623");
    private static readonly Color ColorBackground = HexColor("1A1A2E");
    private static readonly Color ColorPanel = HexColor("2E3358");
    private static readonly Color ColorSecondary = HexColor("596089");

    private static Sprite starSprite;

    [MenuItem("BrickGame/Setup Iteration 12 (Progress + Level Select)")]
    public static void Setup()
    {
        starSprite = AssetDatabase.LoadAssetAtPath<Sprite>(StarPath);

        SetupGame();
        BuildLevelSelect();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 12 setup complete. 15 levels + scrollable LevelSelect.");
    }

    private static void SetupGame()
    {
        EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);
        GameObject lmGo = GameObject.Find("LevelManager");
        if (lmGo == null) return;

        LevelManager lm = lmGo.GetComponent<LevelManager>();
        if (lm.pitCentersSingle == null || lm.pitCentersSingle.Length == 0)
            lm.pitCentersSingle = new float[] { 1.2f };
        if (lm.pitCentersDouble == null || lm.pitCentersDouble.Length == 0)
            lm.pitCentersDouble = new float[] { -0.3f, 2.4f };

        lm.levels = BuildLevels();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static List<LevelDef> BuildLevels()
    {
        return new List<LevelDef>
        {
            new LevelDef { rows = 16, columns = 2, pitCount = 1 },
            new LevelDef { rows = 17, columns = 2, pitCount = 1, woodEvery = 5 },
            new LevelDef { rows = 18, columns = 2, pitCount = 1, stoneEvery = 6 },
            new LevelDef { rows = 19, columns = 2, pitCount = 1, stoneEvery = 6, woodEvery = 7 },
            new LevelDef { rows = 20, columns = 2, pitCount = 2 },
            new LevelDef { rows = 21, columns = 3, pitCount = 2, stoneEvery = 6 },
            new LevelDef { rows = 22, columns = 3, pitCount = 2, iceRows = 3 },
            new LevelDef { rows = 22, columns = 3, pitCount = 2, stoneEvery = 6, iceRows = 3 },
            new LevelDef { rows = 23, columns = 3, pitCount = 2, bombEvery = 9 },
            new LevelDef { rows = 24, columns = 3, pitCount = 2, stoneEvery = 5, bombEvery = 9 },
            new LevelDef { rows = 24, columns = 3, pitCount = 2, iceRows = 4, woodEvery = 7 },
            new LevelDef { rows = 25, columns = 3, pitCount = 2, stoneEvery = 5, bombEvery = 10 },
            new LevelDef { rows = 26, columns = 3, pitCount = 2, iceRows = 4, stoneEvery = 6, bombEvery = 11 },
            new LevelDef { rows = 26, columns = 3, pitCount = 2, stoneEvery = 5, woodEvery = 7, bombEvery = 9 },
            new LevelDef { rows = 28, columns = 3, pitCount = 2, iceRows = 5, stoneEvery = 5, woodEvery = 8, bombEvery = 11 },
        };
    }

    private static void BuildLevelSelect()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera();
        CreateManagers();

        GameObject canvas = CreateCanvas(out GameObject safeArea);

        TextMeshProUGUI title = CreateText("Title", safeArea.transform, "SELECT LEVEL", 80, ColorAccent);
        RectTransform titleRt = title.rectTransform;
        titleRt.anchorMin = new Vector2(0.5f, 1f);
        titleRt.anchorMax = new Vector2(0.5f, 1f);
        titleRt.pivot = new Vector2(0.5f, 1f);
        titleRt.sizeDelta = new Vector2(900f, 130f);
        titleRt.anchoredPosition = new Vector2(0f, -40f);
        title.fontStyle = FontStyles.Bold;
        ApplyOutline(title);

        GameObject scrollGo = new GameObject("ScrollView", typeof(RectTransform));
        scrollGo.transform.SetParent(safeArea.transform, false);
        RectTransform scrollRt = scrollGo.GetComponent<RectTransform>();
        scrollRt.anchorMin = new Vector2(0f, 0f);
        scrollRt.anchorMax = new Vector2(1f, 1f);
        scrollRt.offsetMin = new Vector2(40f, 240f);
        scrollRt.offsetMax = new Vector2(-40f, -200f);
        ScrollRect scroll = scrollGo.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Elastic;
        scroll.scrollSensitivity = 30f;

        GameObject viewport = new GameObject("Viewport", typeof(RectTransform));
        viewport.transform.SetParent(scrollGo.transform, false);
        RectTransform vpRt = viewport.GetComponent<RectTransform>();
        StretchFull(vpRt);
        Image vpImg = viewport.AddComponent<Image>();
        vpImg.color = new Color(1f, 1f, 1f, 0.02f);
        viewport.AddComponent<RectMask2D>();
        scroll.viewport = vpRt;

        float cellW = 760f;
        float cellH = 230f;
        float cellGap = 40f;
        float topPad = 20f;
        float contentHeight = topPad * 2f + LevelCount * cellH + (LevelCount - 1) * cellGap;

        GameObject content = new GameObject("Content", typeof(RectTransform));
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRt = content.GetComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.sizeDelta = new Vector2(0f, contentHeight);
        contentRt.anchoredPosition = Vector2.zero;
        scroll.content = contentRt;

        LevelSelectController.LevelButton[] buttons = new LevelSelectController.LevelButton[LevelCount];
        for (int i = 0; i < LevelCount; i++)
        {
            int level = i + 1;
            float y = -(topPad + cellH * 0.5f + i * (cellH + cellGap));
            buttons[i] = BuildLevelCell(content.transform, level, new Vector2(0f, y), new Vector2(cellW, cellH));
        }

        GameObject backGo = CreateButton("BackButton", safeArea.transform, ColorSecondary, out Button backButton);
        RectTransform backRt = backGo.GetComponent<RectTransform>();
        backRt.anchorMin = new Vector2(0.5f, 0f);
        backRt.anchorMax = new Vector2(0.5f, 0f);
        backRt.pivot = new Vector2(0.5f, 0f);
        backRt.sizeDelta = new Vector2(420f, 150f);
        backRt.anchoredPosition = new Vector2(0f, 60f);
        TextMeshProUGUI backLabel = CreateText("Label", backGo.transform, "BACK", 56, Color.white);
        StretchFull(backLabel.rectTransform);
        backLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(backLabel);

        GameObject controllerGo = new GameObject("LevelSelectController");
        LevelSelectController controller = controllerGo.AddComponent<LevelSelectController>();
        controller.buttons = buttons;
        controller.backButton = backButton;

        EditorSceneManager.SaveScene(scene, LevelSelectPath);
        AddSceneToBuild(LevelSelectPath);
    }

    private static LevelSelectController.LevelButton BuildLevelCell(Transform parent, int level, Vector2 pos, Vector2 size)
    {
        GameObject cellGo = CreateButton("Level" + level, parent, ColorPanel, out Button button);
        RectTransform rt = cellGo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;

        TextMeshProUGUI number = CreateText("Number", cellGo.transform, level.ToString(), 110, Color.white);
        RectTransform nrt = number.rectTransform;
        nrt.anchorMin = new Vector2(0f, 0.5f);
        nrt.anchorMax = new Vector2(0f, 0.5f);
        nrt.pivot = new Vector2(0f, 0.5f);
        nrt.sizeDelta = new Vector2(260f, 180f);
        nrt.anchoredPosition = new Vector2(50f, 0f);
        number.alignment = TextAlignmentOptions.Left;
        number.fontStyle = FontStyles.Bold;
        ApplyOutline(number);

        Image[] stars = new Image[3];
        float[] sx = { 380f, 500f, 620f };
        for (int s = 0; s < 3; s++)
        {
            GameObject starGo = new GameObject("Star" + s, typeof(RectTransform));
            starGo.transform.SetParent(cellGo.transform, false);
            Image img = starGo.AddComponent<Image>();
            img.sprite = starSprite;
            img.color = new Color(1f, 1f, 1f, 0.22f);
            img.preserveAspect = true;
            img.raycastTarget = false;
            RectTransform srt = img.rectTransform;
            srt.anchorMin = new Vector2(0f, 0.5f);
            srt.anchorMax = new Vector2(0f, 0.5f);
            srt.pivot = new Vector2(0.5f, 0.5f);
            srt.sizeDelta = new Vector2(90f, 90f);
            srt.anchoredPosition = new Vector2(sx[s], 0f);
            stars[s] = img;
        }

        GameObject lockGo = new GameObject("LockOverlay", typeof(RectTransform));
        lockGo.transform.SetParent(cellGo.transform, false);
        Image lockImg = lockGo.AddComponent<Image>();
        lockImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        lockImg.type = Image.Type.Sliced;
        lockImg.color = new Color(0.05f, 0.05f, 0.1f, 0.7f);
        lockImg.raycastTarget = false;
        StretchFull(lockImg.rectTransform);
        TextMeshProUGUI lockText = CreateText("LockText", lockGo.transform, "LOCKED", 50, new Color(1f, 1f, 1f, 0.85f));
        StretchFull(lockText.rectTransform);
        lockText.fontStyle = FontStyles.Bold;

        return new LevelSelectController.LevelButton
        {
            level = level,
            button = button,
            numberText = number,
            stars = stars,
            lockOverlay = lockGo
        };
    }

    private static void CreateCamera()
    {
        GameObject camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        Camera cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 10f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = ColorBackground;
        camGo.transform.position = new Vector3(0f, 0f, -10f);
        camGo.AddComponent<AudioListener>();
    }

    private static void CreateManagers()
    {
        GameObject bootstrap = new GameObject("GameBootstrap");
        bootstrap.AddComponent<GameBootstrap>();

        GameObject sound = new GameObject("SoundManager");
        SoundManager sm = sound.AddComponent<SoundManager>();
        sm.tapClip = AssetDatabase.LoadAssetAtPath<AudioClip>(TapPath);
        sm.transitionClip = AssetDatabase.LoadAssetAtPath<AudioClip>(TransitionPath);

        GameObject haptic = new GameObject("HapticManager");
        haptic.AddComponent<HapticManager>();

        GameObject transition = new GameObject("TransitionManager");
        transition.AddComponent<TransitionManager>();
    }

    private static GameObject CreateCanvas(out GameObject safeArea)
    {
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        safeArea = new GameObject("SafeArea", typeof(RectTransform));
        safeArea.transform.SetParent(canvasGo.transform, false);
        StretchFull(safeArea.GetComponent<RectTransform>());
        safeArea.AddComponent<SafeAreaFitter>();

        return canvasGo;
    }

    private static GameObject CreateImage(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.color = color;
        image.type = Image.Type.Sliced;
        return go;
    }

    private static GameObject CreateButton(string name, Transform parent, Color color, out Button button)
    {
        GameObject go = CreateImage(name, parent, color);
        Image image = go.GetComponent<Image>();
        button = go.AddComponent<Button>();
        button.targetGraphic = image;
        go.AddComponent<UIButtonAnimator>();
        return go;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, string content, float size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = size;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;

        TMP_FontAsset font = TMP_Settings.defaultFontAsset;
        if (font != null) text.font = font;
        return text;
    }

    private static void ApplyOutline(TextMeshProUGUI text)
    {
        if (text.fontSharedMaterial == null) return;
        Material instance = new Material(text.fontSharedMaterial);
        text.fontMaterial = instance;
        text.outlineColor = Color.black;
        text.outlineWidth = 0.15f;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void AddSceneToBuild(string path)
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (EditorBuildSettingsScene s in scenes)
        {
            if (s.path == path) return;
        }
        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
