using UnityEngine;

public class Level3EnemyBase : MonoBehaviour
{
    public int health = 3;

    public virtual void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Animator anim = GetComponentInChildren<Animator>();

        if (anim != null)
        {
            anim.SetTrigger("Die");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}