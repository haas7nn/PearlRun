using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    public int livesRemaining = 2;
    public GameObject summaryPanel;

    private bool finished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;

        if (!other.CompareTag("Player")) return;

        finished = true;

        var playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.enabled = false;

        var rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.CompleteLevel(livesRemaining);
        }

        if (summaryPanel != null)
        {
            summaryPanel.SetActive(true);
        }
    }
}