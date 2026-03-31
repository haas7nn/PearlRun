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

        Destroy(gameObject);
    }

    private void ApplyEffect()
    {
        switch (pearlType)
        {
            case PearlType.White:
                GameManager.instance.AddScore(1);
                break;

            case PearlType.Blue:
                GameManager.instance.AddScore(5);
                break;

            case PearlType.Golden:
                GameManager.instance.RestoreLife(1);
                break;

            case PearlType.Red:
                GameManager.instance.AddLife(1);
                break;

            case PearlType.Qarqaoun:
                GameManager.instance.AddScore(3);
                break;
        }
    }
}