using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int health = 3;
    public int damage = 1;

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
        Animator anim = GetComponent<Animator>();

        if (anim != null)
        {
            anim.SetTrigger("Die");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by enemy");
            collision.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }
}