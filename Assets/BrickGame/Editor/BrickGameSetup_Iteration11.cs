using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class BrickGameSetup_Iteration11
{
    private const string GamePath = "Assets/BrickGame/Scenes/Game.unity";
    private const string IcePhysPath = "Assets/BrickGame/Textures/IcePhysics.physicsMaterial2D";

    private const float TruckX = -6f;
    private const float BuildingBaseX = 3.5f;
    private const float PitHalfWidth = 0.8f;

    [MenuItem("BrickGame/Setup Iteration 11 (Camera + Materials)")]
    public static void Setup()
    {
        PhysicsMaterial2D ice = GenerateIcePhysics();

        EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);

        GameObject truck = GameObject.Find("Truck");
        GameObject lmGo = GameObject.Find("LevelManager");
        if (truck == null || lmGo == null)
        {
            Debug.LogWarning("Truck / LevelManager not found. Run Setup Iteration 2 and 6 first.");
            return;
        }

        Vector3 tp = truck.transform.position;
        truck.transform.position = new Vector3(TruckX, tp.y, tp.z);

        LevelManager lm = lmGo.GetComponent<LevelManager>();
        lm.icePhysics = ice;
        lm.buildingBaseX = BuildingBaseX;
        lm.truckX = TruckX;
        lm.pitHalfWidth = PitHalfWidth;
        lm.levels = BuildLevels();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("BrickGame Iteration 11 setup complete. Truck moved, camera reframed, materials added.");
    }

    private static List<LevelDef> BuildLevels()
    {
        return new List<LevelDef>
        {
            new LevelDef { rows = 16, columns = 2, pitCount = 1 },
            new LevelDef { rows = 18, columns = 2, pitCount = 1, woodEvery = 5 },
            new LevelDef { rows = 20, columns = 2, pitCount = 1, stoneEvery = 6 },
            new LevelDef { rows = 22, columns = 3, pitCount = 2, stoneEvery = 6, iceRows = 3 },
            new LevelDef { rows = 24, columns = 3, pitCount = 2, stoneEvery = 5, bombEvery = 9 },
            new LevelDef { rows = 26, columns = 3, pitCount = 2, iceRows = 4, stoneEvery = 5, woodEvery = 7, bombEvery = 11 },
        };
    }

    private static PhysicsMaterial2D GenerateIcePhysics()
    {
        PhysicsMaterial2D mat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(IcePhysPath);
        if (mat == null)
        {
            mat = new PhysicsMaterial2D("IcePhysics");
            AssetDatabase.CreateAsset(mat, IcePhysPath);
        }
        mat.friction = 0.05f;
        mat.bounciness = 0f;
        EditorUtility.SetDirty(mat);
        return mat;
    }
}
