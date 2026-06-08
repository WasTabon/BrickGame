using UnityEngine;

public class ImpactManager : MonoBehaviour
{
    public static ImpactManager Instance;

    public ParticleSystem dustPrefab;
    public CameraShake cameraShake;
    public AudioClip impactClip;

    public int poolSize = 12;
    public float soundInterval = 0.06f;
    public float shakeInterval = 0.15f;
    public float shakeThreshold = 6f;
    public float slowMoThreshold = 11f;
    public float freezeThreshold = 16f;
    public float cameraZoomThreshold = 9f;

    private ParticleSystem[] pool;
    private int index;
    private float lastSound;
    private float lastShake;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        pool = new ParticleSystem[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem ps = Instantiate(dustPrefab, transform);
            ps.gameObject.SetActive(true);
            pool[i] = ps;
        }
    }

    public void RegisterImpact(Vector2 position, float strength)
    {
        ParticleSystem ps = pool[index];
        index = (index + 1) % pool.Length;
        ps.transform.position = position;
        ps.Play();

        if (Time.time - lastSound > soundInterval)
        {
            lastSound = Time.time;
            float pitch = Random.Range(0.88f, 1.12f);
            float volume = Mathf.Clamp01(strength / 10f) * 0.7f + 0.3f;
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySfx(impactClip, pitch, volume);
            }
        }

        if (strength > shakeThreshold && Time.time - lastShake > shakeInterval)
        {
            lastShake = Time.time;
            float s = Mathf.Clamp(strength / 30f, 0.05f, 0.3f);
            if (cameraShake != null)
            {
                cameraShake.Shake(s);
            }
        }

        if (strength >= freezeThreshold && SlowMoController.Instance != null)
        {
            SlowMoController.Instance.Freeze();
        }
        else if (strength >= slowMoThreshold && SlowMoController.Instance != null)
        {
            SlowMoController.Instance.Request();
        }

        if (strength >= cameraZoomThreshold && CameraJuice.Instance != null)
        {
            CameraJuice.Instance.Punch(strength / 25f);
        }
    }
}
