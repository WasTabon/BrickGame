using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectionPit : MonoBehaviour
{
    public int totalBricks;

    public event Action<int> OnCountChanged;

    private readonly HashSet<Brick> inside = new HashSet<Brick>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Brick brick = other.GetComponent<Brick>();
        if (brick == null) return;

        if (inside.Add(brick))
        {
            OnCountChanged?.Invoke(inside.Count);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Brick brick = other.GetComponent<Brick>();
        if (brick == null) return;

        if (inside.Remove(brick))
        {
            OnCountChanged?.Invoke(inside.Count);
        }
    }

    public int CurrentCount()
    {
        return inside.Count;
    }
}
