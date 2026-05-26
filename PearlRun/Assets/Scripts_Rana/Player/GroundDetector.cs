using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public bool isGrounded { get; private set; }
    private int count = 0;

    void Start()
    {
        // Start on ground
        isGrounded = true;
        count = 1;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            count++;
            isGrounded = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            count--;
            if (count <= 0)
            {
                count = 0;
                isGrounded = false;
            }
        }
    }
}