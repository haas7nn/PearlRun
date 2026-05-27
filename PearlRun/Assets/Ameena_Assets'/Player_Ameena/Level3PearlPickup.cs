using UnityEngine;

public class Level3PearlPickup : MonoBehaviour
{
    public enum PearlType { White, Blue, Golden, Red, Qarqaoun }

    [SerializeField] private PearlType pearlType;
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;

    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pearl touched by: " + other.name + " | Tag: " + other.tag);

        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        int pearlScore = GetPearlScore();

        if (pearlScore > 0)
        {
            if (Level3ScoreManager.Instance != null)
            {
                Level3ScoreManager.Instance.AddPearls(pearlScore);

                Level3PlayerController player = other.GetComponent<Level3PlayerController>();
                if (player != null && player.hud != null)
                {
                    player.hud.UpdateScoreText(Level3ScoreManager.Instance.currentPearls);
                }
            }
            else
            {
                Debug.LogError("Level3ScoreManager is missing in scene.");
            }
        }

        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        Destroy(gameObject);
    }

    private int GetPearlScore()
    {
        switch (pearlType)
        {
            case PearlType.White:
                return 1;

            case PearlType.Blue:
                return 5;

            case PearlType.Golden:
                // Full refill — all hearts back
                if (Level3RunnerGameManager.instance != null)
                {
                    Level3RunnerGameManager.instance.RestoreFullLives();
                    Debug.Log("Golden pearl collected → full lives restored.");
                }
                return 0;

            case PearlType.Red:
                // +1 life (one heart back on the HUD)
                if (Level3RunnerGameManager.instance != null)
                {
                    Level3RunnerGameManager.instance.AddLife();
                    Debug.Log("Red pearl collected → +1 life.");
                }
                return 0;

            case PearlType.Qarqaoun:
                return 3;

            default:
                return 0;
        }
    }
}