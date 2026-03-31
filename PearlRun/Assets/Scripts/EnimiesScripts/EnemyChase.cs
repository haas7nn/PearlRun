using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 4f;
    private bool isChasing;

    private void Update()
    {
        if(isChasing && player != null)
        {
            transform.position = Vector2.MoveTowards(
               transform.position, player.position, chaseSpeed * Time.deltaTime
                );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            isChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = false;
        }
    }
}
