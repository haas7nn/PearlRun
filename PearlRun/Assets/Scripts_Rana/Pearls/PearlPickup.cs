using UnityEngine;

public class PearlPickup : MonoBehaviour
{
    public enum PearlType
    {
        White,
        Blue,
        Golden,
        Red,
        Qarqaoun
    }

    [SerializeField] private PearlType pearlType;

    [Header("Effects")]
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;

    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        // VFX
        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        // SFX
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // Score (only White / Blue / Qarqaoun give points)
        int pearlScore = GetPearlScore();
        if (pearlScore > 0 && ScoreManager.Instance != null)
            ScoreManager.Instance.AddPearls(pearlScore);

        // Special effects (Golden / Red)
        ApplyEffect();

        Destroy(gameObject);
    }

    private int GetPearlScore()
    {
        switch (pearlType)
        {
            case PearlType.White: return 1;
            case PearlType.Blue: return 5;
            case PearlType.Qarqaoun: return 3;
            default: return 0;
        }
    }

    private void ApplyEffect()
    {
        if (RunnerGameManager.instance == null) return;

        switch (pearlType)
        {
            case PearlType.Red:
                // +1 life (one heart back on the HUD)
                RunnerGameManager.instance.AddLife();
                Debug.Log("Red pearl collected → +1 life.");
                break;

            case PearlType.Golden:
                // Full refill — all hearts back
                RunnerGameManager.instance.RestoreFullLives();
                Debug.Log("Golden pearl collected → full lives restored.");
                break;
        }
    }
}