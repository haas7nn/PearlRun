using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    public float slowMultiplier = 0.35f;
    public float slowDuration = 0.45f;

    private bool hasDamagedPlayer;

    void OnTriggerEnter(Collider other)
    {
        if (hasDamagedPlayer)
            return;

        RunnerCollisionHandler ch = other.GetComponent<RunnerCollisionHandler>()
                                 ?? other.GetComponentInParent<RunnerCollisionHandler>();
        if (ch == null)
            return;

        hasDamagedPlayer = true;
        ch.HitByObstacle(slowMultiplier, slowDuration);
    }

    public void ResetDamageFlag()
    {
        hasDamagedPlayer = false;
    }
}