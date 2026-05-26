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

    [Header("Enemy Awareness")]
    public float enemyVisibleDistance = 12f;
    public float enemyAwarenessSpeed = 3f;

    [Header("Screen Shake")]
    private float shakeTimer = 0f;
    private float shakeIntensity = 0f;

    [Header("Boundaries")]
    public float minY = 2f;

    private float currentZoom = 0f;
    private float currentLookAhead = 0f;
    private float enemyBlend = 0f;
    private Transform enemyTransform;

    private void Start()
    {
        currentZoom = normalZoom;

        if (target == null)
        {
            RunnerController runner = FindAnyObjectByType<RunnerController>();
            if (runner != null)
                target = runner.transform;
        }

        // ابحث عن العدو
        EnemyChase_Rana enemy = FindAnyObjectByType<EnemyChase_Rana>();
        if (enemy != null)
            enemyTransform = enemy.transform;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float distToEnemy = enemyTransform != null
            ? Mathf.Abs(target.position.x - enemyTransform.position.x)
            : float.MaxValue;

        float targetBlend = (enemyTransform != null && distToEnemy < enemyVisibleDistance)
            ? Mathf.Clamp01(1f - (distToEnemy / enemyVisibleDistance))
            : 0f;

        enemyBlend = Mathf.Lerp(enemyBlend, targetBlend, enemyAwarenessSpeed * Time.deltaTime);

        // ── Look Ahead ──
        currentLookAhead = Mathf.Lerp(currentLookAhead, lookAheadDistance, lookAheadSpeed * Time.deltaTime);

        float targetZoom = Mathf.Lerp(normalZoom, chaseZoom, enemyBlend);
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomSpeed * Time.deltaTime);

        Vector3 focusPoint = target.position;
        if (enemyTransform != null && enemyBlend > 0.01f)
        {
            Vector3 midPoint = Vector3.Lerp(target.position, enemyTransform.position, enemyBlend * 0.3f);
            focusPoint = midPoint;
        }

        Vector3 desiredPosition = new Vector3(
            focusPoint.x + offset.x + currentLookAhead,
            Mathf.Max(focusPoint.y + offset.y, minY),
            currentZoom
        );

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // ── Screen Shake ──
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

    public void SetChaseMode(bool chase) { }
}