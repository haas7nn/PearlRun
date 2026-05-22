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
        if (isDead)
            return;

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
        if (isDead)
            return;

        isDead = true;

        // Disable collider
        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = false;

        // Play death animation if exists
        if (anim != null)
        {
            anim.SetTrigger("Die");

            // Destroy after animation
            Destroy(gameObject, 1.5f);
        }
        else
        {
            Destroy(gameObject, 0.2f);
        }
    }

    // ─────────────────────────────────────
    //  Collision With Player
    // ─────────────────────────────────────
    private void OnCollisionEnter(Collision collision)
    {
        if (isDead)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            RunnerCollisionHandler runnerCollisionHandler =
                collision.gameObject.GetComponent<RunnerCollisionHandler>();

            if (runnerCollisionHandler != null)
            {
                runnerCollisionHandler.SendMessage("DamagePlayer", false,
                    SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}