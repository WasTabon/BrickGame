using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PitMover : MonoBehaviour
{
    public float baseX;
    public float amplitude = 1.2f;
    public float speed = 1.2f;
    public float phase;

    private Rigidbody2D rb;
    private float baseY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseY = transform.position.y;
    }

    private void FixedUpdate()
    {
        float x = baseX + Mathf.Sin(Time.time * speed + phase) * amplitude;
        rb.MovePosition(new Vector2(x, baseY));
    }
}
