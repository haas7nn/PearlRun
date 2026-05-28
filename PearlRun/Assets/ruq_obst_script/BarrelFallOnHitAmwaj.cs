using UnityEngine;

public class BarrelFallOnHitAmwaj : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Keep the barrel fixed at the start
        rb.isKinematic = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player hit the barrel
        if (collision.gameObject.CompareTag("Player"))
        {
            // Enable physics so the barrel can roll
            rb.isKinematic = false;
        }
    }
}