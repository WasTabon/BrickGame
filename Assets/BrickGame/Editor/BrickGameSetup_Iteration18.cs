using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BrickGameSetup_Iteration18
{
    private const string TexFolder = "Assets/BrickGame/Textures";
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";
    private const string BattlePath = "Assets/BrickGame/Scenes/Battle.unity";
    private const string MainMenuPath = "Assets/BrickGame/Scenes/MainMenu.unity";
    private const string LevelSelectPath = "Assets/BrickGame/Scenes/LevelSelect.unity";
    private const string DotPath = "Assets/BrickGame/Textures/dot.png";
    private const string SparkPath = "Assets/BrickGame/Textures/spark.png";
    private const string VignettePath = "Assets/BrickGame/Textures/vignette.png";

    private static readonly Color ColorAccent = HexColor("F5A623");

    [MenuItem("BrickGame/Setup Iteration 18 (VFX)")]
    public static void Setup()
    {
        EnsureFolders();
        Sprite spark = GetSpark();
        Sprite vignette = GenerateVignette();

        SetProjectileHitSprite(spark);

        SetupGame(vignette);
        SetupBattle(spark, vignette);
        AddVignetteOnly(MainMenuPath, vignette);
        AddVignetteOnly(LevelSelectPath, vignette);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 18 setup complete. VFX added (trail, muzzle, sparks, camera juice, pit glow, vignette).");
    }

    private static void SetupGame(Sprite vignette)
    {
        EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);

        Camera cam = Camera.main;
        if (cam != null && cam.GetComponent<CameraJuice>() == null)
        {
            cam.gameObject.AddComponent<CameraJuice>();
        }

        GameObject canvas = GameObject.Find("Canvas");
        GameObject safeArea = GameObject.Find("Canvas/SafeArea");
        GameHUDController hud = Object.FindObjectOfType<GameHUDController>();

        if (safeArea != null && hud != null)
        {
            GameObject existing = GameObject.Find("Canvas/SafeArea/ProgressBar");
            if (existing != null) Object.DestroyImmediate(existing);

            GameObject barBg = CreateImage("ProgressBar", safeArea.transform, new Color(0f, 0f, 0f, 0.35f));
            RectTransform bgRt = barBg.GetComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0.5f, 1f);
            bgRt.anchorMax = new Vector2(0.5f, 1f);
            bgRt.pivot = new Vector2(0.5f, 1f);
            bgRt.sizeDelta = new Vector2(620f, 26f);
            bgRt.anchoredPosition = new Vector2(0f, -210f);

            GameObject fillGo = CreateImage("Fill", barBg.transform, ColorAccent);
            Image fill = fillGo.GetComponent<Image>();
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 0f;
            StretchFull(fill.rectTransform);

            hud.progressFill = fill;
        }

        if (canvas != null) AddVignette(canvas.transform, vignette);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupBattle(Sprite spark, Sprite vignette)
    {
        EditorSceneManager.OpenScene(BattlePath, OpenSceneMode.Single);

        BattleController controller = Object.FindObjectOfType<BattleController>();
        if (controller != null && controller.muzzle != null)
        {
            MuzzleFlash mf = controller.muzzle.GetComponent<MuzzleFlash>();
            if (mf == null) mf = controller.muzzle.gameObject.AddComponent<MuzzleFlash>();
            mf.sprite = spark;
            controller.muzzleFlash = mf;
        }

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null) AddVignette(canvas.transform, vignette);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void AddVignetteOnly(string scenePath, Sprite vignette)
    {
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null) AddVignette(canvas.transform, vignette);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void AddVignette(Transform canvas, Sprite vignette)
    {
        Transform existing = canvas.Find("Vignette");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        GameObject go = new GameObject("Vignette", typeof(RectTransform));
        go.transform.SetParent(canvas, false);
        Image img = go.AddComponent<Image>();
        img.sprite = vignette;
        img.color = Color.white;
        img.raycastTarget = false;
        StretchFull(go.GetComponent<RectTransform>());
        go.transform.SetAsFirstSibling();
    }

    private static void SetProjectileHitSprite(Sprite spark)
    {
        string[] guids = AssetDatabase.FindAssets("Projectile t:GameObject", new[] { "Assets/BrickGame/Prefabs" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            Projectile proj = prefab.GetComponent<Projectile>();
            if (proj == null) continue;
            proj.hitSprite = spark;
            EditorUtility.SetDirty(prefab);
        }
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(TexFolder)) AssetDatabase.CreateFolder("Assets/BrickGame", "Textures");
    }

    private static Sprite GetSpark()
    {
        Sprite dot = AssetDatabase.LoadAssetAtPath<Sprite>(DotPath);
        if (dot != null) return dot;

        if (!File.Exists(SparkPath))
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 c = new Vector2(size / 2f, size / 2f);
            float r = size / 2f;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), c) / r;
                    float a = Mathf.Clamp01(1f - d);
                    a = a * a;
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }
            tex.Apply();
            File.WriteAllBytes(SparkPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            ImportSprite(SparkPath);
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(SparkPath);
    }

    private static Sprite GenerateVignette()
    {
        if (!File.Exists(VignettePath))
        {
            int size = 256;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 c = new Vector2(size / 2f, size / 2f);
            float maxR = Vector2.Distance(Vector2.zero, c);
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), c) / maxR;
                    float a = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.55f, 1f, d)) * 0.55f;
                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, a));
                }
            }
            tex.Apply();
            File.WriteAllBytes(VignettePath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            ImportSprite(VignettePath);
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(VignettePath);
    }

    private static void ImportSprite(string path)
    {
        AssetDatabase.ImportAsset(path);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.SaveAndReimport();
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

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
