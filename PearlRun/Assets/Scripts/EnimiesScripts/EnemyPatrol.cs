using UnityEngine;

public class EnemyPatrol : EnemyBase
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float speed = 2f;
    public float reachDistance = 0.1f;

    [Header("Animation")]
    public Animator animator;
    public string runningParameterName = "isRunning";

    private Transform target;

    private void Start()
    {
        target = pointB;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        SetRunning(true);
    }

    private void Update()
    {
        if (pointA == null || pointB == null)
        {
            SetRunning(false);
            return;
        }

        PatrolMove();
    }

    private void PatrolMove()
    {
        SetRunning(true);

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        FaceTarget();

        if (Vector3.Distance(transform.position, target.position) < reachDistance)
        {
            if (target == pointA)
                target = pointB;
            else
                target = pointA;

            FaceTarget();
        }
    }

    private void FaceTarget()
    {
        if (target == null)
            return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
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