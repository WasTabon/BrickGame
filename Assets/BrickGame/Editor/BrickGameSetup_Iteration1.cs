using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.U2D;
using UnityEngine.U2D;
using TMPro;

public class BrickGameSetup_Iteration1
{
    private const string RootFolder = "Assets/BrickGame";
    private const string AudioFolder = "Assets/BrickGame/Audio";
    private const string ScenesFolder = "Assets/BrickGame/Scenes";
    private const string MainMenuPath = "Assets/BrickGame/Scenes/MainMenu.unity";
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";
    private const string AtlasPath = "Assets/BrickGame/UI.spriteatlas";
    private const string TapPath = "Assets/BrickGame/Audio/sfx_tap.wav";
    private const string TransitionPath = "Assets/BrickGame/Audio/sfx_transition.wav";

    private static readonly Color ColorPrimary = HexColor("4A90E2");
    private static readonly Color ColorAccent = HexColor("F5A623");
    private static readonly Color ColorBackground = HexColor("1A1A2E");
    private static readonly Color ColorText = Color.white;

    [MenuItem("BrickGame/Setup Iteration 1 (Main Menu)")]
    public static void Setup()
    {
        EnsureFolders();
        GeneratePlaceholderAudio();
        CreateSpriteAtlas();
        ConfigurePlayerSettings();

        BuildGameScene();
        BuildMainMenuScene();

        ConfigureBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorSceneManager.OpenScene(MainMenuPath, OpenSceneMode.Single);

        Debug.Log("BrickGame Iteration 1 setup complete. Press Play from the MainMenu scene.");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/BrickGame"))
            AssetDatabase.CreateFolder("Assets", "BrickGame");
        if (!AssetDatabase.IsValidFolder(AudioFolder))
            AssetDatabase.CreateFolder(RootFolder, "Audio");
        if (!AssetDatabase.IsValidFolder(ScenesFolder))
            AssetDatabase.CreateFolder(RootFolder, "Scenes");
    }

    private static void ConfigurePlayerSettings()
    {
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
    }

    private static void GeneratePlaceholderAudio()
    {
        if (!File.Exists(TapPath))
        {
            float[] tap = BuildTone(880f, 0.08f, 44100, 40f);
            WriteWav(TapPath, tap, 44100);
        }

        if (!File.Exists(TransitionPath))
        {
            float[] transition = BuildSweep(220f, 660f, 0.25f, 44100);
            WriteWav(TransitionPath, transition, 44100);
        }

        AssetDatabase.ImportAsset(TapPath);
        AssetDatabase.ImportAsset(TransitionPath);
    }

    private static void CreateSpriteAtlas()
    {
        if (File.Exists(AtlasPath)) return;

        SpriteAtlas atlas = new SpriteAtlas();
        SpriteAtlasPackingSettings packing = new SpriteAtlasPackingSettings
        {
            enableRotation = false,
            enableTightPacking = false,
            padding = 2
        };
        atlas.SetPackingSettings(packing);
        AssetDatabase.CreateAsset(atlas, AtlasPath);
    }

    private static void BuildGameScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera();
        CreateManagers();

        GameObject ground = new GameObject("Ground_Placeholder");
        SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
        sr.sprite = BuiltinSprite();
        sr.color = HexColor("2A2A4A");
        ground.transform.position = new Vector3(0f, -4.5f, 0f);
        ground.transform.localScale = new Vector3(20f, 2f, 1f);

        GameObject canvasGo = CreateCanvas(out GameObject safeArea);
        TextMeshProUGUI wip = CreateText("WIP_Label", safeArea.transform, "GAME SCENE\n(WIP)", 70, ColorText);
        RectTransform wipRt = wip.rectTransform;
        wipRt.anchorMin = new Vector2(0.5f, 0.5f);
        wipRt.anchorMax = new Vector2(0.5f, 0.5f);
        wipRt.pivot = new Vector2(0.5f, 0.5f);
        wipRt.sizeDelta = new Vector2(900f, 400f);
        wipRt.anchoredPosition = Vector2.zero;
        wip.alignment = TextAlignmentOptions.Center;

        EditorSceneManager.SaveScene(scene, GamePath);
    }

    private static void BuildMainMenuScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera();
        CreateManagers();

        GameObject canvasGo = CreateCanvas(out GameObject safeArea);

        GameObject topbar = new GameObject("TopBar", typeof(RectTransform));
        topbar.transform.SetParent(safeArea.transform, false);
        RectTransform topRt = topbar.GetComponent<RectTransform>();
        topRt.anchorMin = new Vector2(0f, 1f);
        topRt.anchorMax = new Vector2(1f, 1f);
        topRt.pivot = new Vector2(0.5f, 1f);
        topRt.sizeDelta = new Vector2(0f, 150f);
        topRt.anchoredPosition = Vector2.zero;

        GameObject brickIcon = CreateImage("BrickIcon", topbar.transform, BuiltinSprite(), ColorAccent);
        RectTransform iconRt = brickIcon.GetComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0f, 0.5f);
        iconRt.anchorMax = new Vector2(0f, 0.5f);
        iconRt.pivot = new Vector2(0f, 0.5f);
        iconRt.sizeDelta = new Vector2(70f, 70f);
        iconRt.anchoredPosition = new Vector2(40f, 0f);

        TextMeshProUGUI countText = CreateText("BrickCount", topbar.transform, "0", 60, ColorText);
        RectTransform countRt = countText.rectTransform;
        countRt.anchorMin = new Vector2(0f, 0.5f);
        countRt.anchorMax = new Vector2(0f, 0.5f);
        countRt.pivot = new Vector2(0f, 0.5f);
        countRt.sizeDelta = new Vector2(250f, 90f);
        countRt.anchoredPosition = new Vector2(130f, 0f);
        countText.alignment = TextAlignmentOptions.Left;
        ApplyOutline(countText);

        GameObject settingsGo = CreateButton("SettingsButton", topbar.transform, BuiltinKnob(), ColorPrimary, out Button settingsButton);
        RectTransform setRt = settingsGo.GetComponent<RectTransform>();
        setRt.anchorMin = new Vector2(1f, 0.5f);
        setRt.anchorMax = new Vector2(1f, 0.5f);
        setRt.pivot = new Vector2(1f, 0.5f);
        setRt.sizeDelta = new Vector2(110f, 110f);
        setRt.anchoredPosition = new Vector2(-40f, 0f);
        TextMeshProUGUI setLabel = CreateText("Label", settingsGo.transform, "S", 50, ColorText);
        StretchFull(setLabel.rectTransform);
        setLabel.alignment = TextAlignmentOptions.Center;

        TextMeshProUGUI title = CreateText("Title", safeArea.transform, "BRICK GAME", 110, ColorAccent);
        RectTransform titleRt = title.rectTransform;
        titleRt.anchorMin = new Vector2(0.5f, 0.5f);
        titleRt.anchorMax = new Vector2(0.5f, 0.5f);
        titleRt.pivot = new Vector2(0.5f, 0.5f);
        titleRt.sizeDelta = new Vector2(1000f, 250f);
        titleRt.anchoredPosition = new Vector2(0f, 400f);
        title.alignment = TextAlignmentOptions.Center;
        title.fontStyle = FontStyles.Bold;
        ApplyOutline(title);

        GameObject playGo = CreateButton("PlayButton", safeArea.transform, BuiltinSprite(), ColorPrimary, out Button playButton);
        RectTransform playRt = playGo.GetComponent<RectTransform>();
        playRt.anchorMin = new Vector2(0.5f, 0.5f);
        playRt.anchorMax = new Vector2(0.5f, 0.5f);
        playRt.pivot = new Vector2(0.5f, 0.5f);
        playRt.sizeDelta = new Vector2(520f, 190f);
        playRt.anchoredPosition = new Vector2(0f, -100f);
        TextMeshProUGUI playLabel = CreateText("Label", playGo.transform, "PLAY", 80, ColorText);
        StretchFull(playLabel.rectTransform);
        playLabel.alignment = TextAlignmentOptions.Center;
        playLabel.fontStyle = FontStyles.Bold;
        ApplyOutline(playLabel);

        GameObject controllerGo = new GameObject("MainMenuController");
        MainMenuController controller = controllerGo.AddComponent<MainMenuController>();
        controller.playButton = playButton;
        controller.settingsButton = settingsButton;
        controller.brickCountText = countText;

        EditorSceneManager.SaveScene(scene, MainMenuPath);
        EditorSceneManager.MarkSceneDirty(scene);
    }

    private static void CreateCamera()
    {
        GameObject camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        Camera cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 10f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = ColorBackground;
        camGo.transform.position = new Vector3(0f, 0f, -10f);
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
        RectTransform saRt = safeArea.GetComponent<RectTransform>();
        StretchFull(saRt);
        safeArea.AddComponent<SafeAreaFitter>();

        return canvasGo;
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

    private static GameObject CreateButton(string name, Transform parent, Sprite sprite, Color color, out Button button)
    {
        GameObject go = CreateImage(name, parent, sprite, color);
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
        if (font != null)
        {
            text.font = font;
        }
        else
        {
            Debug.LogWarning("TMP default font asset not found. Import TMP Essentials (Window > TextMeshPro > Import TMP Essential Resources).");
        }

        return text;
    }

    private static void ApplyOutline(TextMeshProUGUI text)
    {
        if (text.fontSharedMaterial == null) return;
        Material instance = new Material(text.fontSharedMaterial);
        text.fontMaterial = instance;
        text.outlineColor = new Color(0f, 0f, 0f, 1f);
        text.outlineWidth = 0.15f;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static Sprite BuiltinSprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }

    private static Sprite BuiltinKnob()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
    }

    private static void ConfigureBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(MainMenuPath, true),
            new EditorBuildSettingsScene(GamePath, true)
        };
        EditorBuildSettings.scenes = scenes;
    }

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }

    private static float[] BuildTone(float frequency, float duration, int sampleRate, float decay)
    {
        int count = (int)(duration * sampleRate);
        float[] samples = new float[count];
        for (int i = 0; i < count; i++)
        {
            float t = (float)i / sampleRate;
            float env = Mathf.Exp(-decay * t);
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * env * 0.6f;
        }
        return samples;
    }

    private static float[] BuildSweep(float startFreq, float endFreq, float duration, int sampleRate)
    {
        int count = (int)(duration * sampleRate);
        float[] samples = new float[count];
        float phase = 0f;
        for (int i = 0; i < count; i++)
        {
            float t = (float)i / count;
            float freq = Mathf.Lerp(startFreq, endFreq, t);
            phase += 2f * Mathf.PI * freq / sampleRate;
            float env = Mathf.Sin(Mathf.PI * t);
            samples[i] = Mathf.Sin(phase) * env * 0.5f;
        }
        return samples;
    }

    private static void WriteWav(string path, float[] samples, int sampleRate)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            int channels = 1;
            int bitsPerSample = 16;
            int byteRate = sampleRate * channels * bitsPerSample / 8;
            int blockAlign = channels * bitsPerSample / 8;
            int dataSize = samples.Length * blockAlign;

            writer.Write(new char[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + dataSize);
            writer.Write(new char[] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[] { 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)blockAlign);
            writer.Write((short)bitsPerSample);
            writer.Write(new char[] { 'd', 'a', 't', 'a' });
            writer.Write(dataSize);

            for (int i = 0; i < samples.Length; i++)
            {
                short value = (short)(Mathf.Clamp(samples[i], -1f, 1f) * short.MaxValue);
                writer.Write(value);
            }
        }
    }
}
