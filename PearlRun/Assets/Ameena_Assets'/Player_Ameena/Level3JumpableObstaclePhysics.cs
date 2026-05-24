using UnityEngine;

/// <summary>
/// Add this to any jumpable obstacle in Level 3.
/// It automatically builds two invisible colliders:
/// - A BoxCollider for the solid body
/// - A SphereCollider on the top so the player does not get stuck on the edge
///
/// Put the visible mesh as a child with no collider on it.
/// Tag the root GameObject as "JumpObstacle".
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Level3JumpableObstaclePhysics : MonoBehaviour
{
    [Header("Size")]
    [Tooltip("Width and depth of the obstacle.")]
    public Vector2 footprintSize = new Vector2(1.5f, 1.5f);

    [Tooltip("Height of the solid box body.")]
    public float bodyHeight = 1.2f;

    [Header("Top Cap")]
    [Tooltip("Radius of the sphere on the top. Bigger = smoother landing.")]
    public float capRadius = 0.65f;

    private bool collidersBuilt = false;

    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        BuildColliders();
    }

    void BuildColliders()
    {
        if (collidersBuilt)
            return;

        collidersBuilt = true;

        // Solid box body
        GameObject bodyGO = new GameObject("Level3PhysicsBody");
        bodyGO.transform.SetParent(transform, false);
        bodyGO.layer = gameObject.layer;
        bodyGO.tag = gameObject.tag;

        BoxCollider box = bodyGO.AddComponent<BoxCollider>();
        box.size = new Vector3(footprintSize.x, bodyHeight, footprintSize.y);
        box.center = new Vector3(0f, bodyHeight * 0.5f, 0f);

        // Smooth sphere top cap
        GameObject capGO = new GameObject("Level3PhysicsTopCap");
        capGO.transform.SetParent(transform, false);
        capGO.layer = gameObject.layer;
        capGO.tag = gameObject.tag;

        SphereCollider sphere = capGO.AddComponent<SphereCollider>();
        sphere.radius = capRadius;
        sphere.center = new Vector3(0f, bodyHeight - capRadius * 0.4f, 0f);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Green box = solid body
        Gizmos.color = new Color(0.2f, 0.8f, 0.5f, 0.35f);

        Vector3 boxCenter = transform.position + Vector3.up * (bodyHeight * 0.5f);
        Vector3 boxSize = new Vector3(footprintSize.x, bodyHeight, footprintSize.y);

        Gizmos.DrawCube(boxCenter, boxSize);

        Gizmos.color = new Color(0.2f, 0.8f, 0.5f, 1f);
        Gizmos.DrawWireCube(boxCenter, boxSize);

        // Yellow sphere = smooth top
        float capY = bodyHeight - capRadius * 0.4f;

        Gizmos.color = new Color(1f, 0.75f, 0.1f, 0.35f);
        Gizmos.DrawSphere(transform.position + Vector3.up * capY, capRadius);

        Gizmos.color = new Color(1f, 0.75f, 0.1f, 1f);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * capY, capRadius);
    }
#endif
}