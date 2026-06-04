using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BrickGameSetup_Iteration5
{
    private const string RootFolder = "Assets/BrickGame";
    private const string PrefabFolder = "Assets/BrickGame/Prefabs";
    private const string BattlePath = "Assets/BrickGame/Scenes/Battle.unity";
    private const string SquarePath = "Assets/BrickGame/Textures/square.png";
    private const string TapPath = "Assets/BrickGame/Audio/sfx_tap.wav";
    private const string TransitionPath = "Assets/BrickGame/Audio/sfx_transition.wav";
    private const string ImpactPath = "Assets/BrickGame/Audio/sfx_impact.wav";
    private const string DustPrefabPath = "Assets/BrickGame/Prefabs/DustBurst.prefab";
    private const string EnemyPrefabPath = "Assets/BrickGame/Prefabs/Enemy.prefab";
    private const string ProjectilePrefabPath = "Assets/BrickGame/Prefabs/Projectile.prefab";

    private static readonly Color ColorAccent = HexColor("F5A623");
    private static readonly Color ColorBackground = HexColor("1A1A2E");
    private static readonly Color ColorGround = HexColor("2A2A4A");
    private static readonly Color ColorCannon = HexColor("596089");
    private static readonly Color ColorEnemy = HexColor("E0533D");
    private static readonly Color ColorProjectile = HexColor("6BA8F0");
    private static readonly Color ColorPanel = HexColor("232347");

    private static Sprite squareSprite;

    [MenuItem("BrickGame/Setup Iteration 5 (Battle)")]
    public static void Setup()
    {
        squareSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SquarePath);
        if (squareSprite == null)
        {
            Debug.LogWarning("square.png not found. Run Setup Iteration 2 first.");
            return;
        }

        BuildEnemyPrefab();
        BuildProjectilePrefab();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera();
        CreateManagers();
        MaybeCreateImpact();

        NewSprite("Ground", squareSprite, ColorGround, new Vector3(1.5f, -3.2f, 0f), new Vector3(40f, 2f, 1f), -5);

        GameObject cannon = NewSprite("Cannon", squareSprite, ColorCannon, new Vector3(-3f, -1.6f, 0f), new Vector3(1.3f, 1.5f, 1f), 5);
        GameObject barrel = NewSprite("Barrel", squareSprite, HexColor("3E4566"), Vector3.zero, Vector3.one, 6);
        barrel.transform.SetParent(cannon.transform, false);
        barrel.transform.localScale = new Vector3(0.55f, 0.35f, 1f);
        barrel.transform.localPosition = new Vector3(0.55f, 0.2f, 0f);
        GameObject muzzle = new GameObject("Muzzle");
        muzzle.transform.SetParent(cannon.transform, false);
        muzzle.transform.localPosition = new Vector3(0.8f, 0.2f, 0f);

        GameObject canvas = CreateCanvas(out GameObject safeArea);
        BattleHUD hud = CreateHUD(safeArea.transform);
        BattleResultPopup popup = CreatePopup(canvas.transform);

        GameObject controllerGo = new GameObject("BattleController");
        BattleController controller = controllerGo.AddComponent<BattleController>();
        controller.muzzle = muzzle.transform;
        controller.cannon = cannon.transform;
        controller.enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyPrefabPath);
        controller.projectilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ProjectilePrefabPath).GetComponent<Projectile>();
        controller.hud = hud;
        controller.popup = popup;

        EditorSceneManager.SaveScene(scene, BattlePath);

        AddSceneToBuild(BattlePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 5 setup complete. Battle scene created and added to Build Settings.");
    }

    private static void BuildEnemyPrefab()
    {
        if (System.IO.File.Exists(EnemyPrefabPath)) return;

        GameObject go = new GameObject("Enemy");
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = squareSprite;
        sr.color = ColorEnemy;
        sr.sortingOrder = 2;
        go.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;

        go.AddComponent<Enemy>();

        PrefabUtility.SaveAsPrefabAsset(go, EnemyPrefabPath);
        Object.DestroyImmediate(go);
    }

    private static void BuildProjectilePrefab()
    {
        if (System.IO.File.Exists(ProjectilePrefabPath)) return;

        GameObject go = new GameObject("Projectile");
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = squareSprite;
        sr.color = ColorProjectile;
        sr.sortingOrder = 4;
        go.transform.localScale = new Vector3(0.35f, 0.35f, 1f);

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.6f;

        go.AddComponent<Projectile>();

        PrefabUtility.SaveAsPrefabAsset(go, ProjectilePrefabPath);
        Object.DestroyImmediate(go);
    }

    private static void CreateCamera()
    {
        GameObject camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        Camera cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 9f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = ColorBackground;
        camGo.transform.position = new Vector3(1.5f, -0.5f, -10f);
        camGo.AddComponent<AudioListener>();
    }

    private static void CreateManagers()
    {
        GameObject bootstrap = new GameObject("GameBootstrap");
        bootstrap.AddComponent<GameBootstrap>();

        GameObject sound = new GameObject("SoundManager");
        SoundManager sm = sound.AddComponent<SoundManager>();
        sm.tapClip = AssetDatabase.LoadAssetAtPath<AudioClip>(TapPath);
        sm.transitionClip = AssetDatabase.LoadAssetAtPath<AudioClip>(TransitionPath);

        GameObject haptic = new GameObject("HapticManager");
        haptic.AddComponent<HapticManager>();

        GameObject transition = new GameObject("TransitionManager");
        transition.AddComponent<TransitionManager>();
    }

    private static void MaybeCreateImpact()
    {
        GameObject dustPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DustPrefabPath);
        if (dustPrefab == null) return;

        Camera cam = Camera.main;
        CameraShake shake = cam.gameObject.AddComponent<CameraShake>();

        GameObject imGo = new GameObject("ImpactManager");
        ImpactManager im = imGo.AddComponent<ImpactManager>();
        im.dustPrefab = dustPrefab.GetComponent<ParticleSystem>();
        im.cameraShake = shake;
        im.impactClip = AssetDatabase.LoadAssetAtPath<AudioClip>(ImpactPath);
    }

    private static GameObject CreateCanvas(out GameObject safeArea)
    {
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        safeArea = new GameObject("SafeArea", typeof(RectTransform));
        safeArea.transform.SetParent(canvasGo.transform, false);
        StretchFull(safeArea.GetComponent<RectTransform>());
        safeArea.AddComponent<SafeAreaFitter>();

        return canvasGo;
    }

    private static BattleHUD CreateHUD(Transform safeArea)
    {
        GameObject hudGo = new GameObject("BattleHUD");
        BattleHUD hud = hudGo.AddComponent<BattleHUD>();

        TextMeshProUGUI ammo = CreateText("AmmoText", safeArea, "Attacks: 0", 54, Color.white);
        RectTransform ammoRt = ammo.rectTransform;
        ammoRt.anchorMin = new Vector2(0f, 1f);
        ammoRt.anchorMax = new Vector2(0f, 1f);
        ammoRt.pivot = new Vector2(0f, 1f);
        ammoRt.sizeDelta = new Vector2(500f, 90f);
        ammoRt.anchoredPosition = new Vector2(40f, -30f);
        ammo.alignment = TextAlignmentOptions.Left;
        ApplyOutline(ammo);

        TextMeshProUGUI enemies = CreateText("EnemyText", safeArea, "Enemies: 0", 54, Color.white);
        RectTransform enRt = enemies.rectTransform;
        enRt.anchorMin = new Vector2(1f, 1f);
        enRt.anchorMax = new Vector2(1f, 1f);
        enRt.pivot = new Vector2(1f, 1f);
        enRt.sizeDelta = new Vector2(500f, 90f);
        enRt.anchoredPosition = new Vector2(-40f, -30f);
        enemies.alignment = TextAlignmentOptions.Right;
        ApplyOutline(enemies);

        hud.ammoText = ammo;
        hud.enemyText = enemies;
        return hud;
    }

    private static BattleResultPopup CreatePopup(Transform canvas)
    {
        GameObject root = new GameObject("BattleResultPopup", typeof(RectTransform));
        root.transform.SetParent(canvas, false);
        StretchFull(root.GetComponent<RectTransform>());

        GameObject backdropGo = CreateImage("Backdrop", root.transform, new Color(0f, 0f, 0f, 1f));
        StretchFull(backdropGo.GetComponent<RectTransform>());
        CanvasGroup backdropCg = backdropGo.AddComponent<CanvasGroup>();
        backdropCg.alpha = 0f;

        GameObject panelGo = CreateImage("Panel", root.transform, ColorPanel);
        RectTransform panelRt = panelGo.GetComponent<RectTransform>();
        SetCentered(panelRt, new Vector2(820f, 720f), Vector2.zero);

        TextMeshProUGUI title = CreateText("Title", panelGo.transform, "VICTORY", 100, ColorAccent);
        SetCentered(title.rectTransform, new Vector2(760f, 160f), new Vector2(0f, 220f));
        title.fontStyle = FontStyles.Bold;
        ApplyOutline(title);

        TextMeshProUGUI subtitle = CreateText("Subtitle", panelGo.transform, "Wave cleared!", 50, Color.white);
        SetCentered(subtitle.rectTransform, new Vector2(720f, 120f), new Vector2(0f, 40f));

        GameObject actionGo = CreateButton("ActionButton", panelGo.transform, ColorAccent, out Button actionButton);
        SetCentered(actionGo.GetComponent<RectTransform>(), new Vector2(420f, 160f), new Vector2(0f, -200f));
        TextMeshProUGUI actionLabel = CreateText("Label", actionGo.transform, "NEXT", 64, Color.white);
        StretchFull(actionLabel.rectTransform);
        actionLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(actionLabel);

        BattleResultPopup popup = root.AddComponent<BattleResultPopup>();
        popup.backdrop = backdropCg;
        popup.panel = panelRt;
        popup.titleText = title;
        popup.subtitleText = subtitle;
        popup.actionButton = actionButton;
        popup.actionLabel = actionLabel;

        root.SetActive(false);
        return popup;
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

    private static void AddSceneToBuild(string path)
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (EditorBuildSettingsScene s in scenes)
        {
            if (s.path == path) return;
        }
        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
