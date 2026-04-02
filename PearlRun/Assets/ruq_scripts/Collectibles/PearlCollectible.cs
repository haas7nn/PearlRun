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

    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

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
            case PearlType.White:
                return 1;

            case PearlType.Blue:
                return 5;

            case PearlType.Golden:
                return 0;

            case PearlType.Red:
                return 0;

            case PearlType.Qarqaoun:
                return 3;

            default:
                return 0;
        }
    }

    private void ApplyEffect()
    {
        switch (pearlType)
        {
            case PearlType.White:
                Debug.Log("White pearl collected: +1 point");
                break;

            case PearlType.Blue:
                Debug.Log("Blue pearl collected: +5 points");
                break;

            case PearlType.Golden:
                Debug.Log("Golden pearl collected: restore life");
                break;

            case PearlType.Red:
                Debug.Log("Red pearl collected: extra life");
                break;

            case PearlType.Qarqaoun:
                Debug.Log("Qarqaoun sweet collected: +3 points");
                break;
        }
    }
}