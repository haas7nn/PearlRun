using UnityEngine;

public class RunnerCheckpoint : MonoBehaviour
{
    [Header("Visual")]
    public Renderer flagRenderer;
    public Color activatedColor = Color.yellow;
    public Transform visualRoot;
    public float activatedScale = 1.2f;

    [Header("Audio")]
    public AudioClip activationSound;
    [Range(0f, 1f)] public float soundVolume = 1f;
    public AudioSource audioSource;

    private bool activated = false;
    private Collider triggerCollider;

    // FIX 2: store the original scale so we multiply it, not replace it
    private Vector3 originalScale;

    void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
            triggerCollider.isTrigger = true;

        // Cache original scale before anything changes it
        if (visualRoot != null)
            originalScale = visualRoot.localScale;
        else
            originalScale = Vector3.one;
    }

    void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;

        Vector3 checkpointPos = new Vector3(
            transform.position.x,
            transform.position.y,
            other.transform.position.z
        );

        RunnerProgressSystem.SaveCheckpoint(checkpointPos);

        if (RunnerGameManager.instance != null)
            RunnerGameManager.instance.SetCheckpoint(checkpointPos);

        // Visual feedback
        if (flagRenderer != null)
            flagRenderer.material.color = activatedColor;

        // FIX 2: multiply the ORIGINAL scale instead of replacing with Vector3.one
        if (visualRoot != null)
            visualRoot.localScale = originalScale * activatedScale;

        PlayActivationSound();

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