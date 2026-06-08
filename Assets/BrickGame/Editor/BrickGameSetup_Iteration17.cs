using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BrickGameSetup_Iteration17
{
    private const string BattlePath = "Assets/BrickGame/Scenes/Battle.unity";

    private static readonly Color ColorAccent = HexColor("F5A623");
    private static readonly Color ColorPanel = HexColor("232347");
    private static readonly Color ColorCard = HexColor("2E3358");
    private static readonly Color ColorCardName = HexColor("4A90E2");

    [MenuItem("BrickGame/Setup Iteration 17 (Battle Buffs)")]
    public static void Setup()
    {
        EditorSceneManager.OpenScene(BattlePath, OpenSceneMode.Single);

        GameObject canvas = GameObject.Find("Canvas");
        BattleController controller = Object.FindObjectOfType<BattleController>();
        if (canvas == null || controller == null)
        {
            Debug.LogWarning("Battle Canvas / BattleController not found. Run Setup Iteration 5 first.");
            return;
        }

        DestroyIfExists("Canvas/BuffPicker");

        BattleBuffPicker picker = BuildPicker(canvas.transform);
        controller.buffPicker = picker;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 17 setup complete. Battle buff picker added.");
    }

    private static BattleBuffPicker BuildPicker(Transform canvas)
    {
        GameObject root = new GameObject("BuffPicker", typeof(RectTransform));
        root.transform.SetParent(canvas, false);
        StretchFull(root.GetComponent<RectTransform>());

        GameObject backdropGo = CreateImage("Backdrop", root.transform, new Color(0f, 0f, 0f, 1f));
        StretchFull(backdropGo.GetComponent<RectTransform>());
        CanvasGroup backdrop = backdropGo.AddComponent<CanvasGroup>();
        backdrop.alpha = 0f;

        GameObject panelGo = CreateImage("Panel", root.transform, ColorPanel);
        RectTransform panelRt = panelGo.GetComponent<RectTransform>();
        SetCentered(panelRt, new Vector2(840f, 1120f), Vector2.zero);

        TextMeshProUGUI title = CreateText("Title", panelGo.transform, "CHOOSE A BUFF", 70, ColorAccent);
        SetCentered(title.rectTransform, new Vector2(780f, 110f), new Vector2(0f, 470f));
        title.fontStyle = FontStyles.Bold;
        ApplyOutline(title);

        BattleBuffPicker.Card[] cards = new BattleBuffPicker.Card[3];
        float startY = 270f;
        float step = 250f;
        for (int i = 0; i < 3; i++)
        {
            float y = startY - i * step;
            cards[i] = BuildCard(panelRt, i, new Vector2(0f, y));
        }

        BattleBuffPicker picker = root.AddComponent<BattleBuffPicker>();
        picker.backdrop = backdrop;
        picker.panel = panelRt;
        picker.title = title;
        picker.cards = cards;

        root.SetActive(false);
        return picker;
    }

    private static BattleBuffPicker.Card BuildCard(RectTransform panel, int index, Vector2 pos)
    {
        GameObject cardGo = CreateButton("Card" + index, panel, ColorCard, out Button button);
        RectTransform rt = cardGo.GetComponent<RectTransform>();
        SetCentered(rt, new Vector2(720f, 220f), pos);

        TextMeshProUGUI nameText = CreateText("Name", cardGo.transform, "BUFF", 48, ColorCardName);
        SetCentered(nameText.rectTransform, new Vector2(660f, 70f), new Vector2(0f, 38f));
        nameText.fontStyle = FontStyles.Bold;
        ApplyOutline(nameText);

        TextMeshProUGUI descText = CreateText("Desc", cardGo.transform, "desc", 34, new Color(1f, 1f, 1f, 0.8f));
        SetCentered(descText.rectTransform, new Vector2(660f, 60f), new Vector2(0f, -38f));

        return new BattleBuffPicker.Card
        {
            button = button,
            root = rt,
            nameText = nameText,
            descText = descText
        };
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
