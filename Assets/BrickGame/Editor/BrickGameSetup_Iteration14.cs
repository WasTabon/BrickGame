using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BrickGameSetup_Iteration14
{
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";
    private const string AudioFolder = "Assets/BrickGame/Audio";
    private const string FanfarePath = "Assets/BrickGame/Audio/sfx_fanfare.wav";

    private static readonly Color ColorAccent = HexColor("F5A623");

    [MenuItem("BrickGame/Setup Iteration 14 (Juice Layer)")]
    public static void Setup()
    {
        AudioClip fanfare = GenerateFanfare();

        EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);

        GameObject safeArea = GameObject.Find("Canvas/SafeArea");
        if (safeArea == null)
        {
            Debug.LogWarning("Canvas/SafeArea not found. Run earlier setups first.");
            return;
        }

        Cleanup("ComboText", safeArea.transform);
        DestroyIfExists("ComboManager");
        DestroyIfExists("SlowMoController");

        TextMeshProUGUI comboText = CreateText("ComboText", safeArea.transform, "", 60, ColorAccent);
        RectTransform crt = comboText.rectTransform;
        crt.anchorMin = new Vector2(0.5f, 1f);
        crt.anchorMax = new Vector2(0.5f, 1f);
        crt.pivot = new Vector2(0.5f, 1f);
        crt.sizeDelta = new Vector2(700f, 90f);
        crt.anchoredPosition = new Vector2(0f, -360f);
        comboText.fontStyle = FontStyles.Bold;
        ApplyOutline(comboText);

        GameObject comboGo = new GameObject("ComboManager");
        ComboManager combo = comboGo.AddComponent<ComboManager>();
        combo.comboText = comboText;

        GameObject slowGo = new GameObject("SlowMoController");
        slowGo.AddComponent<SlowMoController>();

        ResultPopup popup = Object.FindObjectOfType<ResultPopup>(true);
        if (popup != null) popup.fanfare = fanfare;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 14 setup complete. Combo, slow-mo, +1 popups, confetti+fanfare, brick consumption.");
    }

    private static AudioClip GenerateFanfare()
    {
        if (!AssetDatabase.IsValidFolder(AudioFolder)) AssetDatabase.CreateFolder("Assets/BrickGame", "Audio");

        if (!File.Exists(FanfarePath))
        {
            int sr = 44100;
            float noteDur = 0.14f;
            float[] freqs = { 523.25f, 659.25f, 783.99f, 1046.5f };
            int total = (int)(sr * noteDur * freqs.Length);
            float[] samples = new float[total];

            for (int n = 0; n < freqs.Length; n++)
            {
                int start = (int)(sr * noteDur * n);
                int len = (int)(sr * noteDur);
                for (int i = 0; i < len; i++)
                {
                    float t = (float)i / sr;
                    float env = Mathf.Exp(-6f * t) * (1f - (float)i / len);
                    float v = Mathf.Sin(2f * Mathf.PI * freqs[n] * t) * 0.5f;
                    v += Mathf.Sin(2f * Mathf.PI * freqs[n] * 2f * t) * 0.15f;
                    int idx = start + i;
                    if (idx < total) samples[idx] += v * env * 0.6f;
                }
            }

            WriteWav(FanfarePath, samples, sr);
            AssetDatabase.ImportAsset(FanfarePath);
        }

        return AssetDatabase.LoadAssetAtPath<AudioClip>(FanfarePath);
    }

    private static void WriteWav(string path, float[] samples, int sampleRate)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create))
        using (BinaryWriter bw = new BinaryWriter(fs))
        {
            int byteCount = samples.Length * 2;
            bw.Write(new char[] { 'R', 'I', 'F', 'F' });
            bw.Write(36 + byteCount);
            bw.Write(new char[] { 'W', 'A', 'V', 'E' });
            bw.Write(new char[] { 'f', 'm', 't', ' ' });
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)1);
            bw.Write(sampleRate);
            bw.Write(sampleRate * 2);
            bw.Write((short)2);
            bw.Write((short)16);
            bw.Write(new char[] { 'd', 'a', 't', 'a' });
            bw.Write(byteCount);

            foreach (float s in samples)
            {
                short val = (short)(Mathf.Clamp(s, -1f, 1f) * short.MaxValue);
                bw.Write(val);
            }
        }
    }

    private static void Cleanup(string name, Transform parent)
    {
        Transform t = parent.Find(name);
        if (t != null) Object.DestroyImmediate(t.gameObject);
    }

    private static void DestroyIfExists(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go != null) Object.DestroyImmediate(go);
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
