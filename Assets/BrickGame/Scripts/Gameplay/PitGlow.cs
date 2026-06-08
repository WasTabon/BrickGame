using UnityEngine;

public class PitGlow : MonoBehaviour
{
    public SpriteRenderer postL;
    public SpriteRenderer postR;
    public SpriteRenderer floor;
    public float halfWidth = 0.8f;

    private Color postBase;
    private Color floorBase;
    private float glow;

    private void Start()
    {
        if (postL != null) postBase = postL.color;
        if (floor != null) floorBase = floor.color;
    }

    private void Update()
    {
        bool overhead = BrickOverhead();
        float targetGlow = overhead ? 1f : 0f;
        glow = Mathf.MoveTowards(glow, targetGlow, Time.deltaTime * 4f);

        float pulse = overhead ? (0.5f + 0.5f * Mathf.Sin(Time.time * 10f)) : 0f;
        float k = glow * (0.5f + 0.5f * pulse);

        if (postL != null) postL.color = Color.Lerp(postBase, Color.white, k);
        if (postR != null) postR.color = Color.Lerp(postBase, Color.white, k);
        if (floor != null) floor.color = Color.Lerp(floorBase, Color.white, k * 0.6f);
    }

    private bool BrickOverhead()
    {
        Vector2 center = new Vector2(transform.position.x, transform.position.y + 1.1f);
        Vector2 size = new Vector2(halfWidth * 2f, 1.8f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);
        foreach (Collider2D c in hits)
        {
            if (c.GetComponent<Brick>() != null) return true;
        }
        return false;
    }
}
