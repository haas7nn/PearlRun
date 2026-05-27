using System.Collections;
using UnityEngine;

public class Level5AmwajCollisionHandler : MonoBehaviour
{
    private Level5AmwajPlayerController playerController;

    [Header("Invincibility")]
    public float invincibilityTime = 1.5f;
    private bool isInvincibleAmwaj = false;

    [Header("Jump Obstacles")]
    public string jumpObstacleTag = "JumpObstacle";
    public float obstacleSlowMultiplier = 0.35f;
    public float obstacleSlowDuration = 0.45f;

    [Header("Landing Check")]
    public float topHitNormalY = 0.45f;

    void Start()
    {
        playerController = GetComponent<Level5AmwajPlayerController>();

        if (playerController == null)
        {
            Debug.LogWarning("Level5AmwajCollisionHandler needs Level5AmwajPlayerController on the same Player object.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("KillZone"))
        {
            Level5AmwajRunnerGameManager.instance?.PlayerDied();
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            DamagePlayerAmwaj(false);
            return;
        }

        if (collision.gameObject.CompareTag(jumpObstacleTag))
        {
            HandleJumpObstacleCollisionAmwaj(collision);
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone"))
        {
            Level5AmwajRunnerGameManager.instance?.PlayerDied();
            return;
        }

        if (other.CompareTag("Finish"))
        {
            Level5AmwajRunnerGameManager.instance?.LevelComplete();
            return;
        }

        // Checkpoint saving is handled only by Level5AmwajCheckpoint.
    }

    void HandleJumpObstacleCollisionAmwaj(Collision collision)
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

        if (landedOnTop)
        {
            playerController?.ForceGrounded();
            return;
        }

        if (playerController != null && !playerController.isDead)
        {
            playerController.TakeDamage();
        }

        Collider obstacleCol = collision.collider;
        float obstacleTop = obstacleCol.bounds.max.y;

        Collider myCol = GetComponent<Collider>();
        float halfHeight = myCol != null ? myCol.bounds.extents.y : 0.9f;

        Vector3 pos = transform.position;
        pos.y = obstacleTop + halfHeight;
        transform.position = pos;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;
        }

        playerController?.ForceGrounded();

        StopAllCoroutines();

        Renderer playerRenderer = GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
        {
            playerRenderer.enabled = true;
        }

        isInvincibleAmwaj = false;
    }

    void DamagePlayerAmwaj(bool applySlowdown)
    {
        if (isInvincibleAmwaj)
            return;

        if (playerController == null || playerController.isDead)
            return;

        playerController.TakeDamage();

        if (applySlowdown)
        {
            playerController.ApplyObstacleSlowdown(obstacleSlowMultiplier, obstacleSlowDuration);
        }

        StartCoroutine(InvincibilityFramesAmwaj());
    }

    public void HitByObstacleAmwaj(float slowMultiplier, float slowDuration)
    {
        if (isInvincibleAmwaj)
            return;

        if (playerController == null || playerController.isDead)
            return;

        playerController.TakeDamage();
        playerController.ApplyObstacleSlowdown(slowMultiplier, slowDuration);

        StartCoroutine(InvincibilityFramesAmwaj());
    }

    IEnumerator InvincibilityFramesAmwaj()
    {
        isInvincibleAmwaj = true;

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

        isInvincibleAmwaj = false;
    }

    public void SetInvincibleAmwaj(bool value)
    {
        isInvincibleAmwaj = value;
    }
}