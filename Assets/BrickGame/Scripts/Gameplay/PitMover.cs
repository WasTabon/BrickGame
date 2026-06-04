using UnityEngine;

public class PitMover : MonoBehaviour
{
    public float baseX;
    public float amplitude = 1.2f;
    public float speed = 1.2f;
    public float phase;

    private void Update()
    {
        Vector3 p = transform.position;
        p.x = baseX + Mathf.Sin(Time.time * speed + phase) * amplitude;
        transform.position = p;
    }
}
