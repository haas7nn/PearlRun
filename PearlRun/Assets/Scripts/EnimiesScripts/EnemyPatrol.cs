using UnityEngine;

public class EnemyPatrol : EnemyBase
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Transform target;

    private void Start()
    {
        target = pointB;
    }

    private void Update()
    {
        if (pointA == null || pointB == null) return;

        // Move enemy in 3D space
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
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
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}