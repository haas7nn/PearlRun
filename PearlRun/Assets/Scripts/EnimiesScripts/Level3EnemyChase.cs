using UnityEngine;

public class Level3EnemyChase : MonoBehaviour
{
    [Header("Stats")]
    public int health = 3;
    public int damage = 1;

    [Header("Chase Settings")]
    public Transform player;
    public float detectionRange = 15f;
    public float chaseSpeed = 4f;
    public float stopDistance = 1.5f;

    [Header("Behind Player")]
    public float behindDistance = 4f;
    public bool playerRunsForwardZ = true;

    [Header("Fixed Movement")]
    public bool matchPlayerX = true;
    public bool lockY = true;

    private float fixedY;

    [Header("Animation")]
    public Animator animator;
    public string runningParameterName = "isRunning";

    [Header("Rotation")]
    public float rotationYWhenMovingForward = 0f;
    public float rotationYWhenMovingBackward = 180f;

    private bool isChasing = false;

    void Start()
    {
        fixedY = transform.position.y;

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

        float zDistance = Mathf.Abs(player.position.z - transform.position.z);

        isChasing = zDistance <= detectionRange;

        if (isChasing && zDistance > stopDistance)
        {
            ChasePlayerBehindOnZ();
        }
        else
        {
            SetRunning(false);
        }
    }

    void ChasePlayerBehindOnZ()
    {
        Vector3 targetPosition = player.position;

        // نخلي العدو ورا أوال على Z axis
        if (playerRunsForwardZ)
            targetPosition.z = player.position.z - behindDistance;
        else
            targetPosition.z = player.position.z + behindDistance;

        // نخليه على نفس خط أوال، مو جنبها
        if (matchPlayerX)
            targetPosition.x = player.position.x;

        if (lockY)
            targetPosition.y = fixedY;

        float directionZ = Mathf.Sign(targetPosition.z - transform.position.z);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            chaseSpeed * Time.deltaTime
        );

        FaceDirectionZ(directionZ);
        SetRunning(true);

        Debug.DrawLine(transform.position, targetPosition, Color.red, 0.1f);
    }

    void FaceDirectionZ(float directionZ)
    {
        if (directionZ > 0)
        {
            transform.rotation = Quaternion.Euler(0f, rotationYWhenMovingForward, 0f);
        }
        else if (directionZ < 0)
        {
            transform.rotation = Quaternion.Euler(0f, rotationYWhenMovingBackward, 0f);
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

        if (playerRunsForwardZ)
            newPosition.z = player.position.z - behindDistance;
        else
            newPosition.z = player.position.z + behindDistance;

        if (matchPlayerX)
            newPosition.x = player.position.x;

        if (lockY)
            newPosition.y = fixedY;

        transform.position = newPosition;

        isChasing = true;
        SetRunning(true);

        FaceDirectionZ(playerRunsForwardZ ? 1f : -1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}