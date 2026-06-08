using UnityEngine;
using DG.Tweening;

public class PitJuice : MonoBehaviour
{
    public CollectionPit pit;
    public Transform floor;
    public Transform postL;
    public Transform postR;
    public Sprite rippleSprite;

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

        if (HapticManager.Instance != null) HapticManager.Instance.Light();

        Pulse(floor, floorBase);
        Pulse(postL, postLBase);
        Pulse(postR, postRBase);

        SpawnRipple(position);
    }

    private void SpawnRipple(Vector2 position)
    {
        if (rippleSprite == null) return;

        GameObject go = new GameObject("Ripple");
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = rippleSprite;
        sr.color = new Color(1f, 0.85f, 0.4f, 0.8f);
        sr.sortingOrder = 5;
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.3f;

        go.transform.DOScale(1.6f, 0.4f).SetEase(Ease.OutQuad);
        sr.DOFade(0f, 0.4f).SetEase(Ease.InQuad);
        Object.Destroy(go, 0.45f);
    }

    private void Pulse(Transform t, Vector3 baseScale)
    {
        if (t == null) return;
        t.DOKill();
        t.localScale = baseScale;
        t.DOPunchScale(baseScale * 0.25f, 0.3f, 6, 1f);
    }
}
