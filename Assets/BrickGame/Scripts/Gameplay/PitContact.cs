using UnityEngine;

public class PitContact : MonoBehaviour
{
    public CollectionPit pit;
    public Collider2D floorCollider;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider != floorCollider) return;

        Brick brick = collision.collider.GetComponent<Brick>();
        if (brick != null && pit != null) pit.Collect(brick);
    }
}
