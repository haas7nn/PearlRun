using UnityEngine;

public enum PowerUpType
{
    Shield,
    Magnet,
    SlowMotion,
    DoublePoints
}

public class PowerUpPickup : MonoBehaviour
{
    [Header("Power-Up Settings")]
    public PowerUpType powerUpType;

    [Header("Effects")]
    public GameObject collectEffect;
    public AudioClip collectSound;

    [Header("Rotation")]
    public float rotateSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.3f;

    private Vector3 startPosition;
    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Rotate and bob up and down to look attractive
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);

        float newY = startPosition.y +
                     Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Get PowerUpSystem from player
        PowerUpSystem powerUpSystem =
            other.GetComponent<PowerUpSystem>();

        if (powerUpSystem == null) return;

        // Activate correct power-up
        switch (powerUpType)
        {
            case PowerUpType.Shield:
                powerUpSystem.ActivateShield();
                break;

            case PowerUpType.Magnet:
                powerUpSystem.ActivateMagnet();
                break;

            case PowerUpType.SlowMotion:
                powerUpSystem.ActivateSlowMotion();
                break;

            case PowerUpType.DoublePoints:
                powerUpSystem.ActivateDoublePoints();
                break;
        }

        // Play sound
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(
                collectSound,
                transform.position
            );
        }

        // Spawn effect
        if (collectEffect != null)
        {
            Instantiate(
                collectEffect,
                transform.position,
                Quaternion.identity
            );
        }

        // Destroy power-up
        Destroy(gameObject);
    }
}
