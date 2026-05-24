// FinishZoneTrigger.cs
using UnityEngine;

public class FinishZoneTrigger : MonoBehaviour
{
    [Header("Victory Panel")]
    public GameObject victoryPanel;

    [Header("HUD to hide on win")]
    public GameObject hudObject;      // ← اسحب هنا الـ GameObject الي فيه RunnerHUD

    [Header("Victory Music")]
    public AudioClip victoryMusic;

    private bool finished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;
        if (!other.CompareTag("Player")) return;

        finished = true;

        RunnerController player = other.GetComponent<RunnerController>();
        if (player != null)
        {
            // Stop the script (movement)
            player.enabled = false;

            // Stop the Animator so the running animation freezes
            Animator anim = player.GetComponent<Animator>();
            if (anim != null)
                anim.enabled = false;

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            player.runSource?.Stop();

            if (player.musicSource != null && victoryMusic != null)
            {
                player.musicSource.Stop();
                player.musicSource.loop = false;
                player.musicSource.clip = victoryMusic;
                player.musicSource.Play();
            }
        }

        // Hide the HUD (Score/Lives display)
        if (hudObject != null)
            hudObject.SetActive(false);

        RunnerGameManager.instance?.LevelComplete();

        int livesLeft = RunnerGameManager.instance != null
                        ? RunnerGameManager.instance.currentLives
                        : 1;

        ScoreManager.Instance?.CompleteLevel(livesLeft);

        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }
}