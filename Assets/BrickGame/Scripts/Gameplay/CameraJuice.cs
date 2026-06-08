using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class CameraJuice : MonoBehaviour
{
    public static CameraJuice Instance;

    private Camera cam;
    private float baseSize;
    private bool captured;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        Capture();
    }

    public void Capture()
    {
        if (cam == null) cam = GetComponent<Camera>();
        baseSize = cam.orthographicSize;
        captured = true;
    }

    public void Punch(float strength)
    {
        if (!captured) Capture();

        float amount = Mathf.Clamp01(strength);
        float target = baseSize * (1f - 0.06f * amount);

        cam.DOKill();
        Sequence s = DOTween.Sequence().SetUpdate(true);
        s.Append(cam.DOOrthoSize(target, 0.07f).SetEase(Ease.OutQuad));
        s.Append(cam.DOOrthoSize(baseSize, 0.28f).SetEase(Ease.OutQuad));
    }
}
