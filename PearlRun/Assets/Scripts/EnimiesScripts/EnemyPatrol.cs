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
        if (isDead) return;
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        
        if(Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            if (target == pointA)
                target = pointB;
            else
                target = pointA;

            Flip();
        }
}
        private void Flip()
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    
}
