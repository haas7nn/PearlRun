using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarMover : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.left;
    public float speed = 25f;
    public float lifeTime = 8f;
    public bool active = true;

    [Header("Ground Lock")]
    public float groundY = 0.08716574f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;

        Destroy(gameObject, lifeTime);
    }
    void Update()
    {
        if (!active) return;
        transform.position += moveDirection.normalized * speed * Time.deltaTime;
    }

    void FixedUpdate()
    {
        Vector3 delta = moveDirection.normalized * speed * Time.fixedDeltaTime;

        // Calculate next position
        Vector3 nextPos = rb.position + delta;

        // Force Y to stay on road
        nextPos.y = groundY;

        rb.MovePosition(nextPos);
    }
}