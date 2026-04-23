using UnityEngine;

public class FishingUncle : MonoBehaviour
{
    // ─────────────────────────────────────
    //  Settings
    // ─────────────────────────────────────
    [Header("Fishing Rod Settings")]
    [SerializeField] private Transform rodTipPoint;
    [SerializeField] private int damageAmount = 1;

    [Header("Audio")]
    [SerializeField] private AudioClip hitRodSound;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    // ─────────────────────────────────────
    //  Trigger
    // ─────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        // Only care about the player
        if (!other.CompareTag("Player"))
            return;

        // Get PlayerController from the player
        PlayerController playerController = other.GetComponent<PlayerController>();

        // Safety check
        if (playerController == null)
        {
            Debug.LogWarning("FishingUncle: Player tag found but no PlayerController!");
            return;
        }

        // IsSliding is a PROPERTY not a method - no brackets
        if (!playerController.IsSliding)
        {
            // Player did NOT slide - they hit the rod
            HitPlayer(playerController);
        }
        else
        {
            // Player slid under successfully
            if (showDebugLogs)
                Debug.Log("FishingUncle: Nice slide! Player avoided the rod!");
        }
    }

    // ─────────────────────────────────────
    //  Hit Player
    // ─────────────────────────────────────
    private void HitPlayer(PlayerController playerController)
    {
        // TakeDamage lives in PlayerController not PlayerCollision
        playerController.TakeDamage();

        // Play bonk sound using correct instance name and method
        if (AudioManager.instance != null && hitRodSound != null)
        {
            AudioManager.instance.PlaySFX(hitRodSound);
        }

        if (showDebugLogs)
            Debug.Log("FishingUncle: Player hit the rod! Should have slid under!");
    }

    // ─────────────────────────────────────
    //  Gizmo (see rod tip in editor)
    // ─────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        if (rodTipPoint != null)
        {
            // Draw a sphere at rod tip position
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(rodTipPoint.position, 0.2f);

            // Draw line from uncle to rod tip
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, rodTipPoint.position);
        }

        // Draw the trigger area
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}