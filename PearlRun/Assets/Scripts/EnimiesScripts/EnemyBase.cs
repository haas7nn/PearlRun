using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float health = 3f;
    public float damage = 1f;
    protected bool isDead;
    protected Animator anim;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        if (anim != null)
            anim.SetTrigger("die");

        Destroy(gameObject, 1f);
    }

    protected virtual void OnPlayerContact(GameObject player)
    {
        Health playerHealth = player.GetComponent<Health>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health player = collision.gameObject.GetComponent<Health>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
