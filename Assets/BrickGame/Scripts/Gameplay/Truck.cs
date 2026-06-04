using UnityEngine;
using DG.Tweening;

public class Truck : MonoBehaviour
{
    public Transform anchor;

    private Vector3 basePos;

    private void Awake()
    {
        basePos = transform.position;
    }

    public void Recoil()
    {
        transform.DOKill();
        transform.position = basePos;
        transform.DOPunchPosition(new Vector3(-0.25f, 0f, 0f), 0.4f, 6, 0.6f);
    }
}
