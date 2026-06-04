using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class BrickGameSetup_Iteration4
{
    private const string RootFolder = "Assets/BrickGame";
    private const string TexFolder = "Assets/BrickGame/Textures";
    private const string PrefabFolder = "Assets/BrickGame/Prefabs";
    private const string AudioFolder = "Assets/BrickGame/Audio";
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";

    private const string DotPath = "Assets/BrickGame/Textures/dot.png";
    private const string DustMatPath = "Assets/BrickGame/Textures/DustMaterial.mat";
    private const string ImpactPath = "Assets/BrickGame/Audio/sfx_impact.wav";
    private const string DustPrefabPath = "Assets/BrickGame/Prefabs/DustBurst.prefab";

    [MenuItem("BrickGame/Setup Iteration 4 (Juice)")]
    public static void Setup()
    {
        EnsureFolders();
        GenerateDotSprite();
        GenerateDustMaterial();
        GenerateImpactSound();
        BuildDustPrefab();

        EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);

        Cleanup();

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Main Camera not found. Run Iteration 2 first.");
            return;
        }

        CameraShake shake = cam.gameObject.AddComponent<CameraShake>();

        GameObject pitGo = GameObject.Find("CollectionPit");
        GameObject pitVisual = GameObject.Find("PitVisual");
        if (pitGo == null || pitVisual == null)
        {
            Debug.LogWarning("Demolition scene objects not found. Run Iteration 2 first.");
            return;
        }

        CollectionPit pit = pitGo.GetComponent<CollectionPit>();

        GameObject imGo = new GameObject("ImpactManager");
        ImpactManager im = imGo.AddComponent<ImpactManager>();
        GameObject dustPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DustPrefabPath);
        im.dustPrefab = dustPrefab.GetComponent<ParticleSystem>();
        im.cameraShake = shake;
        im.impactClip = AssetDatabase.LoadAssetAtPath<AudioClip>(ImpactPath);

        PitJuice juice = pitVisual.AddComponent<PitJuice>();
        juice.pit = pit;
        juice.floor = FindChild(pitVisual, "Floor");
        juice.postL = FindChild(pitVisual, "PostL");
        juice.postR = FindChild(pitVisual, "PostR");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 4 setup complete. Dust, shake, impact sound, pit juice added.");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(TexFolder)) AssetDatabase.CreateFolder(RootFolder, "Textures");
        if (!AssetDatabase.IsValidFolder(PrefabFolder)) AssetDatabase.CreateFolder(RootFolder, "Prefabs");
        if (!AssetDatabase.IsValidFolder(AudioFolder)) AssetDatabase.CreateFolder(RootFolder, "Audio");
    }

    private static void Cleanup()
    {
        GameObject im = GameObject.Find("ImpactManager");
        if (im != null) Object.DestroyImmediate(im);

        Camera cam = Camera.main;
        if (cam != null)
        {
            CameraShake existing = cam.GetComponent<CameraShake>();
            if (existing != null) Object.DestroyImmediate(existing);
        }

        GameObject pitVisual = GameObject.Find("PitVisual");
        if (pitVisual != null)
        {
            PitJuice existing = pitVisual.GetComponent<PitJuice>();
            if (existing != null) Object.DestroyImmediate(existing);
        }
    }

    private static Transform FindChild(GameObject parent, string name)
    {
        Transform t = parent.transform.Find(name);
        return t;
    }

    private static void BuildDustPrefab()
    {
        if (File.Exists(DustPrefabPath)) return;

        GameObject go = new GameObject("DustBurst");
        ParticleSystem ps = go.AddComponent<ParticleSystem>();

        ParticleSystem.MainModule main = ps.main;
        main.duration = 0.8f;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = 0.5f;
        main.startSpeed = 1.6f;
        main.startSize = 0.28f;
        main.startColor = HexColor("CFC9BE");
        main.gravityModifier = 0.25f;
        main.maxParticles = 40;

        ParticleSystem.EmissionModule emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)10) });

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.12f;

        ParticleSystem.ColorOverLifetimeModule col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0f), new GradientAlphaKey(0f, 1f) });
        col.color = new ParticleSystem.MinMaxGradient(gradient);

        ParticleSystem.SizeOverLifetimeModule sol = ps.sizeOverLifetime;
        sol.enabled = true;
        AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0.6f, 1f, 1.4f);
        sol.size = new ParticleSystem.MinMaxCurve(1f, curve);

        ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = AssetDatabase.LoadAssetAtPath<Material>(DustMatPath);
        renderer.sortingOrder = 20;

        PrefabUtility.SaveAsPrefabAsset(go, DustPrefabPath);
        Object.DestroyImmediate(go);
    }

    private static void GenerateDotSprite()
    {
        if (File.Exists(DotPath)) return;

        int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) / radius;
                float alpha = Mathf.SmoothStep(1f, 0f, dist);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        tex.Apply();
        File.WriteAllBytes(DotPath, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(DotPath);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(DotPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.SaveAndReimport();
    }

    private static void GenerateDustMaterial()
    {
        if (File.Exists(DustMatPath)) return;

        Material mat = new Material(Shader.Find("Sprites/Default"));
        Texture2D dot = AssetDatabase.LoadAssetAtPath<Texture2D>(DotPath);
        mat.mainTexture = dot;
        AssetDatabase.CreateAsset(mat, DustMatPath);
    }

    private static void GenerateImpactSound()
    {
        if (File.Exists(ImpactPath)) return;

        int sampleRate = 44100;
        float duration = 0.12f;
        int count = (int)(duration * sampleRate);
        float[] samples = new float[count];
        System.Random rng = new System.Random(1234);

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / sampleRate;
            float env = Mathf.Exp(-45f * t);
            float noise = (float)(rng.NextDouble() * 2.0 - 1.0);
            samples[i] = noise * env * 0.5f;
        }

        WriteWav(ImpactPath, samples, sampleRate);
        AssetDatabase.ImportAsset(ImpactPath);
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

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
