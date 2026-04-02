using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 2f;
    public int damage = 1;
    public float lifeTime = 5f;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit by projectile");

            // This will work later when Health script exists
            var playerHealth = collision.GetComponent("Health");
            if (playerHealth != null)
            {
                collision.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }

            Destroy(gameObject);
        }
    }
}