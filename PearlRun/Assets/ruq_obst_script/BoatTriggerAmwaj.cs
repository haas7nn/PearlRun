using UnityEngine;

public class BoatTriggerAmwaj : MonoBehaviour
{
    // The boat that will move
    public BoatMoveForwardAmwaj boatScript;

    void OnTriggerEnter(Collider other)
    {
        // Check if player entered trigger
        if (other.CompareTag("Player"))
        {
            // Start boat movement
            boatScript.StartMoving();
        }
    }
}