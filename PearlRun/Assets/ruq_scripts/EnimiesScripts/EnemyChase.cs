using UnityEngine;

public class EnemyChase : EnemyBase
{
    public Transform player;
    public float chaseSpeed = 4f;
    public float patrolSpeed = 2f;

    private bool isChasing;
    private EnemyPatrol patrol;

    private void Start()
    {
        patrol = GetComponent<EnemyPatrol>();
    }

    private void Update()
    {
        if (isChasing && player != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                chaseSpeed * Time.deltaTime
            );
        }
        else
        {
            if (patrol != null)
            {
                patrol.speed = patrolSpeed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            isChasing = true;

            if (patrol != null)
                patrol.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = false;

            if (patrol != null)
                patrol.enabled = true;
        }
    }
}