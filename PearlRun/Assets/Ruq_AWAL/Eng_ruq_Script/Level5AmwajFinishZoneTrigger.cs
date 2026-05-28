using UnityEngine;

public class Level5AmwajFinishZoneTrigger : MonoBehaviour
{
    [Header("Victory Panel")]
    public GameObject victoryPanel;

    [Header("HUD to hide on win")]
    public GameObject hudObject;

    [Header("Victory Music")]
    public AudioClip victoryMusic;

    private bool finished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (finished)
            return;

        if (!other.CompareTag("Player"))
            return;

        finished = true;

        Level5AmwajPlayerController player = other.GetComponent<Level5AmwajPlayerController>();

        if (player == null)
        {
            player = other.GetComponentInParent<Level5AmwajPlayerController>();
        }

        if (player != null)
        {
            // Stop player movement
            player.enabled = false;

            // Stop Animator so the animation freezes
            Animator anim = player.GetComponentInChildren<Animator>();

            if (anim != null)
            {
                anim.enabled = false;
            }

            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            if (player.runSource != null)
            {
                player.runSource.Stop();
            }

            if (player.musicSource != null && victoryMusic != null)
            {
                player.musicSource.Stop();
                player.musicSource.loop = false;
                player.musicSource.clip = victoryMusic;
                player.musicSource.Play();
            }
        }

        // Hide HUD
        if (hudObject != null)
        {
            hudObject.SetActive(false);
        }

        // Tell Level 5 Amwaj manager the level is complete
        Level5AmwajRunnerGameManager.instance?.LevelComplete();

        int livesLeft = Level5AmwajRunnerGameManager.instance != null
            ? Level5AmwajRunnerGameManager.instance.currentLives
            : 1;

        // Optional: only works if ScoreManager exists in your project
        if (Level5AmwajScoreManager.Instance != null)
        {
            Level5AmwajScoreManager.Instance.CompleteLevel(livesLeft);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }
}