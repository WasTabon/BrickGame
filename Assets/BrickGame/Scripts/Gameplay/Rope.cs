using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{
    public Transform truckAnchor;
    public float maxPullForce = 60f;
    public float rampTime = 0.5f;
    public float massCap = 3f;

    private LineRenderer line;
    private Rigidbody2D target;
    private bool pulling;
    private float pullTimer;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.enabled = false;
    }

    public void Attach(Rigidbody2D body)
    {
        target = body;
        pulling = true;
        pullTimer = 0f;
        line.enabled = true;
    }

    public void Release()
    {
        pulling = false;
        line.enabled = false;
        target = null;
    }

    private void Update()
    {
        if (target == null)
        {
            line.enabled = false;
            return;
        }

        line.enabled = true;
        line.SetPosition(0, truckAnchor.position);
        line.SetPosition(1, target.worldCenterOfMass);
    }

    private void FixedUpdate()
    {
        if (!pulling || target == null) return;

        pullTimer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(pullTimer / rampTime);

        Vector2 dir = (Vector2)truckAnchor.position - target.worldCenterOfMass;
        dir.Normalize();

        float massFactor = Mathf.Min(target.mass, massCap);
        target.AddForce(dir * maxPullForce * t * massFactor, ForceMode2D.Force);
    }
}
