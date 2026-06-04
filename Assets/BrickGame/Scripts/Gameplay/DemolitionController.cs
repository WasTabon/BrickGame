using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DemolitionController : MonoBehaviour
{
    public Camera cam;
    public Rope rope;
    public Truck truck;

    public float pullDuration = 1.0f;
    public float settleDelay = 0.5f;

    public GameHUDController hud;
    public LevelFlowController flow;
    public int maxThrows = 0;

    private bool busy;
    private bool depleted;
    private int throwsUsed;
    private Brick[] allBricks;

    private void Start()
    {
        if (cam == null) cam = Camera.main;
        allBricks = FindObjectsByType<Brick>(FindObjectsSortMode.None);
    }

    private void Update()
    {
        if (busy || depleted) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (IsPointerOverUI()) return;

        Vector2 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        Brick nearest = FindNearestBrick(worldPoint);

        if (nearest != null)
        {
            if (maxThrows > 0)
            {
                throwsUsed++;
                if (hud != null) hud.SetThrowsLeft(maxThrows - throwsUsed);
                if (throwsUsed >= maxThrows) depleted = true;
            }
            StartCoroutine(PullRoutine(nearest));
        }
    }

    private Brick FindNearestBrick(Vector2 point)
    {
        Brick nearest = null;
        float best = float.MaxValue;

        foreach (Brick brick in allBricks)
        {
            if (brick == null) continue;

            float sqr = ((Vector2)brick.transform.position - point).sqrMagnitude;
            if (sqr < best)
            {
                best = sqr;
                nearest = brick;
            }
        }

        return nearest;
    }

    private IEnumerator PullRoutine(Brick brick)
    {
        busy = true;

        brick.Highlight();
        truck.Recoil();
        rope.Attach(brick.body);
        if (HapticManager.Instance != null) HapticManager.Instance.Medium();

        yield return new WaitForSeconds(pullDuration);

        rope.Release();

        yield return new WaitForSeconds(settleDelay);

        busy = false;

        if (depleted && maxThrows > 0 && flow != null)
        {
            flow.ForceCollect();
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }

        return false;
    }
}
