using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    public int livesRemaining = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.CompleteLevel(livesRemaining);
        }
    }
}