using UnityEngine;

/// <summary>
/// Add this to any jumpable obstacle (rock, tent, crate, etc.).
/// It automatically builds two invisible colliders:
///   - A BoxCollider for the solid body
///   - A SphereCollider on the crown so the player never gets stuck on the top edge
///
/// Your mesh sits as a child with NO collider on it.
/// Tag this root GameObject "JumpObstacle".
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class JumpableObstaclePhysics : MonoBehaviour
{
    [Header("Size")]
    [Tooltip("Width and depth of the obstacle.")]
    public Vector2 footprintSize = new Vector2(1.5f, 1.5f);

    [Tooltip("Height of the solid box body.")]
    public float bodyHeight = 1.2f;

    [Header("Top Cap")]
    [Tooltip("Radius of the sphere on the crown. Bigger = smoother landing. "
           + "Should be at least half your smallest footprint dimension.")]
    public float capRadius = 0.65f;

    void Awake()
    {
        // Lock the Rigidbody — obstacle never moves
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        BuildColliders();
    }

    void BuildColliders()
    {
        // --- Solid box (the body) ---
        GameObject bodyGO = new GameObject("PhysicsBody");
        bodyGO.transform.SetParent(transform, false);
        bodyGO.layer = gameObject.layer;

        BoxCollider box = bodyGO.AddComponent<BoxCollider>();
        box.size = new Vector3(footprintSize.x, bodyHeight, footprintSize.y);
        box.center = new Vector3(0f, bodyHeight * 0.5f, 0f);

        // --- Sphere cap (the crown) ---
        GameObject capGO = new GameObject("PhysicsTopCap");
        capGO.transform.SetParent(transform, false);
        capGO.layer = gameObject.layer;

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
        Gizmos.DrawCube(boxCenter, new Vector3(footprintSize.x, bodyHeight, footprintSize.y));
        Gizmos.color = new Color(0.2f, 0.8f, 0.5f, 1f);
        Gizmos.DrawWireCube(boxCenter, new Vector3(footprintSize.x, bodyHeight, footprintSize.y));

        // Yellow sphere = smooth crown
        float capY = bodyHeight - capRadius * 0.4f;
        Gizmos.color = new Color(1f, 0.75f, 0.1f, 0.35f);
        Gizmos.DrawSphere(transform.position + Vector3.up * capY, capRadius);
        Gizmos.color = new Color(1f, 0.75f, 0.1f, 1f);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * capY, capRadius);
    }
#endif
}