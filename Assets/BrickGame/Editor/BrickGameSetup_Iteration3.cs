using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BrickGameSetup_Iteration3
{
    private const string RootFolder = "Assets/BrickGame";
    private const string TexFolder = "Assets/BrickGame/Textures";
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";
    private const string StarPath = "Assets/BrickGame/Textures/star.png";

    private static readonly Color ColorPrimary = HexColor("4A90E2");
    private static readonly Color ColorAccent = HexColor("F5A623");
    private static readonly Color ColorPanel = HexColor("232347");
    private static readonly Color ColorSecondary = HexColor("596089");

    private static Sprite uiSprite;
    private static Sprite starSprite;

    [MenuItem("BrickGame/Setup Iteration 3 (Result + Stars)")]
    public static void Setup()
    {
        EnsureFolders();
        GenerateStarSprite();

        uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        starSprite = AssetDatabase.LoadAssetAtPath<Sprite>(StarPath);

        EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);

        Cleanup();

        GameObject canvas = GameObject.Find("Canvas");
        GameObject safeArea = GameObject.Find("Canvas/SafeArea");
        GameObject hudGo = GameObject.Find("GameHUD");
        GameObject pitGo = GameObject.Find("CollectionPit");
        GameObject demolitionGo = GameObject.Find("DemolitionController");

        if (canvas == null || safeArea == null || hudGo == null || pitGo == null || demolitionGo == null)
        {
            Debug.LogWarning("Required objects from Iteration 2 not found. Run Setup Iteration 2 first.");
            return;
        }

        GameHUDController hud = hudGo.GetComponent<GameHUDController>();
        DemolitionController demolition = demolitionGo.GetComponent<DemolitionController>();

        Button collectButton = CreateCollectButton(safeArea.transform);
        hud.collectButton = collectButton;

        ResultPopup popup = CreateResultPopup(canvas.transform);

        GameObject flowGo = new GameObject("LevelFlowController");
        LevelFlowController flow = flowGo.AddComponent<LevelFlowController>();
        flow.popup = popup;
        flow.hud = hud;
        flow.demolition = demolition;
        flow.collectButton = collectButton;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 3 setup complete. Collect button + result popup added.");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(TexFolder))
            AssetDatabase.CreateFolder(RootFolder, "Textures");
    }

    private static void Cleanup()
    {
        DestroyIfExists("LevelFlowController");
        DestroyIfExists("Canvas/ResultPopup");
        DestroyIfExists("Canvas/SafeArea/CollectButton");
    }

    private static void DestroyIfExists(string path)
    {
        GameObject go = GameObject.Find(path);
        if (go != null) Object.DestroyImmediate(go);
    }

    private static Button CreateCollectButton(Transform parent)
    {
        GameObject go = CreateButton("CollectButton", parent, ColorPrimary, out Button button);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = new Vector2(540f, 160f);
        rt.anchoredPosition = new Vector2(0f, 60f);

        TextMeshProUGUI label = CreateText("Label", go.transform, "COLLECT", 70, Color.white);
        StretchFull(label.rectTransform);
        label.alignment = TextAlignmentOptions.Center;
        label.fontStyle = FontStyles.Bold;
        ApplyOutline(label);

        return button;
    }

    private static ResultPopup CreateResultPopup(Transform canvas)
    {
        GameObject root = new GameObject("ResultPopup", typeof(RectTransform));
        root.transform.SetParent(canvas, false);
        StretchFull(root.GetComponent<RectTransform>());

        GameObject backdropGo = CreateImage("Backdrop", root.transform, uiSprite, new Color(0f, 0f, 0f, 1f));
        StretchFull(backdropGo.GetComponent<RectTransform>());
        CanvasGroup backdropCg = backdropGo.AddComponent<CanvasGroup>();
        backdropCg.alpha = 0f;

        GameObject panelGo = CreateImage("Panel", root.transform, uiSprite, ColorPanel);
        RectTransform panelRt = panelGo.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.sizeDelta = new Vector2(840f, 1000f);
        panelRt.anchoredPosition = Vector2.zero;

        TextMeshProUGUI title = CreateText("Title", panelGo.transform, "RESULT", 90, Color.white);
        RectTransform titleRt = title.rectTransform;
        SetCentered(titleRt, new Vector2(760f, 140f), new Vector2(0f, 390f));
        title.fontStyle = FontStyles.Bold;
        ApplyOutline(title);

        Image[] stars = new Image[3];
        float[] xs = { -210f, 0f, 210f };
        for (int i = 0; i < 3; i++)
        {
            GameObject starGo = CreateImage("Star" + i, panelGo.transform, starSprite, ColorAccent);
            RectTransform srt = starGo.GetComponent<RectTransform>();
            SetCentered(srt, new Vector2(170f, 170f), new Vector2(xs[i], 200f));
            if (i == 1) srt.anchoredPosition = new Vector2(0f, 240f);
            stars[i] = starGo.GetComponent<Image>();
            stars[i].preserveAspect = true;
        }

        TextMeshProUGUI count = CreateText("Count", panelGo.transform, "0 / 0", 120, ColorAccent);
        RectTransform countRt = count.rectTransform;
        SetCentered(countRt, new Vector2(760f, 180f), new Vector2(0f, -60f));
        count.fontStyle = FontStyles.Bold;
        ApplyOutline(count);

        GameObject retryGo = CreateButton("RetryButton", panelGo.transform, ColorSecondary, out Button retryButton);
        RectTransform retryRt = retryGo.GetComponent<RectTransform>();
        SetCentered(retryRt, new Vector2(340f, 150f), new Vector2(-200f, -360f));
        TextMeshProUGUI retryLabel = CreateText("Label", retryGo.transform, "RETRY", 56, Color.white);
        StretchFull(retryLabel.rectTransform);
        retryLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(retryLabel);

        GameObject battleGo = CreateButton("BattleButton", panelGo.transform, ColorAccent, out Button battleButton);
        RectTransform battleRt = battleGo.GetComponent<RectTransform>();
        SetCentered(battleRt, new Vector2(340f, 150f), new Vector2(200f, -360f));
        TextMeshProUGUI battleLabel = CreateText("Label", battleGo.transform, "BATTLE", 56, Color.white);
        StretchFull(battleLabel.rectTransform);
        battleLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(battleLabel);

        ResultPopup popup = root.AddComponent<ResultPopup>();
        popup.backdrop = backdropCg;
        popup.panel = panelRt;
        popup.titleText = title;
        popup.countText = count;
        popup.stars = stars;
        popup.retryButton = retryButton;
        popup.battleButton = battleButton;

        root.SetActive(false);
        return popup;
    }

    private static GameObject CreateImage(string name, Transform parent, Sprite sprite, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.type = Image.Type.Sliced;
        return go;
    }

    private static GameObject CreateButton(string name, Transform parent, Color color, out Button button)
    {
        GameObject go = CreateImage(name, parent, uiSprite, color);
        Image image = go.GetComponent<Image>();
        button = go.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock cb = button.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = Color.white;
        cb.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        cb.disabledColor = new Color(1f, 1f, 1f, 0.5f);
        button.colors = cb;
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

    private static void GenerateStarSprite()
    {
        if (File.Exists(StarPath)) return;

        int size = 96;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float outer = size * 0.47f;
        float inner = size * 0.2f;

        Vector2[] pts = new Vector2[10];
        for (int i = 0; i < 10; i++)
        {
            float ang = Mathf.PI / 2f + i * Mathf.PI / 5f;
            float rad = (i % 2 == 0) ? outer : inner;
            pts[i] = center + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * rad;
        }

        Color clear = new Color(1f, 1f, 1f, 0f);
        Color fill = Color.white;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                tex.SetPixel(x, y, PointInPolygon(p, pts) ? fill : clear);
            }
        }
        tex.Apply();
        File.WriteAllBytes(StarPath, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(StarPath);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(StarPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.SaveAndReimport();
    }

    private static bool PointInPolygon(Vector2 p, Vector2[] poly)
    {
        bool inside = false;
        int n = poly.Length;
        int j = n - 1;
        for (int i = 0; i < n; i++)
        {
            if (((poly[i].y > p.y) != (poly[j].y > p.y)) &&
                (p.x < (poly[j].x - poly[i].x) * (p.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x))
            {
                inside = !inside;
            }
            j = i;
        }
        return inside;
    }

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
