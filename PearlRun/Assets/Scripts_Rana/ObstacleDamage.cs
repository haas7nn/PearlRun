using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    public float slowMultiplier = 0.35f;
    public float slowDuration = 0.45f;
    private bool hasDamagedPlayer;

    void OnTriggerEnter(Collider other)
    {
        if (hasDamagedPlayer) return;

        RunnerCollisionHandler ch = other.GetComponent<RunnerCollisionHandler>()
                                 ?? other.GetComponentInParent<RunnerCollisionHandler>();
        if (ch == null) return;

        // if player is above this obstacle, they jumped over it cleanly — do nothing
        Collider myCol = GetComponent<Collider>();
        float obstacleTop = myCol.bounds.max.y;
        float playerBottom = other.bounds.min.y;

        if (playerBottom >= obstacleTop - 0.15f)
            return;

        hasDamagedPlayer = true;
        ch.HitByObstacle(slowMultiplier, slowDuration);
    }

    public void ResetDamageFlag()
    {
        hasDamagedPlayer = false;
    }
}