using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Sprite squareSprite;
    public PhysicsMaterial2D brickPhysics;
    public PhysicsMaterial2D icePhysics;
    public GameHUDController hud;
    public DemolitionController demolition;
    public Camera cam;
    public List<LevelDef> levels = new List<LevelDef>();

    public float brickW = 0.62f;
    public float brickH = 0.34f;
    public float supportH = 0.6f;
    public float colGap = 0.9f;
    public float gap = 0.002f;
    public float buildingBaseX = 3.5f;
    public float truckX = -6f;
    public float pitHalfWidth = 0.8f;
    public float[] pitCentersSingle = { 1.2f };
    public float[] pitCentersDouble = { -0.3f, 2.4f };

    private readonly List<CollectionPit> pits = new List<CollectionPit>();

    public int TotalBricks { get; private set; }

    private static readonly Color ColorSupport = HexColor("8E5BC9");
    private static readonly Color ColorLight = HexColor("6BA8F0");
    private static readonly Color ColorDark = HexColor("3A6FB0");
    private static readonly Color ColorBeam = HexColor("F5A623");
    private static readonly Color ColorStone = HexColor("6E6E78");
    private static readonly Color ColorWood = HexColor("9B6B3A");
    private static readonly Color ColorIce = HexColor("8FD8E8");
    private static readonly Color ColorBomb = HexColor("C0392B");
    private static readonly Color ColorBrittle = HexColor("CFC2A8");
    private static readonly Color ColorSticky = HexColor("6FCF6F");

    private void Awake()
    {
        if (cam == null) cam = Camera.main;

        int level = Mathf.Max(1, GameSession.Level);
        int index = levels.Count > 0 ? Mathf.Clamp(level - 1, 0, levels.Count - 1) : 0;
        LevelDef def = levels.Count > 0 ? levels[index] : new LevelDef();

        BuildPits(def);
        TotalBricks = BuildTower(def);

        foreach (CollectionPit pit in pits)
        {
            pit.OnCountChanged += OnPitChanged;
        }

        if (hud != null)
        {
            hud.SetLevel(level);
            hud.SetTotal(TotalBricks);
            hud.SetCount(0);
        }

        if (demolition != null) demolition.maxThrows = def.maxThrows;
        if (hud != null) hud.SetThrows(def.maxThrows);

        FitCamera(def);
    }

    private void OnDestroy()
    {
        foreach (CollectionPit pit in pits)
        {
            if (pit != null) pit.OnCountChanged -= OnPitChanged;
        }
    }

    private void OnPitChanged(int ignored)
    {
        if (hud != null) hud.SetCount(TotalCollected());
    }

    public int TotalCollected()
    {
        int sum = 0;
        foreach (CollectionPit pit in pits)
        {
            if (pit != null) sum += pit.CurrentCount();
        }
        return sum;
    }

    private void BuildPits(LevelDef def)
    {
        float[] centers = def.pitCount >= 2 ? pitCentersDouble : pitCentersSingle;
        if (centers == null || centers.Length == 0)
        {
            centers = def.pitCount >= 2 ? new float[] { -0.3f, 2.4f } : new float[] { 1.2f };
        }

        foreach (float cx in centers)
        {
            pits.Add(BuildPit(cx, def));
        }
    }

    private CollectionPit BuildPit(float centerX, LevelDef def)
    {
        float halfWidth = pitHalfWidth + Upgrades.MagnetBonus();
        GameObject group = new GameObject("PitGroup");
        group.transform.position = new Vector3(centerX, 0f, 0f);
        GameObject visual = new GameObject("PitVisual");
        visual.transform.SetParent(group.transform, true);

        GameObject floor = NewSprite("Floor", new Color(0.96f, 0.65f, 0.14f, 0.45f), new Vector3(centerX, 0.06f, 0f), new Vector3(halfWidth * 2f, 0.25f, 1f), -3);
        floor.transform.SetParent(visual.transform, true);
        GameObject postL = NewSprite("PostL", ColorBeam, new Vector3(centerX - halfWidth, 0.55f, 0f), new Vector3(0.14f, 1.1f, 1f), -2);
        postL.transform.SetParent(visual.transform, true);
        GameObject postR = NewSprite("PostR", ColorBeam, new Vector3(centerX + halfWidth, 0.55f, 0f), new Vector3(0.14f, 1.1f, 1f), -2);
        postR.transform.SetParent(visual.transform, true);

        GameObject pitGo = new GameObject("CollectionPit");
        pitGo.transform.position = new Vector3(centerX, 2f, 0f);
        BoxCollider2D trigger = pitGo.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = new Vector2(halfWidth * 2f, 4f);
        CollectionPit pit = pitGo.AddComponent<CollectionPit>();

        pitGo.transform.SetParent(group.transform, true);

        PitJuice juice = visual.AddComponent<PitJuice>();
        juice.pit = pit;
        juice.floor = floor.transform;
        juice.postL = postL.transform;
        juice.postR = postR.transform;

        if (def.pitMoveAmplitude > 0f)
        {
            PitMover mover = group.AddComponent<PitMover>();
            mover.baseX = centerX;
            mover.amplitude = def.pitMoveAmplitude;
            mover.speed = def.pitMoveSpeed;
        }

        return pit;
    }

    private int BuildTower(LevelDef def)
    {
        GameObject building = new GameObject("Building");

        int columns = Mathf.Clamp(def.columns, 1, 4);
        float totalWidth = (columns - 1) * colGap;
        float startX = buildingBaseX - totalWidth * 0.5f;
        float[] colX = new float[columns];
        for (int i = 0; i < columns; i++) colX[i] = startX + i * colGap;

        int count = 0;
        GameObject[] lastInColumn = new GameObject[columns];

        for (int i = 0; i < columns; i++)
        {
            lastInColumn[i] = BuildBrick(building.transform, new Vector3(colX[i], supportH * 0.5f, 0f), new Vector3(brickW, supportH, 1f), Brick.BrickMaterial.Normal, ColorSupport);
            count++;
        }

        float currentTop = supportH;
        int brickOrdinal = 0;

        for (int r = 0; r < def.rows; r++)
        {
            float yc = currentTop + gap + brickH * 0.5f;
            bool beamRow = r == 4 || r == 9 || r == 14 || r == 19 || r == 24;

            if (beamRow)
            {
                GameObject beam = BuildBrick(building.transform, new Vector3(buildingBaseX, yc, 0f), new Vector3(totalWidth + brickW, brickH, 1f), Brick.BrickMaterial.Normal, ColorBeam);
                for (int i = 0; i < columns; i++) lastInColumn[i] = beam;
                count++;
            }
            else
            {
                for (int i = 0; i < columns; i++)
                {
                    brickOrdinal++;
                    Brick.BrickMaterial mat = PickMaterial(def, r, brickOrdinal);
                    Color c = MaterialColor(mat, r, def.rows);
                    GameObject b = BuildBrick(building.transform, new Vector3(colX[i], yc, 0f), new Vector3(brickW, brickH, 1f), mat, c);

                    if (mat == Brick.BrickMaterial.Sticky && lastInColumn[i] != null)
                    {
                        Rigidbody2D belowRb = lastInColumn[i].GetComponent<Rigidbody2D>();
                        if (belowRb != null)
                        {
                            FixedJoint2D joint = b.AddComponent<FixedJoint2D>();
                            joint.connectedBody = belowRb;
                            joint.autoConfigureConnectedAnchor = true;
                            joint.breakForce = 250f;
                            joint.breakTorque = 250f;
                        }
                    }

                    lastInColumn[i] = b;
                    count++;
                }
            }

            currentTop = yc + brickH * 0.5f;
        }

        return count;
    }

    private Brick.BrickMaterial PickMaterial(LevelDef def, int row, int ordinal)
    {
        if (def.iceRows > 0 && row < def.iceRows) return Brick.BrickMaterial.Ice;
        if (def.bombEvery > 0 && ordinal % def.bombEvery == 0) return Brick.BrickMaterial.Bomb;
        if (def.stoneEvery > 0 && ordinal % def.stoneEvery == 0) return Brick.BrickMaterial.Stone;
        if (def.woodEvery > 0 && ordinal % def.woodEvery == 0) return Brick.BrickMaterial.Wood;
        if (def.brittleEvery > 0 && ordinal % def.brittleEvery == 0) return Brick.BrickMaterial.Brittle;
        if (def.stickyEvery > 0 && ordinal % def.stickyEvery == 0) return Brick.BrickMaterial.Sticky;
        return Brick.BrickMaterial.Normal;
    }

    private Color MaterialColor(Brick.BrickMaterial mat, int row, int rows)
    {
        switch (mat)
        {
            case Brick.BrickMaterial.Ice: return ColorIce;
            case Brick.BrickMaterial.Stone: return ColorStone;
            case Brick.BrickMaterial.Wood: return ColorWood;
            case Brick.BrickMaterial.Bomb: return ColorBomb;
            case Brick.BrickMaterial.Brittle: return ColorBrittle;
            case Brick.BrickMaterial.Sticky: return ColorSticky;
            default: return Color.Lerp(ColorDark, ColorLight, (float)row / Mathf.Max(1, rows));
        }
    }

    private GameObject BuildBrick(Transform parent, Vector3 pos, Vector3 scale, Brick.BrickMaterial mat, Color color)
    {
        GameObject go = new GameObject("Brick");
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = squareSprite;
        sr.color = color;
        sr.sortingOrder = 0;
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.transform.SetParent(parent, true);

        float massFactor = 8f;
        if (mat == Brick.BrickMaterial.Stone) massFactor = 24f;
        else if (mat == Brick.BrickMaterial.Wood) massFactor = 4f;
        else if (mat == Brick.BrickMaterial.Bomb) massFactor = 10f;
        else if (mat == Brick.BrickMaterial.Brittle) massFactor = 4f;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2.0f;
        rb.mass = scale.x * scale.y * massFactor;
        rb.drag = 0.2f;
        rb.angularDrag = 0.8f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
        col.sharedMaterial = (mat == Brick.BrickMaterial.Ice && icePhysics != null) ? icePhysics : brickPhysics;

        Brick brick = go.AddComponent<Brick>();
        brick.material = mat;

        return go;
    }

    private void FitCamera(LevelDef def)
    {
        if (cam == null) return;

        float aspect = 0.5625f;
        int columns = Mathf.Clamp(def.columns, 1, 4);
        float towerRight = buildingBaseX + (columns - 1) * colGap * 0.5f + brickW;
        float left = truckX - 1.5f;
        float right = towerRight + 1.5f;
        float worldWidth = right - left;

        float sizeW = (worldWidth * 0.5f) / aspect;
        float towerTop = supportH + def.rows * (brickH + gap);
        float sizeH = (towerTop + 2f) * 0.5f + 1.5f;
        float size = Mathf.Max(sizeW, sizeH);

        cam.orthographicSize = size;
        cam.transform.position = new Vector3((left + right) * 0.5f, size - 3f, -10f);
    }

    private GameObject NewSprite(string name, Color color, Vector3 pos, Vector3 scale, int order)
    {
        GameObject go = new GameObject(name);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = squareSprite;
        sr.color = color;
        sr.sortingOrder = order;
        go.transform.position = pos;
        go.transform.localScale = scale;
        return go;
    }

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
