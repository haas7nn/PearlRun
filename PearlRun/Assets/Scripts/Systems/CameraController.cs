using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(5f, 3f, -10f);
    public float smoothSpeed = 5f;
    public float lookAheadDistance = 3f;
    public float lookAheadSpeed = 2f;

    [Header("Zoom")]
    public float normalZoom = -10f;
    public float chaseZoom = -14f;
    public float zoomSpeed = 2f;
    private float currentZoom;

    [Header("Screen Shake")]
    private float shakeTimer = 0f;
    private float shakeIntensity = 0f;

    [Header("Boundaries")]
    public float minY = 2f;

    private float currentLookAhead = 0f;
    private bool isChaseMode = false;

    void Start()
    {
        currentZoom = normalZoom;

        if (target == null)
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate look ahead
        float targetLookAhead = lookAheadDistance;
        currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, lookAheadSpeed * Time.deltaTime);

        // Calculate zoom
        float targetZoom = isChaseMode ? chaseZoom : normalZoom;
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomSpeed * Time.deltaTime);

        // Calculate target position
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x + currentLookAhead,
            Mathf.Max(target.position.y + offset.y, minY),
            currentZoom
        );

        // Smooth follow
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Apply screen shake
        if (shakeTimer > 0)
        {
            smoothedPosition += Random.insideUnitSphere * shakeIntensity;
            shakeTimer -= Time.deltaTime;
        }

        transform.position = smoothedPosition;

        // Look at the player with slight offset ahead
        Vector3 lookTarget = new Vector3(target.position.x + currentLookAhead * 0.5f, target.position.y, target.position.z);
        transform.LookAt(lookTarget);

        // Override rotation to keep camera level for side-scroller
        transform.rotation = Quaternion.Euler(10f, 0f, 0f);
    }

    public void ShakeCamera(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeTimer = duration;
    }

    public void SetChaseMode(bool chase)
    {
        isChaseMode = chase;
    }
}