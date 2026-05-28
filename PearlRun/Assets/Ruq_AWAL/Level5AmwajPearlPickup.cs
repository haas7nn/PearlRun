using UnityEngine;

public class Level5AmwajPearlPickup : MonoBehaviour
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
            if (Level5AmwajScoreManager.Instance != null)
            {
                Level5AmwajScoreManager.Instance.AddPearls(pearlScore);

                Level5AmwajPlayerController player = other.GetComponent<Level5AmwajPlayerController>();
                if (player != null && player.hud != null)
                {
                    player.hud.UpdateScoreText(Level5AmwajScoreManager.Instance.currentPearls);
                }
            }
            else
            {
                Debug.LogError("Level5AmwajScoreManager is missing in scene.");
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
            case PearlType.White: return 1;
            case PearlType.Blue: return 5;
            case PearlType.Golden:
                Level5AmwajRunnerGameManager.instance.AddLife();
                Debug.Log("Level 5 Amwaj Golden pearl collected: +1 life.");
                return 0;
            case PearlType.Qarqaoun: return 3;
            default: return 0;
        }
    }
}
