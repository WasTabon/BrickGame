using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BrickGameSetup_Iteration15
{
    private const string MainMenuPath = "Assets/BrickGame/Scenes/MainMenu.unity";
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";
    private const string BattlePath = "Assets/BrickGame/Scenes/Battle.unity";

    private static readonly Color ColorAccent = HexColor("F5A623");
    private static readonly Color ColorPanel = HexColor("232347");
    private static readonly Color ColorRow = HexColor("2E3358");
    private static readonly Color ColorSecondary = HexColor("596089");

    [MenuItem("BrickGame/Setup Iteration 15 (Daily + Achievements)")]
    public static void Setup()
    {
        SetupMainMenu();
        AddToastToScene(GamePath);
        AddToastToScene(BattlePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 15 setup complete. Daily challenge + achievements added.");
    }

    private static void SetupMainMenu()
    {
        EditorSceneManager.OpenScene(MainMenuPath, OpenSceneMode.Single);

        GameObject canvas = GameObject.Find("Canvas");
        GameObject safeArea = GameObject.Find("Canvas/SafeArea");
        GameObject controllerGo = GameObject.Find("MainMenuController");
        if (canvas == null || safeArea == null || controllerGo == null)
        {
            Debug.LogWarning("MainMenu objects not found. Run earlier setups first.");
            return;
        }

        DestroyIfExists("Canvas/SafeArea/DailyButton");
        DestroyIfExists("Canvas/SafeArea/AchievementsButton");
        DestroyIfExists("Canvas/AchievementsPanel");
        DestroyIfExists("Canvas/AchievementToast");

        MainMenuController controller = controllerGo.GetComponent<MainMenuController>();

        GameObject dailyGo = CreateButton("DailyButton", safeArea.transform, ColorAccent, out Button dailyButton);
        PlaceCenter(dailyGo, new Vector2(440f, 150f), new Vector2(0f, -520f));
        TextMeshProUGUI dailyLabel = CreateText("Label", dailyGo.transform, "DAILY", 54, Color.white);
        StretchFull(dailyLabel.rectTransform);
        dailyLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(dailyLabel);

        GameObject achGo = CreateButton("AchievementsButton", safeArea.transform, ColorSecondary, out Button achButton);
        PlaceCenter(achGo, new Vector2(440f, 150f), new Vector2(0f, -700f));
        TextMeshProUGUI achLabel = CreateText("Label", achGo.transform, "AWARDS", 50, Color.white);
        StretchFull(achLabel.rectTransform);
        achLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(achLabel);

        AchievementsPanel panel = BuildAchievementsPanel(canvas.transform);
        BuildToast(canvas.transform);

        controller.dailyButton = dailyButton;
        controller.dailyLabel = dailyLabel;
        controller.achievementsButton = achButton;
        controller.achievementsPanel = panel;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static AchievementsPanel BuildAchievementsPanel(Transform canvas)
    {
        GameObject root = new GameObject("AchievementsPanel", typeof(RectTransform));
        root.transform.SetParent(canvas, false);
        StretchFull(root.GetComponent<RectTransform>());

        GameObject backdropGo = CreateImage("Backdrop", root.transform, new Color(0f, 0f, 0f, 1f));
        StretchFull(backdropGo.GetComponent<RectTransform>());
        CanvasGroup backdrop = backdropGo.AddComponent<CanvasGroup>();
        backdrop.alpha = 0f;

        GameObject panelGo = CreateImage("Panel", root.transform, ColorPanel);
        RectTransform panelRt = panelGo.GetComponent<RectTransform>();
        SetCentered(panelRt, new Vector2(900f, 1280f), Vector2.zero);

        TextMeshProUGUI title = CreateText("Title", panelGo.transform, "AWARDS", 80, ColorAccent);
        SetCentered(title.rectTransform, new Vector2(800f, 120f), new Vector2(0f, 540f));
        title.fontStyle = FontStyles.Bold;
        ApplyOutline(title);

        AchievementsPanel.Row[] rows = new AchievementsPanel.Row[Achievements.All.Length];
        float startY = 380f;
        float step = 160f;
        for (int i = 0; i < Achievements.All.Length; i++)
        {
            Achievements.Def def = Achievements.All[i];
            float y = startY - i * step;

            GameObject rowBg = CreateImage("Row" + i, panelGo.transform, ColorRow);
            SetCentered(rowBg.GetComponent<RectTransform>(), new Vector2(820f, 140f), new Vector2(0f, y));

            TextMeshProUGUI nameText = CreateText("Name", rowBg.transform, def.name, 42, Color.white);
            SetCentered(nameText.rectTransform, new Vector2(560f, 60f), new Vector2(-110f, 25f));
            nameText.alignment = TextAlignmentOptions.Left;
            nameText.fontStyle = FontStyles.Bold;

            TextMeshProUGUI descText = CreateText("Desc", rowBg.transform, def.desc, 30, new Color(1f, 1f, 1f, 0.6f));
            SetCentered(descText.rectTransform, new Vector2(560f, 50f), new Vector2(-110f, -32f));
            descText.alignment = TextAlignmentOptions.Left;

            TextMeshProUGUI statusText = CreateText("Status", rowBg.transform, "LOCKED", 34, new Color(1f, 1f, 1f, 0.35f));
            SetCentered(statusText.rectTransform, new Vector2(240f, 80f), new Vector2(270f, 0f));
            statusText.fontStyle = FontStyles.Bold;

            rows[i] = new AchievementsPanel.Row { key = def.key, statusText = statusText };
        }

        GameObject closeGo = CreateButton("CloseButton", panelGo.transform, ColorSecondary, out Button closeButton);
        SetCentered(closeGo.GetComponent<RectTransform>(), new Vector2(420f, 140f), new Vector2(0f, -540f));
        TextMeshProUGUI closeLabel = CreateText("Label", closeGo.transform, "CLOSE", 54, Color.white);
        StretchFull(closeLabel.rectTransform);
        closeLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(closeLabel);

        AchievementsPanel script = root.AddComponent<AchievementsPanel>();
        script.backdrop = backdrop;
        script.panel = panelRt;
        script.closeButton = closeButton;
        script.rows = rows;

        root.SetActive(false);
        return script;
    }

    private static void AddToastToScene(string scenePath)
    {
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogWarning("Canvas not found in " + scenePath);
            return;
        }

        DestroyIfExists("Canvas/AchievementToast");
        BuildToast(canvas.transform);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void BuildToast(Transform canvas)
    {
        GameObject root = new GameObject("AchievementToast", typeof(RectTransform));
        root.transform.SetParent(canvas, false);
        StretchFull(root.GetComponent<RectTransform>());

        GameObject panelGo = CreateImage("Panel", root.transform, ColorAccent);
        RectTransform panelRt = panelGo.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 1f);
        panelRt.anchorMax = new Vector2(0.5f, 1f);
        panelRt.pivot = new Vector2(0.5f, 1f);
        panelRt.sizeDelta = new Vector2(780f, 150f);
        panelRt.anchoredPosition = new Vector2(0f, -120f);

        TextMeshProUGUI label = CreateText("Label", panelGo.transform, "Unlocked", 44, Color.white);
        StretchFull(label.rectTransform);
        label.fontStyle = FontStyles.Bold;
        ApplyOutline(label);

        AchievementToast toast = root.AddComponent<AchievementToast>();
        toast.panel = panelRt;
        toast.label = label;
    }

    private static void PlaceCenter(GameObject go, Vector2 size, Vector2 pos)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        SetCentered(rt, size, pos);
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
