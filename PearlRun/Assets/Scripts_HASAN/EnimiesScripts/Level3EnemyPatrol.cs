using UnityEngine;

public class Level3EnemyPatrol : Level3EnemyBase
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float speed = 2f;
    public float reachDistance = 0.1f;

    [Header("Start Direction")]
    public bool startMovingToB = true;

    [Header("Fixed Position")]
    public bool lockY = true;
    public bool lockZ = true;
    public float fixedY;
    public float fixedZ;

    [Header("Enemy Rotation")]
    public float rotationYWhenMovingRight = 90f;
    public float rotationYWhenMovingLeft = -90f;

    [Header("Animation")]
    public Animator animator;
    public string runningParameterName = "isRunning";

    private Transform target;

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("EnemyPatrol: PointA or PointB is missing.");
            return;
        }

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        fixedY = transform.position.y;
        fixedZ = transform.position.z;

        target = startMovingToB ? pointB : pointA;

        SetRunning(true);
        FaceMoveDirection();
    }

    private void Update()
    {
        if (pointA == null || pointB == null)
        {
            SetRunning(false);
            return;
        }

        MovePatrol();
    }

    private void MovePatrol()
    {
        SetRunning(true);

        Vector3 targetPosition = target.position;

        if (lockY)
            targetPosition.y = fixedY;

        if (lockZ)
            targetPosition.z = fixedZ;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        FixPosition();
        FaceMoveDirection();

        if (Vector3.Distance(transform.position, targetPosition) <= reachDistance)
        {
            target = target == pointA ? pointB : pointA;
            FaceMoveDirection();
        }
    }

    private void FixPosition()
    {
        Vector3 pos = transform.position;

        if (lockY)
            pos.y = fixedY;

        if (lockZ)
            pos.z = fixedZ;

        transform.position = pos;
    }

    private void FaceMoveDirection()
    {
        if (target == null)
            return;

        float directionX = target.position.x - transform.position.x;

        if (directionX > 0)
        {
            transform.rotation = Quaternion.Euler(0f, rotationYWhenMovingRight, 0f);
        }
        else if (directionX < 0)
        {
            transform.rotation = Quaternion.Euler(0f, rotationYWhenMovingLeft, 0f);
        }
    }

    private void SetRunning(bool value)
    {
        if (animator != null)
        {
            animator.SetBool(runningParameterName, value);
        }
    }
}