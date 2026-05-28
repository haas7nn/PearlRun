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

        RunnerCollisionHandler collisionHandler = other.GetComponent<RunnerCollisionHandler>();

        if (collisionHandler == null)
            collisionHandler = other.GetComponentInParent<RunnerCollisionHandler>();

        if (collisionHandler != null)
        {
            hasDamagedPlayer = true;
            collisionHandler.HitByObstacle(slowMultiplier, slowDuration);
        }
    }
}