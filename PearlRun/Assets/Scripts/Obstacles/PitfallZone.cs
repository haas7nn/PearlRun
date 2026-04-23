using UnityEngine;

public class PitfallZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Die() is in PlayerController not PlayerCollision
        PlayerController playerController =
            other.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.TakeDamage();
            Debug.Log("PitfallZone: Player fell into pit!");
        }
    }
}