using UnityEngine;

public class Level5AmwajCheckpoint : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint;

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
    private Vector3 originalScale;

    void Awake()
    {
        triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
            triggerCollider.isTrigger = true;

        originalScale = visualRoot != null ? visualRoot.localScale : Vector3.one;
    }

    void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (!other.CompareTag("Player") && !other.transform.root.CompareTag("Player"))
            return;

        if (respawnPoint == null)
        {
            Debug.LogWarning("Level5AmwajCheckpoint: RespawnPoint is not assigned.");
            return;
        }

        activated = true;

        if (Level5AmwajRunnerGameManager.instance != null)
        {
            Level5AmwajRunnerGameManager.instance.SetCheckpoint(respawnPoint.position);
            Debug.Log("Level5AmwajCheckpoint saved RespawnPoint at " + respawnPoint.position);
        }
        else
        {
            Debug.LogWarning("Level5AmwajRunnerGameManager instance was not found.");
        }

        if (flagRenderer != null)
            flagRenderer.material.color = activatedColor;

        if (visualRoot != null)
            visualRoot.localScale = originalScale * activatedScale;

        PlayActivationSound();
    }

    void PlayActivationSound()
    {
        if (activationSound == null)
            return;

        if (audioSource != null)
            audioSource.PlayOneShot(activationSound, soundVolume);
        else
            AudioSource.PlayClipAtPoint(activationSound, transform.position, soundVolume);
    }
}