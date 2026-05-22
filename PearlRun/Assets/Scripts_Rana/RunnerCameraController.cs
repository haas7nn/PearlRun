using UnityEngine;

public class RunnerCameraController : MonoBehaviour
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

    [Header("Screen Shake")]
    private float shakeTimer = 0f;
    private float shakeIntensity = 0f;

    [Header("Boundaries")]
    public float minY = 2f;

    private float currentZoom;
    private float currentLookAhead = 0f;
    private bool isChaseMode = false;

    private void Start()
    {
        currentZoom = normalZoom;

        if (target == null)
        {
            RunnerController runner = FindAnyObjectByType<RunnerController>();

            if (runner != null)
            {
                target = runner.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        float targetLookAhead = lookAheadDistance;
        currentLookAhead = Mathf.Lerp(
            currentLookAhead,
            targetLookAhead,
            lookAheadSpeed * Time.deltaTime
        );

        float targetZoom = isChaseMode ? chaseZoom : normalZoom;
        currentZoom = Mathf.Lerp(
            currentZoom,
            targetZoom,
            zoomSpeed * Time.deltaTime
        );

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x + currentLookAhead,
            Mathf.Max(target.position.y + offset.y, minY),
            currentZoom
        );

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        if (shakeTimer > 0f)
        {
            smoothedPosition += Random.insideUnitSphere * shakeIntensity;
            shakeTimer -= Time.deltaTime;
        }

        transform.position = smoothedPosition;

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