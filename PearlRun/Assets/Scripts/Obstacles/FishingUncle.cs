using UnityEngine;

public class FishingUncle : MonoBehaviour
{
    [Header("Fishing Rod Settings")]
    [SerializeField] private Transform rodTipPoint;
    [SerializeField] private int damageAmount = 1;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        RunnerController runnerController = other.GetComponent<RunnerController>();

        if (runnerController == null)
        {
            Debug.LogWarning("FishingUncle: Player tag found but no RunnerController!");
            return;
        }

        if (!runnerController.IsSliding)
        {
            HitPlayer(runnerController);
        }
        else
        {
            if (showDebugLogs)
                Debug.Log("FishingUncle: Nice slide! Player avoided the rod!");
        }
    }

    private void HitPlayer(RunnerController runnerController)
    {
        if (PowerUpSystem.instance != null && PowerUpSystem.instance.IsShieldActive())
            return;

        runnerController.TakeDamage();

        if (showDebugLogs)
            Debug.Log("FishingUncle: Player hit the rod! Should have slid under!");
    }

    private void OnDrawGizmosSelected()
    {
        if (rodTipPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(rodTipPoint.position, 0.2f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, rodTipPoint.position);
        }

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        BoxCollider box = GetComponent<BoxCollider>();

        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}