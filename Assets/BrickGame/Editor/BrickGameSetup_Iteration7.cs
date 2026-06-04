using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BrickGameSetup_Iteration7
{
    private const string MainMenuPath = "Assets/BrickGame/Scenes/MainMenu.unity";
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";

    private static readonly Color ColorPrimary = HexColor("4A90E2");
    private static readonly Color ColorAccent = HexColor("F5A623");
    private static readonly Color ColorPanel = HexColor("232347");
    private static readonly Color ColorRow = HexColor("2E3358");
    private static readonly Color ColorSecondary = HexColor("596089");

    private static readonly string[] RowKeys = { "Upg_Force", "Upg_Length", "Upg_Speed", "Upg_Magnet" };
    private static readonly string[] RowNames = { "Pull Force", "Rope Length", "Pull Speed", "Pit Magnet" };

    [MenuItem("BrickGame/Setup Iteration 7 (Upgrades)")]
    public static void Setup()
    {
        SetupMainMenu();
        SetupGame();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 7 setup complete. Upgrades panel + economy wired.");
    }

    private static void SetupMainMenu()
    {
        EditorSceneManager.OpenScene(MainMenuPath, OpenSceneMode.Single);

        GameObject canvas = GameObject.Find("Canvas");
        GameObject safeArea = GameObject.Find("Canvas/SafeArea");
        GameObject controllerGo = GameObject.Find("MainMenuController");

        if (canvas == null || safeArea == null || controllerGo == null)
        {
            Debug.LogWarning("MainMenu objects not found. Run Setup Iteration 1 first.");
            return;
        }

        DestroyIfExists("Canvas/SafeArea/UpgradesButton");
        DestroyIfExists("Canvas/UpgradesPanel");

        MainMenuController controller = controllerGo.GetComponent<MainMenuController>();

        GameObject upgGo = CreateButton("UpgradesButton", safeArea.transform, ColorAccent, out Button upgButton);
        RectTransform upgRt = upgGo.GetComponent<RectTransform>();
        upgRt.anchorMin = new Vector2(0.5f, 0.5f);
        upgRt.anchorMax = new Vector2(0.5f, 0.5f);
        upgRt.pivot = new Vector2(0.5f, 0.5f);
        upgRt.sizeDelta = new Vector2(440f, 150f);
        upgRt.anchoredPosition = new Vector2(0f, -340f);
        TextMeshProUGUI upgLabel = CreateText("Label", upgGo.transform, "UPGRADES", 56, Color.white);
        StretchFull(upgLabel.rectTransform);
        upgLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(upgLabel);

        UpgradesPanel panel = CreateUpgradesPanel(canvas.transform);

        controller.upgradesButton = upgButton;
        controller.upgradesPanel = panel;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static UpgradesPanel CreateUpgradesPanel(Transform canvas)
    {
        GameObject root = new GameObject("UpgradesPanel", typeof(RectTransform));
        root.transform.SetParent(canvas, false);
        StretchFull(root.GetComponent<RectTransform>());

        GameObject backdropGo = CreateImage("Backdrop", root.transform, new Color(0f, 0f, 0f, 1f));
        StretchFull(backdropGo.GetComponent<RectTransform>());
        CanvasGroup backdropCg = backdropGo.AddComponent<CanvasGroup>();
        backdropCg.alpha = 0f;

        GameObject panelGo = CreateImage("Panel", root.transform, ColorPanel);
        RectTransform panelRt = panelGo.GetComponent<RectTransform>();
        SetCentered(panelRt, new Vector2(920f, 1240f), Vector2.zero);

        TextMeshProUGUI title = CreateText("Title", panelGo.transform, "UPGRADES", 80, Color.white);
        SetCentered(title.rectTransform, new Vector2(820f, 120f), new Vector2(0f, 520f));
        title.fontStyle = FontStyles.Bold;
        ApplyOutline(title);

        GameObject bankIcon = CreateImage("BankIcon", panelGo.transform, ColorAccent);
        SetCentered(bankIcon.GetComponent<RectTransform>(), new Vector2(60f, 60f), new Vector2(-90f, 410f));
        TextMeshProUGUI bankText = CreateText("BankText", panelGo.transform, "0", 60, ColorAccent);
        SetCentered(bankText.rectTransform, new Vector2(300f, 90f), new Vector2(120f, 410f));
        bankText.alignment = TextAlignmentOptions.Left;
        bankText.fontStyle = FontStyles.Bold;
        ApplyOutline(bankText);

        UpgradesPanel.UpgradeRow[] rows = new UpgradesPanel.UpgradeRow[RowKeys.Length];
        float startY = 240f;
        float step = 170f;
        for (int i = 0; i < RowKeys.Length; i++)
        {
            float y = startY - i * step;

            GameObject rowBg = CreateImage("Row" + i, panelGo.transform, ColorRow);
            SetCentered(rowBg.GetComponent<RectTransform>(), new Vector2(820f, 150f), new Vector2(0f, y));

            TextMeshProUGUI nameText = CreateText("Name", rowBg.transform, RowNames[i], 46, Color.white);
            RectTransform nameRt = nameText.rectTransform;
            SetCentered(nameRt, new Vector2(360f, 90f), new Vector2(-220f, 22f));
            nameText.alignment = TextAlignmentOptions.Left;
            nameText.fontStyle = FontStyles.Bold;

            TextMeshProUGUI levelText = CreateText("Level", rowBg.transform, "Lv 0/5", 38, new Color(1f, 1f, 1f, 0.7f));
            RectTransform lvlRt = levelText.rectTransform;
            SetCentered(lvlRt, new Vector2(360f, 70f), new Vector2(-220f, -38f));
            levelText.alignment = TextAlignmentOptions.Left;

            GameObject buyGo = CreateButton("Buy", rowBg.transform, ColorPrimary, out Button buyButton);
            SetCentered(buyGo.GetComponent<RectTransform>(), new Vector2(240f, 110f), new Vector2(260f, 0f));
            TextMeshProUGUI costText = CreateText("Cost", buyGo.transform, "50", 44, Color.white);
            StretchFull(costText.rectTransform);
            costText.fontStyle = FontStyles.Bold;
            ApplyOutline(costText);

            UpgradesPanel.UpgradeRow row = new UpgradesPanel.UpgradeRow
            {
                key = RowKeys[i],
                nameText = nameText,
                levelText = levelText,
                costText = costText,
                buyButton = buyButton
            };
            rows[i] = row;
        }

        GameObject closeGo = CreateButton("CloseButton", panelGo.transform, ColorSecondary, out Button closeButton);
        SetCentered(closeGo.GetComponent<RectTransform>(), new Vector2(420f, 150f), new Vector2(0f, -520f));
        TextMeshProUGUI closeLabel = CreateText("Label", closeGo.transform, "CLOSE", 56, Color.white);
        StretchFull(closeLabel.rectTransform);
        closeLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(closeLabel);

        UpgradesPanel panel = root.AddComponent<UpgradesPanel>();
        panel.backdrop = backdropCg;
        panel.panel = panelRt;
        panel.bankText = bankText;
        panel.closeButton = closeButton;
        panel.rows = rows;

        root.SetActive(false);
        return panel;
    }

    private static void SetupGame()
    {
        EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);

        GameObject ropeGo = GameObject.Find("Rope");
        GameObject demolitionGo = GameObject.Find("DemolitionController");

        if (ropeGo == null || demolitionGo == null)
        {
            Debug.LogWarning("Game objects not found. Run Setup Iteration 2 first.");
            return;
        }

        DestroyIfExists("UpgradeApplier");

        GameObject applierGo = new GameObject("UpgradeApplier");
        UpgradeApplier applier = applierGo.AddComponent<UpgradeApplier>();
        applier.rope = ropeGo.GetComponent<Rope>();
        applier.demolition = demolitionGo.GetComponent<DemolitionController>();
        applier.baseForce = 60f;
        applier.baseRamp = 0.5f;
        applier.basePullDuration = 1.0f;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
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
