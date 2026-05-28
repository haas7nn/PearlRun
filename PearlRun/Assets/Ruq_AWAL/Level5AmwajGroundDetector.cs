using UnityEngine;

public class Level5AmwajGroundDetector : MonoBehaviour
{
    public bool isGrounded { get; private set; }

    private int groundTouchCount = 0;

    void Start()
    {
        // Start as grounded because the player usually begins on the floor
        isGrounded = true;
        groundTouchCount = 1;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            groundTouchCount++;
            isGrounded = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            groundTouchCount--;

            if (groundTouchCount <= 0)
            {
                groundTouchCount = 0;
                isGrounded = false;
            }
        }
    }

    public void ForceGrounded(bool value)
    {
        isGrounded = value;
        groundTouchCount = value ? 1 : 0;
    }
}
