using UnityEngine;
using DG.Tweening;

public static class Vfx
{
    public static void Flash(Sprite sprite, Vector3 pos, Color color, float size, float dur, int order)
    {
        if (sprite == null) return;

        GameObject go = new GameObject("Flash");
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingOrder = order;
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * size * 0.3f;

        go.transform.DOScale(size, dur).SetEase(Ease.OutQuad);
        sr.DOFade(0f, dur).SetEase(Ease.InQuad);
        Object.Destroy(go, dur + 0.05f);
    }

    public static void Sparks(Sprite sprite, Vector3 pos, Color color, int count, float spread)
    {
        if (sprite == null) return;

        for (int i = 0; i < count; i++)
        {
            GameObject go = new GameObject("Spark");
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = 65;
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * Random.Range(0.12f, 0.22f);

            Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
            float dist = spread * Random.Range(0.5f, 1.2f);
            float dur = Random.Range(0.25f, 0.45f);

            go.transform.DOMove(pos + dir * dist, dur).SetEase(Ease.OutQuad);
            go.transform.DOScale(0f, dur).SetEase(Ease.InQuad);
            sr.DOFade(0f, dur).SetEase(Ease.InQuad);
            Object.Destroy(go, dur + 0.05f);
        }
    }
}
