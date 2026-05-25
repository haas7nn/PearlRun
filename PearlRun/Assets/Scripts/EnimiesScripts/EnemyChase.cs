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
    private Animator animator;

    private void Start()
    {
        patrol = GetComponent<EnemyPatrol>();
        animator = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        if (patrol != null)
            patrol.speed = patrolSpeed;

        if (animator == null)
        {
            Debug.LogWarning("Enemy Animator not found!");
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        float xDistance = Mathf.Abs(player.position.x - transform.position.x);

        if (!isChasing && xDistance <= detectionRange)
            StartChasing();

        if (isChasing)
        {
            if (xDistance > stopChaseRange)
            {
                losePlayerTimer += Time.deltaTime;

                if (losePlayerTimer >= losePlayerDelay)
                    StopChasing();
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

        if (animator != null)
            animator.SetBool("isChasing", true);
        Debug.Log("Enemy animation: isChasing TRUE");

        if (patrol != null)
            patrol.enabled = false;
    }

    private void StopChasing()
    {
        isChasing = false;
        losePlayerTimer = 0f;

        if (animator != null)
            animator.SetBool("isChasing", false);
        Debug.Log("Enemy animation: isChasing FALSE");


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
                player = foundPlayer.transform;
        }

        if (player == null)
            return;

        Vector3 spawnPosition = player.position;
        spawnPosition.x = player.position.x - spawnBehindDistance;
        spawnPosition.y = player.position.y + spawnHeightOffset;
        spawnPosition.z = player.position.z;

        transform.position = spawnPosition;

        StartChasing();

        gameObject.SetActive(true);

        Debug.Log("Enemy appeared behind player and started chasing.");
    }

    private void ChasePlayer()
    {
        float xDifference = player.position.x - transform.position.x;

        if (Mathf.Abs(xDifference) <= stoppingDistance)
            return;

        float direction = Mathf.Sign(xDifference);

        transform.position += new Vector3(
            direction * chaseSpeed * Time.deltaTime,
            0f,
            0f
        );

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopChaseRange);
    }
}