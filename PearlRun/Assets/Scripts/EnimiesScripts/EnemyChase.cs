using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Stats")]
    public int health = 3;
    public int damage = 1;

    [Header("Chase Settings")]
    public Transform player;
    public float detectionRange = 15f;
    public float chaseSpeed = 4f;
    public float stopDistance = 1.5f;

    [Header("Fixed Movement")]
    public bool lockY = true;
    public bool lockZ = true;
    private float fixedY;
    private float fixedZ;

    [Header("Animation")]
    public Animator animator;
    public string runningParameterName = "isRunning";

    [Header("3D Facing")]
    public float rotationYWhenMovingRight = 90f;
    public float rotationYWhenMovingLeft = -90f;

    [Header("Behind Player Offset")]
    public float behindDistance = 3f;
    public bool playerRunsToRight = true;

    private bool isChasing = false;

    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("EnemyChase: No player found. Make sure the player Tag is Player.");
        }

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        SetRunning(false);
    }

    void Update()
    {
        if (player == null)
        {
            SetRunning(false);
            return;
        }

        float xDistance = Mathf.Abs(player.position.x - transform.position.x);

        isChasing = xDistance <= detectionRange;

        if (isChasing && xDistance > stopDistance)
        {
            ChasePlayer();
        }
        else
        {
            SetRunning(false);
        }
    }

    void ChasePlayer()
    {
        Vector3 targetPosition = player.position;

        // نخلي العدو يطارد نقطة ورا أوال، مو أوال نفسها
        if (playerRunsToRight)
            targetPosition.x = player.position.x - behindDistance;
        else
            targetPosition.x = player.position.x + behindDistance;

        if (lockY)
            targetPosition.y = fixedY;

        if (lockZ)
            targetPosition.z = player.position.z; // مهم: نفس مسار أوال، مو مكان العدو الأصلي

        float direction = Mathf.Sign(targetPosition.x - transform.position.x);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            chaseSpeed * Time.deltaTime
        );

        FaceDirection(direction);
        SetRunning(true);

        Debug.DrawLine(transform.position, targetPosition, Color.red, 0.1f);
    }

    void FaceDirection(float direction)
    {
        if (direction > 0)
        {
            transform.rotation = Quaternion.Euler(0f, rotationYWhenMovingRight, 0f);
        }
        else if (direction < 0)
        {
            transform.rotation = Quaternion.Euler(0f, rotationYWhenMovingLeft, 0f);
        }
    }

    void SetRunning(bool value)
    {
        if (animator != null)
        {
            animator.SetBool(runningParameterName, value);
        }
    }

    public void ForceChaseFromBehind()
    {
        if (player == null)
            return;

        Vector3 newPosition = player.position;

        if (playerRunsToRight)
            newPosition.x = player.position.x - behindDistance;
        else
            newPosition.x = player.position.x + behindDistance;

        if (lockY)
            newPosition.y = fixedY;

        if (lockZ)
            newPosition.z = player.position.z;

        transform.position = newPosition;

        isChasing = true;
        SetRunning(true);

        FaceDirection(playerRunsToRight ? 1f : -1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}