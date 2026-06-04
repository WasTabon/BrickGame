using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BrickGameSetup_Iteration6
{
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";
    private const string SquarePath = "Assets/BrickGame/Textures/square.png";
    private const string BrickPhysPath = "Assets/BrickGame/Textures/BrickPhysics.physicsMaterial2D";

    [MenuItem("BrickGame/Setup Iteration 6 (Levels)")]
    public static void Setup()
    {
        Sprite squareSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SquarePath);
        PhysicsMaterial2D brickPhysics = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(BrickPhysPath);
        if (squareSprite == null || brickPhysics == null)
        {
            Debug.LogWarning("square.png / BrickPhysics not found. Run Setup Iteration 2 first.");
            return;
        }

        EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);

        CleanupBaked();

        GameObject truck = GameObject.Find("Truck");
        GameObject rope = GameObject.Find("Rope");
        GameObject hudGo = GameObject.Find("GameHUD");
        GameObject demolitionGo = GameObject.Find("DemolitionController");
        GameObject flowGo = GameObject.Find("LevelFlowController");
        GameObject safeArea = GameObject.Find("Canvas/SafeArea");

        if (truck == null || rope == null || hudGo == null || demolitionGo == null || flowGo == null || safeArea == null)
        {
            Debug.LogWarning("Required objects not found. Run Setup Iteration 2 and 3 first.");
            return;
        }

        GameHUDController hud = hudGo.GetComponent<GameHUDController>();

        SetupHudLevelLabel(safeArea, hud);

        GameObject lmGo = GameObject.Find("LevelManager");
        if (lmGo != null) Object.DestroyImmediate(lmGo);

        lmGo = new GameObject("LevelManager");
        LevelManager lm = lmGo.AddComponent<LevelManager>();
        lm.squareSprite = squareSprite;
        lm.brickPhysics = brickPhysics;
        lm.hud = hud;
        lm.cam = Camera.main;
        lm.levels = BuildLevels();

        LevelFlowController flow = flowGo.GetComponent<LevelFlowController>();
        flow.levelManager = lm;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 6 setup complete. Tower/pits now built at runtime by LevelManager.");
    }

    private static void CleanupBaked()
    {
        DestroyAll("Building");
        DestroyAll("CollectionPit");
        DestroyAll("PitVisual");
    }

    private static void DestroyAll(string name)
    {
        GameObject go = GameObject.Find(name);
        while (go != null)
        {
            Object.DestroyImmediate(go);
            go = GameObject.Find(name);
        }
    }

    private static void SetupHudLevelLabel(GameObject safeArea, GameHUDController hud)
    {
        GameObject counter = GameObject.Find("Canvas/SafeArea/BrickCounter");
        if (counter != null)
        {
            RectTransform crt = counter.GetComponent<RectTransform>();
            crt.anchoredPosition = new Vector2(0f, -120f);
        }

        GameObject existing = GameObject.Find("Canvas/SafeArea/LevelLabel");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject go = new GameObject("LevelLabel", typeof(RectTransform));
        go.transform.SetParent(safeArea.transform, false);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.text = "Level 1";
        text.fontSize = 64;
        text.color = HexColor("F5A623");
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;
        text.raycastTarget = false;
        TMP_FontAsset font = TMP_Settings.defaultFontAsset;
        if (font != null) text.font = font;
        ApplyOutline(text);

        RectTransform rt = text.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(700f, 100f);
        rt.anchoredPosition = new Vector2(0f, -30f);

        hud.levelText = text;
    }

    private static List<LevelDef> BuildLevels()
    {
        return new List<LevelDef>
        {
            new LevelDef { rows = 16, columns = 2, heavyEvery = 0, pitCount = 1 },
            new LevelDef { rows = 18, columns = 2, heavyEvery = 0, pitCount = 1 },
            new LevelDef { rows = 20, columns = 2, heavyEvery = 6, pitCount = 1 },
            new LevelDef { rows = 22, columns = 3, heavyEvery = 6, pitCount = 2 },
            new LevelDef { rows = 26, columns = 3, heavyEvery = 4, pitCount = 2 },
        };
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
