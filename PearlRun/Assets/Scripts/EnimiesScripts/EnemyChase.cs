using UnityEngine;

public class EnemyChase : EnemyBase
{

    [Header("Player")]
    public Transform player;

    [Header("Chase Settings")]
    public float detectionRange = 4f;
    public float stopChaseRange = 7f;
    public float chaseSpeed = 4f;
    public float patrolSpeed = 2f;
    public float stoppingDistance = 1.2f;

    [Header("Forced Chase")]
    public float spawnBehindDistance = 4f;
    public float spawnHeightOffset = 0f;

    [Header("Lose Player")]
    public float losePlayerDelay = 2f;

    private bool isChasing;
    private float losePlayerTimer;
    private EnemyPatrol patrol;

    private void Start()
    {
        patrol = GetComponent<EnemyPatrol>();

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }

        if (patrol != null)
        {
            patrol.speed = patrolSpeed;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Start chasing when player enters detection range
        if (!isChasing && distanceToPlayer <= detectionRange)
        {
            StartChasing();
        }

        // While chasing
        if (isChasing)
        {
            if (distanceToPlayer > stopChaseRange)
            {
                losePlayerTimer += Time.deltaTime;

                if (losePlayerTimer >= losePlayerDelay)
                {
                    StopChasing();
                }
            }
            else
            {
                losePlayerTimer = 0f;
                ChasePlayer();
            }
        }
    }

    private void StartChasing()
    {
        isChasing = true;
        losePlayerTimer = 0f;

        if (patrol != null)
        {
            patrol.enabled = false;
        }
    }

    private void StopChasing()
    {
        isChasing = false;
        losePlayerTimer = 0f;

        if (patrol != null)
        {
            patrol.enabled = true;
            patrol.speed = patrolSpeed;
        }
    }

    public void ForceChaseFromBehind()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }

        if (player == null) return;

        // Put enemy directly behind the player
        Vector3 spawnPosition = player.position - player.forward * spawnBehindDistance;
        spawnPosition.y = player.position.y + spawnHeightOffset;

        transform.position = spawnPosition;

        // Start chasing immediately
        isChasing = true;
        losePlayerTimer = 0f;

        if (patrol != null)
        {
            patrol.enabled = false;
        }

        gameObject.SetActive(true);

        Debug.Log("Enemy appeared behind player and started chasing.");
    }

    private void ChasePlayer()
    {
        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y;

        float distance = Vector3.Distance(transform.position, targetPosition);

        // Prevent enemy from entering inside the player
        if (distance <= stoppingDistance)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            chaseSpeed * Time.deltaTime
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Enemy damaged the player!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopChaseRange);
    }
}