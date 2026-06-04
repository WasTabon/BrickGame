using UnityEngine;
using DG.Tweening;

public class PitJuice : MonoBehaviour
{
    public CollectionPit pit;
    public Transform floor;
    public Transform postL;
    public Transform postR;

    private Vector3 floorBase;
    private Vector3 postLBase;
    private Vector3 postRBase;
    private float lastTime;

    private void Start()
    {
        if (floor != null) floorBase = floor.localScale;
        if (postL != null) postLBase = postL.localScale;
        if (postR != null) postRBase = postR.localScale;

        if (pit != null) pit.OnBrickEntered += OnBrick;
    }

    private void OnDestroy()
    {
        if (pit != null) pit.OnBrickEntered -= OnBrick;
    }

    private void OnBrick(Vector2 position)
    {
        if (Time.time - lastTime < 0.05f) return;
        lastTime = Time.time;

        Pulse(floor, floorBase);
        Pulse(postL, postLBase);
        Pulse(postR, postRBase);
    }

    private void Pulse(Transform t, Vector3 baseScale)
    {
        if (t == null) return;
        t.DOKill();
        t.localScale = baseScale;
        t.DOPunchScale(baseScale * 0.25f, 0.3f, 6, 1f);
    }
}
