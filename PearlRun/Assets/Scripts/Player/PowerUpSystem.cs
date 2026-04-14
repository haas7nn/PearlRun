using UnityEngine;
using System.Collections;

public class PowerUpSystem : MonoBehaviour
{
    public static PowerUpSystem instance;

    [Header("Power-Up Durations")]
    public float shieldDuration = 8f;
    public float magnetDuration = 6f;
    public float slowMotionDuration = 5f;
    public float doublePointsDuration = 10f;

    [Header("Magnet Settings")]
    public float magnetRadius = 5f;
    public float magnetPullSpeed = 10f;
    public LayerMask pearlLayer;

    [Header("Slow Motion Settings")]
    public float slowMotionScale = 0.5f;

    [Header("Active States - Read Only")]
    public bool isShieldActive = false;
    public bool isMagnetActive = false;
    public bool isSlowMotionActive = false;
    public bool isDoublePointsActive = false;

    private PlayerCollision playerCollision;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerCollision = GetComponent<PlayerCollision>();
    }

    void Update()
    {
        HandleMagnet();
    }

    // ─────────────────────────────────────────
    //  ACTIVATE METHODS
    // ─────────────────────────────────────────

    public void ActivateShield()
    {
        StopCoroutine(nameof(ShieldRoutine));
        StartCoroutine(nameof(ShieldRoutine));
    }

    public void ActivateMagnet()
    {
        StopCoroutine(nameof(MagnetRoutine));
        StartCoroutine(nameof(MagnetRoutine));
    }

    public void ActivateSlowMotion()
    {
        StopCoroutine(nameof(SlowMotionRoutine));
        StartCoroutine(nameof(SlowMotionRoutine));
    }

    public void ActivateDoublePoints()
    {
        StopCoroutine(nameof(DoublePointsRoutine));
        StartCoroutine(nameof(DoublePointsRoutine));
    }

    // ─────────────────────────────────────────
    //  COROUTINES
    // ─────────────────────────────────────────

    IEnumerator ShieldRoutine()
    {
        isShieldActive = true;

        // Tell PlayerCollision to ignore damage
        if (playerCollision != null)
            playerCollision.SetInvincible(true);

        yield return new WaitForSeconds(shieldDuration);

        isShieldActive = false;

        if (playerCollision != null)
            playerCollision.SetInvincible(false);
    }

    IEnumerator MagnetRoutine()
    {
        isMagnetActive = true;
        yield return new WaitForSeconds(magnetDuration);
        isMagnetActive = false;
    }

    IEnumerator SlowMotionRoutine()
    {
        isSlowMotionActive = true;
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(slowMotionDuration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        isSlowMotionActive = false;
    }

    IEnumerator DoublePointsRoutine()
    {
        isDoublePointsActive = true;
        yield return new WaitForSeconds(doublePointsDuration);
        isDoublePointsActive = false;
    }

    // ─────────────────────────────────────────
    //  MAGNET LOGIC
    // ─────────────────────────────────────────

    void HandleMagnet()
    {
        if (!isMagnetActive) return;

        // Find all pearls in radius and pull them toward player
        Collider[] pearls = Physics.OverlapSphere(
            transform.position,
            magnetRadius,
            pearlLayer
        );

        foreach (Collider pearl in pearls)
        {
            pearl.transform.position = Vector3.MoveTowards(
                pearl.transform.position,
                transform.position,
                magnetPullSpeed * Time.deltaTime
            );
        }
    }

    // ─────────────────────────────────────────
    //  PUBLIC GETTERS FOR HUD
    // ─────────────────────────────────────────

    public bool IsAnyPowerUpActive()
    {
        return isShieldActive || isMagnetActive ||
               isSlowMotionActive || isDoublePointsActive;
    }

    public string GetActivePowerUpName()
    {
        if (isShieldActive) return "Shield";
        if (isMagnetActive) return "Magnet";
        if (isSlowMotionActive) return "Slow Motion";
        if (isDoublePointsActive) return "Double Points";
        return "";
    }

    void OnDrawGizmosSelected()
    {
        // Show magnet radius in editor
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
