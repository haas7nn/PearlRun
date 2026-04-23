using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // ─────────────────────────────────────
    //  Stats
    // ─────────────────────────────────────
    [Header("Enemy Stats")]
    public int health = 3;
    public int damage = 1;

    // ─────────────────────────────────────
    //  State
    // ─────────────────────────────────────
    protected bool isDead = false;

    // ─────────────────────────────────────
    //  References
    // ─────────────────────────────────────
    protected Animator anim;

    // ─────────────────────────────────────
    //  Unity Lifecycle
    // ─────────────────────────────────────
    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }

    // ─────────────────────────────────────
    //  Damage
    // ─────────────────────────────────────
    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;

        // Play hurt animation if exists
        if (anim != null)
            anim.SetTrigger("Hurt");

        if (health <= 0)
            Die();
    }

    // ─────────────────────────────────────
    //  Death
    // ─────────────────────────────────────
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        // Disable collider so player cant 
        // keep hitting dead enemy
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Play death animation if exists
        if (anim != null)
        {
            anim.SetTrigger("Die");
            // Destroy after animation plays
            Destroy(gameObject, 1.5f);
        }
        else
        {
            // No animator - destroy immediately
            Destroy(gameObject, 0.2f);
        }
    }

    // ─────────────────────────────────────
    //  Collision With Player
    //  Only notify player - let PlayerCollision
    //  handle the actual damage
    // ─────────────────────────────────────
    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Direct GetComponent - NOT SendMessage
            PlayerCollision playerCollision =
                collision.gameObject.GetComponent<PlayerCollision>();

            if (playerCollision != null)
                playerCollision.HandleEnemyCollision();
        }
    }
}