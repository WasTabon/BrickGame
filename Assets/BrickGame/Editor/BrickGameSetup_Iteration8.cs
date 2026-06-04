using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BrickGameSetup_Iteration8
{
    private const string MainMenuPath = "Assets/BrickGame/Scenes/MainMenu.unity";
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";

    private static readonly Color ColorPrimary = HexColor("4A90E2");
    private static readonly Color ColorAccent = HexColor("F5A623");
    private static readonly Color ColorPanel = HexColor("232347");
    private static readonly Color ColorRow = HexColor("2E3358");
    private static readonly Color ColorSecondary = HexColor("596089");

    [MenuItem("BrickGame/Setup Iteration 8 (Settings + Pause)")]
    public static void Setup()
    {
        SetupMainMenu();
        SetupGame();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 8 setup complete. Pause + settings added.");
    }

    private static void SetupMainMenu()
    {
        EditorSceneManager.OpenScene(MainMenuPath, OpenSceneMode.Single);

        GameObject canvas = GameObject.Find("Canvas");
        GameObject controllerGo = GameObject.Find("MainMenuController");
        if (canvas == null || controllerGo == null)
        {
            Debug.LogWarning("MainMenu objects not found. Run earlier setups first.");
            return;
        }

        DestroyIfExists("Canvas/SettingsPanel");

        SettingsPanel settings = BuildSettingsPanel(canvas.transform);

        MainMenuController controller = controllerGo.GetComponent<MainMenuController>();
        controller.settingsPanel = settings;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupGame()
    {
        EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);

        GameObject canvas = GameObject.Find("Canvas");
        GameObject safeArea = GameObject.Find("Canvas/SafeArea");
        GameObject demolitionGo = GameObject.Find("DemolitionController");
        if (canvas == null || safeArea == null)
        {
            Debug.LogWarning("Game objects not found. Run earlier setups first.");
            return;
        }

        DestroyIfExists("Canvas/SafeArea/PauseButton");
        DestroyIfExists("Canvas/PausePanel");
        DestroyIfExists("Canvas/SettingsPanel");

        GameObject pauseGo = CreateButton("PauseButton", safeArea.transform, ColorSecondary, out _);
        RectTransform pauseRt = pauseGo.GetComponent<RectTransform>();
        pauseRt.anchorMin = new Vector2(0f, 1f);
        pauseRt.anchorMax = new Vector2(0f, 1f);
        pauseRt.pivot = new Vector2(0f, 1f);
        pauseRt.sizeDelta = new Vector2(110f, 110f);
        pauseRt.anchoredPosition = new Vector2(40f, -150f);
        TextMeshProUGUI pauseLabel = CreateText("Label", pauseGo.transform, "II", 56, Color.white);
        StretchFull(pauseLabel.rectTransform);
        pauseLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(pauseLabel);

        SettingsPanel settings = BuildSettingsPanel(canvas.transform);

        PausePanel pause = BuildPausePanel(canvas.transform);
        pause.settings = settings;
        if (demolitionGo != null) pause.demolition = demolitionGo.GetComponent<DemolitionController>();

        PauseButton pauseButtonComponent = pauseGo.AddComponent<PauseButton>();
        pauseButtonComponent.pausePanel = pause;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static SettingsPanel BuildSettingsPanel(Transform canvas)
    {
        GameObject root = new GameObject("SettingsPanel", typeof(RectTransform));
        root.transform.SetParent(canvas, false);
        StretchFull(root.GetComponent<RectTransform>());

        CanvasGroup backdrop = BuildBackdrop(root.transform);
        RectTransform panel = BuildPanel(root.transform, new Vector2(820f, 920f));

        TextMeshProUGUI title = CreateText("Title", panel, "SETTINGS", 80, Color.white);
        SetCentered(title.rectTransform, new Vector2(720f, 120f), new Vector2(0f, 360f));
        title.fontStyle = FontStyles.Bold;
        ApplyOutline(title);

        Button sfxButton = BuildToggle(panel, "SfxButton", "SFX: ON", new Vector2(0f, 170f), out TextMeshProUGUI sfxLabel);
        Button musicButton = BuildToggle(panel, "MusicButton", "Music: ON", new Vector2(0f, 30f), out TextMeshProUGUI musicLabel);
        Button hapticButton = BuildToggle(panel, "HapticButton", "Haptics: ON", new Vector2(0f, -110f), out TextMeshProUGUI hapticLabel);

        GameObject closeGo = CreateButton("CloseButton", panel, ColorSecondary, out Button closeButton);
        SetCentered(closeGo.GetComponent<RectTransform>(), new Vector2(420f, 140f), new Vector2(0f, -340f));
        TextMeshProUGUI closeLabel = CreateText("Label", closeGo.transform, "CLOSE", 54, Color.white);
        StretchFull(closeLabel.rectTransform);
        closeLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(closeLabel);

        SettingsPanel panelScript = root.AddComponent<SettingsPanel>();
        panelScript.backdrop = backdrop;
        panelScript.panel = panel;
        panelScript.closeButton = closeButton;
        panelScript.sfxButton = sfxButton;
        panelScript.sfxLabel = sfxLabel;
        panelScript.musicButton = musicButton;
        panelScript.musicLabel = musicLabel;
        panelScript.hapticButton = hapticButton;
        panelScript.hapticLabel = hapticLabel;

        root.SetActive(false);
        return panelScript;
    }

    private static PausePanel BuildPausePanel(Transform canvas)
    {
        GameObject root = new GameObject("PausePanel", typeof(RectTransform));
        root.transform.SetParent(canvas, false);
        StretchFull(root.GetComponent<RectTransform>());

        CanvasGroup backdrop = BuildBackdrop(root.transform);
        RectTransform panel = BuildPanel(root.transform, new Vector2(760f, 980f));

        TextMeshProUGUI title = CreateText("Title", panel, "PAUSED", 90, ColorAccent);
        SetCentered(title.rectTransform, new Vector2(680f, 130f), new Vector2(0f, 380f));
        title.fontStyle = FontStyles.Bold;
        ApplyOutline(title);

        Button resumeButton = BuildMenuButton(panel, "ResumeButton", "RESUME", ColorPrimary, new Vector2(0f, 180f));
        Button settingsButton = BuildMenuButton(panel, "SettingsButton", "SETTINGS", ColorSecondary, new Vector2(0f, 40f));
        Button restartButton = BuildMenuButton(panel, "RestartButton", "RESTART", ColorSecondary, new Vector2(0f, -100f));
        Button menuButton = BuildMenuButton(panel, "MenuButton", "MENU", ColorSecondary, new Vector2(0f, -240f));

        PausePanel panelScript = root.AddComponent<PausePanel>();
        panelScript.backdrop = backdrop;
        panelScript.panel = panel;
        panelScript.resumeButton = resumeButton;
        panelScript.settingsButton = settingsButton;
        panelScript.restartButton = restartButton;
        panelScript.menuButton = menuButton;

        root.SetActive(false);
        return panelScript;
    }

    private static CanvasGroup BuildBackdrop(Transform parent)
    {
        GameObject go = CreateImage("Backdrop", parent, new Color(0f, 0f, 0f, 1f));
        StretchFull(go.GetComponent<RectTransform>());
        CanvasGroup cg = go.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        return cg;
    }

    private static RectTransform BuildPanel(Transform parent, Vector2 size)
    {
        GameObject go = CreateImage("Panel", parent, ColorPanel);
        RectTransform rt = go.GetComponent<RectTransform>();
        SetCentered(rt, size, Vector2.zero);
        return rt;
    }

    private static Button BuildToggle(RectTransform panel, string name, string text, Vector2 pos, out TextMeshProUGUI label)
    {
        GameObject go = CreateButton(name, panel, ColorRow, out Button button);
        SetCentered(go.GetComponent<RectTransform>(), new Vector2(660f, 120f), pos);
        label = CreateText("Label", go.transform, text, 50, Color.white);
        StretchFull(label.rectTransform);
        label.fontStyle = FontStyles.Bold;
        ApplyOutline(label);
        return button;
    }

    private static Button BuildMenuButton(RectTransform panel, string name, string text, Color color, Vector2 pos)
    {
        GameObject go = CreateButton(name, panel, color, out Button button);
        SetCentered(go.GetComponent<RectTransform>(), new Vector2(560f, 120f), pos);
        TextMeshProUGUI label = CreateText("Label", go.transform, text, 56, Color.white);
        StretchFull(label.rectTransform);
        label.fontStyle = FontStyles.Bold;
        ApplyOutline(label);
        return button;
    }

    private static void DestroyIfExists(string path)
    {
        GameObject go = GameObject.Find(path);
        if (go != null) Object.DestroyImmediate(go);
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

    private static void SetCentered(RectTransform rt, Vector2 size, Vector2 pos)
    {
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;
    }

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
