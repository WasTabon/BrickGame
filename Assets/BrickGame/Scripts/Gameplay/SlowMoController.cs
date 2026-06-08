using System.Collections;
using UnityEngine;

public class SlowMoController : MonoBehaviour
{
    public static SlowMoController Instance;

    public float scale = 0.35f;
    public float duration = 0.25f;
    public float cooldown = 1.5f;

    private float lastTime = -99f;
    private bool active;
    private bool frozen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Freeze(float dur = 0.05f)
    {
        if (frozen || active) return;
        if (Time.timeScale == 0f) return;
        StartCoroutine(FreezeRun(dur));
    }

    private System.Collections.IEnumerator FreezeRun(float dur)
    {
        frozen = true;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(dur);
        if (Time.timeScale == 0f) Time.timeScale = 1f;
        frozen = false;
    }

    public void Request()
    {
        if (active) return;
        if (Time.unscaledTime - lastTime < cooldown) return;
        lastTime = Time.unscaledTime;
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        active = true;
        float baseFixed = Time.fixedDeltaTime;

        Time.timeScale = scale;
        Time.fixedDeltaTime = baseFixed * scale;

        yield return new WaitForSecondsRealtime(duration);

        if (Time.timeScale != 0f)
        {
            Time.timeScale = 1f;
        }
        Time.fixedDeltaTime = baseFixed;
        active = false;
    }
}
