using UnityEngine;

public class ObstacleDebugTester : MonoBehaviour
{
    public string expectedTag = "Obstacle";
    public string expectedLayerName = "Obstacle";

    private void Start()
    {
        Debug.Log("========== OBSTACLE CHECK: " + gameObject.name + " ==========");
        Debug.Log("Tag: " + gameObject.tag);
        Debug.Log("Layer: " + LayerMask.LayerToName(gameObject.layer));

        Collider[] colliders = GetComponents<Collider>();

        foreach (Collider col in colliders)
        {
            Debug.Log(
                gameObject.name +
                " collider: " + col.GetType().Name +
                " | Is Trigger: " + col.isTrigger +
                " | Enabled: " + col.enabled
            );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(
            "SOLID COLLISION: " + gameObject.name +
            " hit by " + collision.gameObject.name +
            " | Other Tag: " + collision.gameObject.tag
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning(
            "TRIGGER EVENT on obstacle: " + gameObject.name +
            " | Other object: " + other.gameObject.name +
            " | Other Tag: " + other.tag +
            " | Other Is Trigger: " + other.isTrigger +
            " | This obstacle has solid collider: " + HasSolidCollider()
        );
    }

    private bool HasSolidCollider()
    {
        Collider[] colliders = GetComponents<Collider>();

        foreach (Collider col in colliders)
        {
            if (col.enabled && !col.isTrigger)
                return true;
        }

        return false;
    }
}