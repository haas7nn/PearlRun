using UnityEngine;

public class RunnerCheckpoint : MonoBehaviour
{
    [Header("Visual")]
    public Renderer flagRenderer;
    public Color activatedColor = Color.yellow;
    public Transform visualRoot;        // Optional: a child object to scale (so the collider doesn't grow)
    public float activatedScale = 1.2f;

    [Header("Audio")]
    public AudioClip activationSound;   // Assign your checkpoint sound here
    [Range(0f, 1f)] public float soundVolume = 1f;
    public AudioSource audioSource;     // Optional: drop an AudioSource here, otherwise plays at world point

    private bool activated = false;
    private Collider triggerCollider;

    void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
            triggerCollider.isTrigger = true;   // Force trigger so it cannot block the player
    }

    void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;
        Vector3 checkpointPos = transform.position;   // Use flag position, not the player's

        // Save to persistent storage
        RunnerProgressSystem.SaveCheckpoint(checkpointPos);

        // Tell the game manager so in-memory respawn works without a PlayerPrefs roundtrip
        if (RunnerGameManager.instance != null)
            RunnerGameManager.instance.SetCheckpoint(checkpointPos);

        // Visual feedback
        if (flagRenderer != null)
            flagRenderer.material.color = activatedColor;

        if (visualRoot != null)
            visualRoot.localScale = Vector3.one * activatedScale;

        // Sound feedback
        PlayActivationSound();

        // Disable the trigger so it cannot interfere with physics anymore
        if (triggerCollider != null)
            triggerCollider.enabled = false;

        Debug.Log("RunnerCheckpoint activated at " + checkpointPos);
    }

    void PlayActivationSound()
    {
        if (activationSound == null) return;

        if (audioSource != null)
            audioSource.PlayOneShot(activationSound, soundVolume);
        else
            AudioSource.PlayClipAtPoint(activationSound, transform.position, soundVolume);
    }
}