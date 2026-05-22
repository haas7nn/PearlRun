using UnityEngine;

public class PitfallZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Get RunnerController instead of PlayerController
        RunnerController runnerController =
            other.GetComponent<RunnerController>();

        if (runnerController != null)
        {
            runnerController.TakeDamage();

            Debug.Log("PitfallZone: Player fell into pit!");
        }
    }
}