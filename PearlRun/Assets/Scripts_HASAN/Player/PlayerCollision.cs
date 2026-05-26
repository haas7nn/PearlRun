using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerController playerController;
    private bool isInvincible;
    [SerializeField] private float invincibilityTime = 1.5f;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
            Debug.LogError("PlayerCollision: PlayerController NOT FOUND on this GameObject!");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"COLLISION with: {collision.gameObject.name} | Tag: {collision.gameObject.tag}");

        if (isInvincible)
        {
            Debug.Log("Invincible - ignoring hit");
            return;
        }

        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit OBSTACLE/ENEMY!");

            if (playerController != null)
            {
                playerController.TakeDamage();
                StartCoroutine(InvincibilityFrames());
            }
            else
            {
                Debug.LogError("PlayerController is NULL! Cannot take damage.");
            }
        }

        if (collision.gameObject.CompareTag("KillZone"))
        {
            HandleKill();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"TRIGGER with: {other.name} | Tag: {other.tag}");

        if (other.CompareTag("KillZone"))
        {
            HandleKill();
        }
        else if (other.CompareTag("Finish"))
        {
            HandleFinish();
        }
        else if (other.CompareTag("Checkpoint"))
        {
            HandleCheckpoint();
        }
    }

    void HandleKill()
    {
        if (RunnerGameManager.instance != null)
            RunnerGameManager.instance.PlayerDied();
        else if (GameManager.instance != null)
            GameManager.instance.PlayerDied();
        else
            Debug.LogError("No GameManager found!");
    }

    void HandleFinish()
    {
        if (RunnerGameManager.instance != null)
            RunnerGameManager.instance.LevelComplete();
        else if (GameManager.instance != null)
            GameManager.instance.LevelComplete();
    }

    void HandleCheckpoint()
    {
        Vector3 pos = transform.position;

        if (RunnerGameManager.instance != null)
            RunnerGameManager.instance.SetCheckpoint(pos);
        else if (GameManager.instance != null)
            GameManager.instance.SetCheckpoint(pos);
    }

    System.Collections.IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        Debug.Log("Invincibility START");

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
        Debug.Log("Invincibility END");
    }

    public void SetInvincible(bool value) => isInvincible = value;
}