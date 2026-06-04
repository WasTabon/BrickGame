using UnityEngine;

public class UpgradeApplier : MonoBehaviour
{
    public Rope rope;
    public DemolitionController demolition;

    public float baseForce = 60f;
    public float baseRamp = 0.5f;
    public float basePullDuration = 1.0f;

    private void Awake()
    {
        if (rope != null)
        {
            rope.maxPullForce = baseForce + Upgrades.ForceBonus();
            rope.rampTime = Mathf.Max(0.12f, baseRamp - Upgrades.RampReduction());
        }

        if (demolition != null)
        {
            demolition.pullDuration = basePullDuration + Upgrades.DurationBonus();
        }
    }
}
