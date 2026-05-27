using System.Collections;
using UnityEngine;

public class Level3CollisionHandler : MonoBehaviour
{
    private Level3PlayerController playerController;

    [Header("Invincibility")]
    public float invincibilityTime = 1.5f;
    private bool isInvincible = false;

    [Header("Obstacles")]
    public string obstacleTag = "Obstacle";
    public float obstacleSlowMultiplier = 0.35f;
    public float obstacleSlowDuration = 0.45f;

    void Start()
    {
        playerController = GetComponent<Level3PlayerController>();

        if (playerController == null)
        {
            Debug.LogWarning("Level3CollisionHandler needs Level3PlayerController on the same Player object.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("KillZone"))
        {
            Level3RunnerGameManager.instance?.PlayerDied();
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            DamagePlayer(false);
            return;
        }

        if (collision.gameObject.CompareTag(obstacleTag))
        {
            DamagePlayer(true);
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone"))
        {
            Level3RunnerGameManager.instance?.PlayerDied();
            return;
        }

        if (other.CompareTag("Finish"))
        {
            Level3GameManager.instance?.LevelComplete();
            return;
        }

        if (other.CompareTag("Checkpoint"))
        {
            Level3RunnerGameManager.instance?.SetCheckpoint(transform.position);
            return;
        }
    }

    void DamagePlayer(bool applySlowdown)
    {
        if (isInvincible)
            return;

        if (playerController == null || playerController.isDead)
            return;

        playerController.TakeDamage();

        if (applySlowdown)
        {
            playerController.ApplyObstacleSlowdown(obstacleSlowMultiplier, obstacleSlowDuration);
        }

        StartCoroutine(InvincibilityFrames());
    }

    public void HitByObstacle(float slowMultiplier, float slowDuration)
    {
        if (isInvincible)
            return;

        if (playerController == null || playerController.isDead)
            return;

        playerController.TakeDamage();
        playerController.ApplyObstacleSlowdown(slowMultiplier, slowDuration);

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