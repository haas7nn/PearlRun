using UnityEngine;

public enum PearlType
{
    White,      // 1 point
    Blue,       // 5 points
    Golden,     // Restores 1 life
    Red,        // Extra life
    Qarqaoun    // 3 points (Level 3 only)
}

public class PearlCollectible : MonoBehaviour
{
    [Header("Pearl Type")]
    public PearlType pearlType = PearlType.White;

    [Header("Rotation & Bob")]
    public float rotateSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    [Header("Effects")]
    public GameObject collectEffect;

    private Vector3 startPosition;

    // ─────────────────────────────────────────
    //  SETUP
    // ─────────────────────────────────────────

    void Start()
    {
        startPosition = transform.position;
    }

    // ─────────────────────────────────────────
    //  ANIMATION
    // ─────────────────────────────────────────

    void Update()
    {
        // Rotate
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);

        // Bob up and down
        float newY = startPosition.y +
                     Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }

    // ─────────────────────────────────────────
    //  COLLECTION
    // ─────────────────────────────────────────

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (pearlType)
        {
            case PearlType.White:
                CollectPoints(1);
                break;

            case PearlType.Blue:
                CollectPoints(5);
                break;

            case PearlType.Qarqaoun:
                CollectPoints(3);
                break;

            case PearlType.Golden:
                CollectGoldenPearl();
                break;

            case PearlType.Red:
                CollectRedPearl();
                break;
        }

        // Play sound
        if (AudioManager.instance != null)
            AudioManager.instance.PlayPearlCollect();

        // Spawn effect
        if (collectEffect != null)
        {
            Instantiate(
                collectEffect,
                transform.position,
                Quaternion.identity
            );
        }

        // Destroy pearl
        Destroy(gameObject);
    }

    // ─────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────

    void CollectPoints(int points)
    {
        if (GameManager.instance == null) return;

        // Double points if power-up active
        if (PowerUpSystem.instance != null &&
            PowerUpSystem.instance.isDoublePointsActive)
        {
            points *= 2;
        }

        GameManager.instance.AddScore(points);
    }

    void CollectGoldenPearl()
    {
        if (GameManager.instance == null) return;

        // Restore 1 life
        GameManager.instance.AddLife();

        // Play special sound
        if (AudioManager.instance != null)
            AudioManager.instance.PlayLifePickup();
    }

    void CollectRedPearl()
    {
        if (GameManager.instance == null) return;

        // Extra life above max
        GameManager.instance.AddLife();

        // Play special sound
        if (AudioManager.instance != null)
            AudioManager.instance.PlayLifePickup();
    }
}