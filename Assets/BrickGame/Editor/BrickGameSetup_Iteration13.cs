using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BrickGameSetup_Iteration13
{
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";
    private static readonly Color ColorAccent = HexColor("F5A623");

    [MenuItem("BrickGame/Setup Iteration 13 (Puzzle Layer)")]
    public static void Setup()
    {
        EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);

        GameObject safeArea = GameObject.Find("Canvas/SafeArea");
        if (safeArea == null)
        {
            Debug.LogWarning("Canvas/SafeArea not found. Run earlier setups first.");
            return;
        }

        GameHUDController hud = Object.FindObjectOfType<GameHUDController>();
        LevelManager lm = Object.FindObjectOfType<LevelManager>();
        DemolitionController demolition = Object.FindObjectOfType<DemolitionController>();
        LevelFlowController flow = Object.FindObjectOfType<LevelFlowController>();

        if (hud == null || lm == null || demolition == null || flow == null)
        {
            Debug.LogWarning("Required components not found. Run earlier setups first.");
            return;
        }

        GameObject existing = GameObject.Find("Canvas/SafeArea/ThrowsText");
        if (existing != null) Object.DestroyImmediate(existing);

        TextMeshProUGUI throwsText = CreateText("ThrowsText", safeArea.transform, "Throws: 0", 48, ColorAccent);
        RectTransform rt = throwsText.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(620f, 80f);
        rt.anchoredPosition = new Vector2(0f, -270f);
        throwsText.fontStyle = FontStyles.Bold;
        ApplyOutline(throwsText);
        throwsText.gameObject.SetActive(false);

        hud.throwsText = throwsText;
        lm.demolition = demolition;
        demolition.hud = hud;
        demolition.flow = flow;

        lm.levels = BuildLevels();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 13 setup complete. Puzzle layer added (moving pit, throw limit, brittle/sticky).");
    }

    private static List<LevelDef> BuildLevels()
    {
        return new List<LevelDef>
        {
            new LevelDef { rows = 16, columns = 2, pitCount = 1 },
            new LevelDef { rows = 17, columns = 2, pitCount = 1, woodEvery = 5 },
            new LevelDef { rows = 18, columns = 2, pitCount = 1, stoneEvery = 6 },
            new LevelDef { rows = 19, columns = 2, pitCount = 1, stoneEvery = 6, woodEvery = 7, maxThrows = 6 },
            new LevelDef { rows = 20, columns = 2, pitCount = 2, pitMoveAmplitude = 1.2f, pitMoveSpeed = 1.2f },
            new LevelDef { rows = 21, columns = 3, pitCount = 2, stoneEvery = 6 },
            new LevelDef { rows = 22, columns = 3, pitCount = 2, iceRows = 3, brittleEvery = 7 },
            new LevelDef { rows = 22, columns = 3, pitCount = 2, stoneEvery = 6, iceRows = 3, stickyEvery = 6 },
            new LevelDef { rows = 23, columns = 3, pitCount = 2, bombEvery = 9, pitMoveAmplitude = 1.0f, pitMoveSpeed = 1.1f },
            new LevelDef { rows = 24, columns = 3, pitCount = 2, stoneEvery = 5, bombEvery = 9, maxThrows = 5 },
            new LevelDef { rows = 24, columns = 3, pitCount = 2, iceRows = 4, woodEvery = 7, stickyEvery = 5 },
            new LevelDef { rows = 25, columns = 3, pitCount = 2, stoneEvery = 5, bombEvery = 10, pitMoveAmplitude = 1.4f, pitMoveSpeed = 1.4f, maxThrows = 5 },
            new LevelDef { rows = 26, columns = 3, pitCount = 2, iceRows = 4, stoneEvery = 6, bombEvery = 11, brittleEvery = 9 },
            new LevelDef { rows = 26, columns = 3, pitCount = 2, stoneEvery = 5, woodEvery = 7, bombEvery = 9, stickyEvery = 6, maxThrows = 5 },
            new LevelDef { rows = 28, columns = 3, pitCount = 2, iceRows = 5, stoneEvery = 5, woodEvery = 8, bombEvery = 11, stickyEvery = 7, brittleEvery = 9, pitMoveAmplitude = 1.5f, pitMoveSpeed = 1.3f, maxThrows = 6 },
        };
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

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
