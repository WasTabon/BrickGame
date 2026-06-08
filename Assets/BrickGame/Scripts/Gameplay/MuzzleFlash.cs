using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public Sprite sprite;
    public Color color = new Color(1f, 0.8f, 0.3f, 1f);

    public void Flash()
    {
        Vfx.Flash(sprite, transform.position, color, 0.9f, 0.16f, 70);
        Vfx.Sparks(sprite, transform.position, new Color(0.6f, 0.6f, 0.6f, 0.7f), 3, 0.5f);
    }
}
