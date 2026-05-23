using System.Collections;
using UnityEngine;

public class RunnerCollisionHandler : MonoBehaviour
{
    private RunnerController runnerController;

    [Header("Invincibility")]
    public float invincibilityTime = 1.5f;
    private bool isInvincible = false;

    [Header("Jump Obstacles")]
    public string jumpObstacleTag = "JumpObstacle";
    public float obstacleSlowMultiplier = 0.35f;
    public float obstacleSlowDuration = 0.45f;

    [Header("Landing Check")]
    public float topHitNormalY = 0.45f;

    void Start()
    {
        runnerController = GetComponent<RunnerController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("KillZone"))
        {
            RunnerGameManager.instance?.PlayerDied();
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            DamagePlayer(false);
            return;
        }

        if (collision.gameObject.CompareTag(jumpObstacleTag))
        {
            HandleJumpObstacleCollision(collision);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone"))
        {
            RunnerGameManager.instance?.PlayerDied();
            return;
        }

        if (other.CompareTag("Finish"))
        {
            RunnerGameManager.instance?.LevelComplete();
            return;
        }

        if (other.CompareTag("Checkpoint"))
        {
            RunnerGameManager.instance?.SetCheckpoint(transform.position);
            return;
        }
    }

    void HandleJumpObstacleCollision(Collision collision)
    {
        bool landedOnTop = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > topHitNormalY)
            {
                landedOnTop = true;
                break;
            }
        }

        // Clean landing on top — no damage, no nudge
        if (landedOnTop) return;

        // Direct damage only — bypass invincibility system entirely
        if (runnerController != null && !runnerController.isDead)
            runnerController.TakeDamage();

        // Snap to top of obstacle
        Collider obstacleCol = collision.collider;
        float obstacleTop = obstacleCol.bounds.max.y;
        Collider myCol = GetComponent<Collider>();
        float halfHeight = myCol != null ? myCol.bounds.extents.y : 0.9f;

        Vector3 pos = transform.position;
        pos.y = obstacleTop + halfHeight;
        transform.position = pos;

        // Kill vertical velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;
        }

        // Reset all jump/hurt states instantly
        runnerController?.ForceGrounded();

        // Kill any running invincibility coroutine
        StopAllCoroutines();
        Renderer playerRenderer = GetComponentInChildren<Renderer>();
        if (playerRenderer != null) playerRenderer.enabled = true;
        isInvincible = false;
    }

    void HandleJumpObstacleTrigger()
    {
        if (runnerController == null || runnerController.isDead) return;
        if (isInvincible) return;

        runnerController.ApplyObstacleSlowdown(obstacleSlowMultiplier, obstacleSlowDuration);
        StartCoroutine(InvincibilityFrames());
    }

    void DamagePlayer(bool applySlowdown)
    {
        if (isInvincible) return;
        if (runnerController == null || runnerController.isDead) return;

        runnerController.TakeDamage();
        if (applySlowdown)
            runnerController.ApplyObstacleSlowdown(obstacleSlowMultiplier, obstacleSlowDuration);

        StartCoroutine(InvincibilityFrames());
    }

    public void HitByObstacle(float slowMultiplier, float slowDuration)
    {
        if (isInvincible) return;
        if (runnerController == null || runnerController.isDead) return;

        runnerController.TakeDamage();
        runnerController.ApplyObstacleSlowdown(slowMultiplier, slowDuration);
        StartCoroutine(InvincibilityFrames());
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        Renderer playerRenderer = GetComponentInChildren<Renderer>();

        if (playerRenderer != null)
        {
            float flashTimer = 0f;
            while (flashTimer < invincibilityTime)
            {
                playerRenderer.enabled = !playerRenderer.enabled;
                yield return new WaitForSeconds(0.1f);
                flashTimer += 0.1f;
            }
            playerRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invincibilityTime);
        }

        isInvincible = false;
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }
}