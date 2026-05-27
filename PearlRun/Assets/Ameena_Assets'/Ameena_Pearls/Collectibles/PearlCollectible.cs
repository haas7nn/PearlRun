using UnityEngine;

public class PearlCollectible : MonoBehaviour
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

        //Particle
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        //Sound
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        ApplyEffect();

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPearls(GetPearlScore());
        }

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
        switch (pearlType)
        {
            case PearlType.Golden:
                Debug.Log("Restore health");
                // TODO:Health system
                break;

            case PearlType.Red:
                Debug.Log("Extra life");
                // TODO:Life system
                break;
        }
    }
}