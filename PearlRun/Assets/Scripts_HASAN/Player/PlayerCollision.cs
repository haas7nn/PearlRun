using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    // ─────────────────────────────────────
    //  References
    // ─────────────────────────────────────
    private PlayerController playerController;
    private PowerUpSystem powerUpSystem;

    // ─────────────────────────────────────
    //  Invincibility
    // ─────────────────────────────────────
    private bool isInvincible = false;
    private float invincibilityTime = 1.5f;

    // ─────────────────────────────────────
    //  Unity Lifecycle
    // ─────────────────────────────────────
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        powerUpSystem = GetComponent<PowerUpSystem>();
    }

    // ─────────────────────────────────────
    //  Called By EnemyBase
    //  This is the ONE place enemy damage
    //  enters the player
    // ─────────────────────────────────────
    public void HandleEnemyCollision()
    {
        if (isInvincible) return;
        if (playerController == null) return;

        // Check if shield power up is active
        if (powerUpSystem != null && powerUpSystem.IsShieldActive())
            return;

        playerController.TakeDamage();
        StartCoroutine(InvincibilityFrames());
    }

    // ─────────────────────────────────────
    //  Physics Collisions
    // ─────────────────────────────────────
    void OnCollisionEnter(Collision collision)
    {
        if (isInvincible) return;

        // Obstacle tag damage
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (powerUpSystem != null && powerUpSystem.IsShieldActive())
                return;

            playerController.TakeDamage();
            StartCoroutine(InvincibilityFrames());
        }

        // Kill zone (solid collider type)
        if (collision.gameObject.CompareTag("KillZone"))
        {
            HandleKillZone();
        }
    }

    // ─────────────────────────────────────
    //  Trigger Collisions
    // ─────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        // Kill zone (trigger type)
        if (other.CompareTag("KillZone"))
        {
            HandleKillZone();
            return;
        }

        // Finish line
        if (other.CompareTag("Finish"))
        {
            if (GameManager.instance != null)
                GameManager.instance.LevelComplete();
            return;
        }

        // Checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            if (GameManager.instance != null)
                GameManager.instance.SetCheckpoint(transform.position);

            // Play checkpoint sound
            if (AudioManager.instance != null)
                AudioManager.instance.PlayCheckpoint();
            return;
        }
    }

    // ─────────────────────────────────────
    //  Kill Zone Handler
    //  One method handles both collision
    //  and trigger kill zones
    // ─────────────────────────────────────
    private void HandleKillZone()
    {
        // Shield does NOT protect against kill zones
        // Falling off the map = instant death
        if (GameManager.instance != null)
            GameManager.instance.PlayerDied();
    }

    // ─────────────────────────────────────
    //  Invincibility Frames (flashing)
    // ─────────────────────────────────────
    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        // Play hurt sound
        if (AudioManager.instance != null)
            AudioManager.instance.PlayHurt();

        // Flash player renderer
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
            // Make sure renderer is ON when done
            playerRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invincibilityTime);
        }

        isInvincible = false;
    }

    // ─────────────────────────────────────
    //  Public Getters
    // ─────────────────────────────────────
    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}