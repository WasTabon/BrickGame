#if BRICKGAME_URP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;

public class BrickGameSetup_Iteration19_URP
{
    private const string RenderFolder = "Assets/BrickGame/Rendering";
    private const string ProfilePath = "Assets/BrickGame/Rendering/BrickGamePostFX.asset";

    private static readonly string[] Scenes =
    {
        "Assets/BrickGame/Scenes/Game.unity",
        "Assets/BrickGame/Scenes/Battle.unity",
        "Assets/BrickGame/Scenes/MainMenu.unity",
        "Assets/BrickGame/Scenes/LevelSelect.unity"
    };

    [MenuItem("BrickGame/Setup URP Post FX")]
    public static void Setup()
    {
        VolumeProfile profile = CreateProfile();

        foreach (string scenePath in Scenes)
        {
            ConfigureScene(scenePath, profile);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame URP Post FX setup complete. Bloom/Vignette/Chromatic Aberration/Color Grading added to all scenes.");
    }

    private static VolumeProfile CreateProfile()
    {
        if (!AssetDatabase.IsValidFolder(RenderFolder))
            AssetDatabase.CreateFolder("Assets/BrickGame", "Rendering");

        VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(ProfilePath);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, ProfilePath);
        }

        Bloom bloom = GetOrAdd<Bloom>(profile);
        bloom.active = true;
        bloom.intensity.overrideState = true;
        bloom.intensity.value = 0.9f;
        bloom.threshold.overrideState = true;
        bloom.threshold.value = 0.85f;
        bloom.scatter.overrideState = true;
        bloom.scatter.value = 0.7f;

        Vignette vignette = GetOrAdd<Vignette>(profile);
        vignette.active = true;
        vignette.intensity.overrideState = true;
        vignette.intensity.value = 0.28f;
        vignette.smoothness.overrideState = true;
        vignette.smoothness.value = 0.4f;

        ChromaticAberration ca = GetOrAdd<ChromaticAberration>(profile);
        ca.active = true;
        ca.intensity.overrideState = true;
        ca.intensity.value = 0.08f;

        ColorAdjustments color = GetOrAdd<ColorAdjustments>(profile);
        color.active = true;
        color.saturation.overrideState = true;
        color.saturation.value = 12f;
        color.contrast.overrideState = true;
        color.contrast.value = 8f;
        color.postExposure.overrideState = true;
        color.postExposure.value = 0.05f;

        EditorUtility.SetDirty(profile);
        return profile;
    }

    private static T GetOrAdd<T>(VolumeProfile profile) where T : VolumeComponent
    {
        if (profile.TryGet(out T existing)) return existing;
        return profile.Add<T>(true);
    }

    private static void ConfigureScene(string scenePath, VolumeProfile profile)
    {
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject volGo = GameObject.Find("Global Volume");
        if (volGo == null)
        {
            volGo = new GameObject("Global Volume");
        }
        Volume volume = volGo.GetComponent<Volume>();
        if (volume == null) volume = volGo.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 1f;
        volume.sharedProfile = profile;

        Camera cam = Camera.main;
        if (cam != null)
        {
            UniversalAdditionalCameraData data = cam.GetUniversalAdditionalCameraData();
            if (data != null) data.renderPostProcessing = true;
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }
}
#endif
