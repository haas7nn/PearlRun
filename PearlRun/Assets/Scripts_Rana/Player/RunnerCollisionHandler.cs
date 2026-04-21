using UnityEngine;

public class RunnerCollisionHandler : MonoBehaviour
{
    private RunnerController runnerController;
    private bool isInvincible = false;
    private float invincibilityTime = 1.5f;

    void Start()
    {
        runnerController = GetComponent<RunnerController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isInvincible)
            return;

        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy"))
        {
            if (runnerController != null)
            {
                runnerController.TakeDamage();
                StartCoroutine(InvincibilityFrames());
            }
        }

        if (collision.gameObject.CompareTag("KillZone"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.PlayerDied();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.PlayerDied();
            }
        }

        if (other.CompareTag("Finish"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.LevelComplete();
            }
        }

        if (other.CompareTag("Checkpoint"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.SetCheckpoint(transform.position);
            }
        }
    }

    System.Collections.IEnumerator InvincibilityFrames()
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