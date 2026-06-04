using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BrickGameSetup_Iteration2
{
    private const string RootFolder = "Assets/BrickGame";
    private const string TexFolder = "Assets/BrickGame/Textures";
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";

    private const string SquarePath = "Assets/BrickGame/Textures/square.png";
    private const string RopeTexPath = "Assets/BrickGame/Textures/rope.png";
    private const string RopeMatPath = "Assets/BrickGame/Textures/RopeMaterial.mat";
    private const string BrickPhysPath = "Assets/BrickGame/Textures/BrickPhysics.physicsMaterial2D";

    private static readonly Color ColorPrimary = HexColor("4A90E2");
    private static readonly Color ColorAccent = HexColor("F5A623");
    private static readonly Color ColorBackground = HexColor("1A1A2E");
    private static readonly Color ColorGround = HexColor("2A2A4A");
    private static readonly Color ColorBrickLight = HexColor("6BA8F0");
    private static readonly Color ColorBrickDark = HexColor("3A6FB0");
    private static readonly Color ColorBeam = HexColor("F5A623");
    private static readonly Color ColorSupport = HexColor("8E5BC9");

    private static Sprite squareSprite;
    private static Material ropeMaterial;
    private static PhysicsMaterial2D brickPhysics;

    private const float BrickW = 0.62f;
    private const float BrickH = 0.34f;
    private const float SupportH = 0.6f;
    private const float BuildingBaseX = 2.5f;
    private const float ColGap = 0.9f;

    [MenuItem("BrickGame/Setup Iteration 2 (Demolition Scene)")]
    public static void Setup()
    {
        EnsureFolders();
        GenerateSquareSprite();
        GenerateRopeAssets();
        GenerateBrickPhysics();

        squareSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SquarePath);
        ropeMaterial = AssetDatabase.LoadAssetAtPath<Material>(RopeMatPath);
        brickPhysics = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(BrickPhysPath);

        Scene scene = EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);

        CleanupPrevious();
        SetupCamera();

        CreateGround();
        Truck truck = CreateTruck();
        CollectionPit pit = CreatePit();

        List<Brick> bricks = new List<Brick>();
        BuildStructure(bricks);

        pit.totalBricks = bricks.Count;

        Rope rope = CreateRope(truck);

        SetupHUD();

        GameObject controllerGo = new GameObject("DemolitionController");
        DemolitionController controller = controllerGo.AddComponent<DemolitionController>();
        controller.cam = Camera.main;
        controller.rope = rope;
        controller.truck = truck;
        controller.pullDuration = 1.0f;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 2 setup complete. Open Game scene and press Play. Built " + bricks.Count + " bricks.");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(TexFolder))
            AssetDatabase.CreateFolder(RootFolder, "Textures");
    }

    private static void CleanupPrevious()
    {
        string[] names = { "Ground", "Truck", "CollectionPit", "PitVisual", "Building", "Rope", "DemolitionController", "GameHUD" };
        Scene scene = SceneManager.GetActiveScene();
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root == null) continue;

            bool remove = root.name.StartsWith("AttachPoint");
            if (!remove)
            {
                foreach (string n in names)
                {
                    if (root.name == n)
                    {
                        remove = true;
                        break;
                    }
                }
            }

            if (remove) Object.DestroyImmediate(root);
        }

        GameObject placeholder = GameObject.Find("Ground_Placeholder");
        if (placeholder != null) Object.DestroyImmediate(placeholder);

        GameObject wip = GameObject.Find("Canvas/SafeArea/WIP_Label");
        if (wip != null) Object.DestroyImmediate(wip);
    }

    private static void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        cam.orthographic = true;
        cam.orthographicSize = 10f;
        cam.backgroundColor = ColorBackground;
        cam.transform.position = new Vector3(0f, 2.5f, -10f);
    }

    private static void CreateGround()
    {
        GameObject ground = NewSprite("Ground", squareSprite, ColorGround, new Vector3(0f, -1f, 0f), new Vector3(26f, 2f, 1f), -5);
        BoxCollider2D col = ground.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
        col.sharedMaterial = brickPhysics;
    }

    private static Truck CreateTruck()
    {
        GameObject truckGo = NewSprite("Truck", squareSprite, ColorPrimary, new Vector3(-4.5f, 0.6f, 0f), new Vector3(1.5f, 1.2f, 1f), 5);

        GameObject cab = NewSprite("Cab", squareSprite, HexColor("2E5A9E"), Vector3.zero, Vector3.one, 6);
        cab.transform.SetParent(truckGo.transform, false);
        cab.transform.localScale = new Vector3(0.55f, 0.6f, 1f);
        cab.transform.localPosition = new Vector3(-0.2f, 0.45f, 0f);

        GameObject anchor = new GameObject("RopeAnchor");
        anchor.transform.SetParent(truckGo.transform, false);
        anchor.transform.localPosition = new Vector3(0.65f, 0.15f, 0f);

        Truck truck = truckGo.AddComponent<Truck>();
        truck.anchor = anchor.transform;
        return truck;
    }

    private static CollectionPit CreatePit()
    {
        float centerX = -0.5f;
        float halfWidth = 1.5f;

        GameObject visual = new GameObject("PitVisual");
        visual.transform.position = Vector3.zero;

        GameObject floor = NewSprite("Floor", squareSprite, new Color(ColorAccent.r, ColorAccent.g, ColorAccent.b, 0.45f), new Vector3(centerX, 0.06f, 0f), new Vector3(halfWidth * 2f, 0.25f, 1f), -3);
        floor.transform.SetParent(visual.transform, true);

        GameObject postL = NewSprite("PostL", squareSprite, ColorAccent, new Vector3(centerX - halfWidth, 0.55f, 0f), new Vector3(0.14f, 1.1f, 1f), -2);
        postL.transform.SetParent(visual.transform, true);
        GameObject postR = NewSprite("PostR", squareSprite, ColorAccent, new Vector3(centerX + halfWidth, 0.55f, 0f), new Vector3(0.14f, 1.1f, 1f), -2);
        postR.transform.SetParent(visual.transform, true);

        GameObject pitGo = new GameObject("CollectionPit");
        pitGo.transform.position = new Vector3(centerX, 2f, 0f);
        BoxCollider2D trigger = pitGo.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = new Vector2(halfWidth * 2f, 4f);
        CollectionPit pit = pitGo.AddComponent<CollectionPit>();
        return pit;
    }

    private static void BuildStructure(List<Brick> bricks)
    {
        GameObject building = new GameObject("Building");
        building.transform.position = Vector3.zero;

        float leftX = BuildingBaseX - ColGap * 0.5f;
        float rightX = BuildingBaseX + ColGap * 0.5f;
        float groundTop = 0f;
        float gap = 0.002f;

        Brick supportL = CreateBrick(building.transform, new Vector3(leftX, groundTop + SupportH * 0.5f, 0f), new Vector3(BrickW, SupportH, 1f), ColorSupport);
        Brick supportR = CreateBrick(building.transform, new Vector3(rightX, groundTop + SupportH * 0.5f, 0f), new Vector3(BrickW, SupportH, 1f), ColorSupport);
        bricks.Add(supportL);
        bricks.Add(supportR);

        float currentTop = groundTop + SupportH;
        int rows = 16;
        for (int r = 0; r < rows; r++)
        {
            float yc = currentTop + gap + BrickH * 0.5f;
            float lerp = (float)r / rows;
            Color c = Color.Lerp(ColorBrickDark, ColorBrickLight, lerp);

            if (r == 4 || r == 9 || r == 14)
            {
                Brick beam = CreateBrick(building.transform, new Vector3(BuildingBaseX, yc, 0f), new Vector3(ColGap + BrickW, BrickH, 1f), ColorBeam);
                bricks.Add(beam);
            }
            else
            {
                Brick bl = CreateBrick(building.transform, new Vector3(leftX, yc, 0f), new Vector3(BrickW, BrickH, 1f), c);
                Brick br = CreateBrick(building.transform, new Vector3(rightX, yc, 0f), new Vector3(BrickW, BrickH, 1f), c);
                bricks.Add(bl);
                bricks.Add(br);
            }

            currentTop = yc + BrickH * 0.5f;
        }
    }

    private static Brick CreateBrick(Transform parent, Vector3 pos, Vector3 scale, Color color)
    {
        GameObject go = NewSprite("Brick", squareSprite, color, pos, scale, 0);
        go.transform.SetParent(parent, true);

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2.0f;
        rb.mass = scale.x * scale.y * 8f;
        rb.drag = 0.2f;
        rb.angularDrag = 0.8f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
        col.sharedMaterial = brickPhysics;

        return go.AddComponent<Brick>();
    }

    private static Rope CreateRope(Truck truck)
    {
        GameObject go = new GameObject("Rope");
        LineRenderer line = go.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.22f;
        line.endWidth = 0.22f;
        line.numCapVertices = 4;
        line.numCornerVertices = 2;
        line.textureMode = LineTextureMode.Tile;
        line.alignment = LineAlignment.View;
        line.useWorldSpace = true;
        line.material = ropeMaterial;
        line.sortingOrder = 8;
        line.enabled = false;

        Rope rope = go.AddComponent<Rope>();
        rope.truckAnchor = truck.anchor;
        rope.maxPullForce = 60f;
        rope.rampTime = 0.5f;
        return rope;
    }

    private static GameHUDController SetupHUD()
    {
        GameObject safeArea = GameObject.Find("Canvas/SafeArea");
        if (safeArea == null)
        {
            Debug.LogWarning("Canvas/SafeArea not found! Run Iteration 1 setup first.");
        }

        GameObject hudGo = new GameObject("GameHUD");
        GameHUDController hud = hudGo.AddComponent<GameHUDController>();

        TextMeshProUGUI countText = CreateText("BrickCounter", safeArea.transform, "Bricks: 0 / 0", 60, Color.white);
        RectTransform rt = countText.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(900f, 100f);
        rt.anchoredPosition = new Vector2(0f, -30f);
        countText.alignment = TextAlignmentOptions.Center;
        ApplyOutline(countText);

        hud.countText = countText;
        return hud;
    }

    private static GameObject NewSprite(string name, Sprite sprite, Color color, Vector3 pos, Vector3 scale, int order)
    {
        GameObject go = new GameObject(name);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingOrder = order;
        go.transform.position = pos;
        go.transform.localScale = scale;
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

    private static void GenerateSquareSprite()
    {
        if (!File.Exists(SquarePath))
        {
            int size = 8;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color32[] pixels = new Color32[size * size];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(255, 255, 255, 255);
            tex.SetPixels32(pixels);
            tex.Apply();
            File.WriteAllBytes(SquarePath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(SquarePath);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(SquarePath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 8f;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
        }
    }

    private static void GenerateRopeAssets()
    {
        if (!File.Exists(RopeTexPath))
        {
            int w = 48;
            int h = 24;
            int twists = 3;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            Color light = HexColor("C9A05A");
            Color dark = HexColor("6E4E26");
            Color edge = HexColor("4A331A");

            for (int x = 0; x < w; x++)
            {
                float u = (float)x / w;
                for (int yy = 0; yy < h; yy++)
                {
                    float v = (float)yy / h;
                    float strand = Mathf.Sin((u * twists + v) * Mathf.PI * 2f);
                    float k = strand * 0.5f + 0.5f;
                    Color c = Color.Lerp(dark, light, k);
                    float edgeDist = Mathf.Abs(v - 0.5f) * 2f;
                    float edgeFactor = Mathf.SmoothStep(0f, 1f, 1f - edgeDist);
                    c = Color.Lerp(edge, c, edgeFactor);
                    tex.SetPixel(x, yy, c);
                }
            }
            tex.Apply();
            File.WriteAllBytes(RopeTexPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(RopeTexPath);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(RopeTexPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

        if (!File.Exists(RopeMatPath))
        {
            Material mat = new Material(Shader.Find("Sprites/Default"));
            Texture2D ropeTex = AssetDatabase.LoadAssetAtPath<Texture2D>(RopeTexPath);
            mat.mainTexture = ropeTex;
            AssetDatabase.CreateAsset(mat, RopeMatPath);
        }
    }

    private static void GenerateBrickPhysics()
    {
        PhysicsMaterial2D mat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(BrickPhysPath);
        if (mat == null)
        {
            mat = new PhysicsMaterial2D("BrickPhysics");
            AssetDatabase.CreateAsset(mat, BrickPhysPath);
        }
        mat.friction = 0.9f;
        mat.bounciness = 0f;
        EditorUtility.SetDirty(mat);
    }

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
