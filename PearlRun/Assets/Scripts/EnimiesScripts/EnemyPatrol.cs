using UnityEngine;

public class EnemyPatrol : EnemyBase
{
    [Header("Patrol Waypoints")]
    public Transform pointA;
    public Transform pointB;

    [Header("Speed")]
    public float speed = 2f;

    private Transform target;

    protected override void Start()
    {
        base.Start();
        target = pointB;
    }

    private void Update()
    {
        // Don't move if dead
        if (isDead) return;

        // Safety checks
        if (pointA == null || pointB == null) return;

        // Move enemy toward target using Vector3
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            // Switch target
            if (target == pointA)
                target = pointB;
            else
                target = pointA;

            Flip();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ─────────────────────────────────────
    //  Gizmos – see patrol path in editor
    // ─────────────────────────────────────
    private void OnDrawGizmos()
    {
        if (pointA == null || pointB == null) return;

        // Line between waypoints
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pointA.position, pointB.position);

        // Point A marker
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pointA.position, 0.3f);

        // Point B marker
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pointB.position, 0.3f);
    }
}