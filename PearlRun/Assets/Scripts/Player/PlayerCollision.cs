using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    private Level3PlayerController playerController;
    private bool isInvincible = false;
    private float invincibilityTime = 1.5f;

    void Start()
    {
        playerController = GetComponent<Level3PlayerController>();

        if (playerController == null)
        {
            Debug.LogWarning("PlayerCollision needs Level3PlayerController on the same Player object.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isInvincible)
            return;

        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy"))
        {
            if (playerController != null)
            {
                playerController.TakeDamage();
                StartCoroutine(InvincibilityFrames());
            }
        }

        if (collision.gameObject.CompareTag("KillZone"))
        {
            if (Level3RunnerGameManager.instance != null)
            {
                Level3RunnerGameManager.instance.PlayerDied();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone"))
        {
            if (Level3RunnerGameManager.instance != null)
            {
                Level3RunnerGameManager.instance.PlayerDied();
            }
        }

        if (other.CompareTag("Finish"))
        {
            if (Level3RunnerGameManager.instance != null)
            {
                Level3RunnerGameManager.instance.LevelComplete();
            }
        }

        if (other.CompareTag("Checkpoint"))
        {
            if (Level3RunnerGameManager.instance != null)
            {
                Level3RunnerGameManager.instance.SetCheckpoint(transform.position);
            }
        }
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