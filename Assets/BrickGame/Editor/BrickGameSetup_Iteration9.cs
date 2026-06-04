using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class BrickGameSetup_Iteration9
{
    private const string RootFolder = "Assets/BrickGame";
    private const string TexFolder = "Assets/BrickGame/Textures";
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";
    private const string RingPath = "Assets/BrickGame/Textures/ring.png";
    private const string ArrowPath = "Assets/BrickGame/Textures/arrow.png";

    private static readonly Color ColorAccent = HexColor("F5A623");

    [MenuItem("BrickGame/Setup Iteration 9 (Tutorial)")]
    public static void Setup()
    {
        EnsureFolders();
        GenerateRingSprite();
        GenerateArrowSprite();

        Sprite ringSprite = AssetDatabase.LoadAssetAtPath<Sprite>(RingPath);
        Sprite arrowSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArrowPath);

        EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);

        GameObject existing = GameObject.Find("Tutorial");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject root = new GameObject("Tutorial");

        SpriteRenderer ring = NewSprite("RingHighlight", ringSprite, ColorAccent, new Vector3(2.5f, 0.6f, 0f), new Vector3(1.2f, 1.2f, 1f), 50);
        ring.transform.SetParent(root.transform, true);

        SpriteRenderer ripple = NewSprite("Ripple", ringSprite, ColorAccent, new Vector3(2.5f, 0.6f, 0f), new Vector3(1.2f, 1.2f, 1f), 50);
        ripple.transform.SetParent(root.transform, true);

        SpriteRenderer arrow = NewSprite("Arrow", arrowSprite, ColorAccent, new Vector3(1.1f, 1.0f, 0f), new Vector3(1.6f, 1.0f, 1f), 50);
        arrow.transform.SetParent(root.transform, true);

        Tutorial tutorial = root.AddComponent<Tutorial>();
        tutorial.ring = ring;
        tutorial.ripple = ripple;
        tutorial.arrow = arrow;
        tutorial.tapWorld = new Vector3(2.5f, 0.6f, 0f);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 9 setup complete. Tutorial added (level 1, first time only).");
    }

    [MenuItem("BrickGame/Reset Tutorial Flag")]
    public static void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutDone", 0);
        PlayerPrefs.Save();
        Debug.Log("Tutorial flag reset. It will show again on level 1.");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(TexFolder)) AssetDatabase.CreateFolder(RootFolder, "Textures");
    }

    private static SpriteRenderer NewSprite(string name, Sprite sprite, Color color, Vector3 pos, Vector3 scale, int order)
    {
        GameObject go = new GameObject(name);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingOrder = order;
        go.transform.position = pos;
        go.transform.localScale = scale;
        return sr;
    }

    private static void GenerateRingSprite()
    {
        if (File.Exists(RingPath)) return;

        int size = 96;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        float inner = 0.62f;
        float outer = 0.82f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) / radius;
                float a = (d >= inner && d <= outer) ? 1f : 0f;
                if (a > 0f)
                {
                    float edge = Mathf.Min(Mathf.InverseLerp(inner - 0.06f, inner, d), Mathf.InverseLerp(outer + 0.06f, outer, d));
                    a = Mathf.Clamp01(edge);
                }
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();
        File.WriteAllBytes(RingPath, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        ImportSprite(RingPath);
    }

    private static void GenerateArrowSprite()
    {
        if (File.Exists(ArrowPath)) return;

        int w = 96;
        int h = 64;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        float cy = h / 2f;
        float headEnd = 34f;
        float shaftTop = cy + 9f;
        float shaftBot = cy - 9f;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                bool inside = false;
                if (x <= headEnd)
                {
                    float halfH = (x / headEnd) * 30f;
                    if (Mathf.Abs(y - cy) <= halfH) inside = true;
                }
                else if (x <= 88f && y <= shaftTop && y >= shaftBot)
                {
                    inside = true;
                }
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, inside ? 1f : 0f));
            }
        }
        tex.Apply();
        File.WriteAllBytes(ArrowPath, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        ImportSprite(ArrowPath);
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

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
