using UnityEngine;

public class EnemyChase_Rana : MonoBehaviour
{
    [Header("Chase Settings")]
    public Transform player;
    public float chaseSpeed = 5f;
    public float normalGap = 4f;

    [Header("Obstacle Detection")]
    public float obstacleCheckDistance = 1.0f;
    public float obstacleCheckHeight = 1.2f;
    public LayerMask obstacleLayer;
    public string[] obstacleTags = { "Obstacle", "JumpObstacle" };

    [Header("Touch Detection")]
    public float touchDistance = 0.8f;
    public float damageCooldown = 1.5f;

    [Header("Audio (Optional)")]
    public AudioSource enemyAudioSource;
    public AudioClip chaseSound;
    public float maxHearDistance = 8f;
    public float minHearDistance = 18f;

    private Animator animator;
    private RunnerController runnerController;
    private bool isStopped = false;
    private float damageTimer = 0f;
    private float groundY;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        groundY = transform.position.y;

        if (player == null)
        {
            GameObject p = GameObject.Find("Awal");
            if (p != null)
            {
                player = p.transform;
                runnerController = p.GetComponent<RunnerController>();
            }
        }
        else
        {
            runnerController = player.GetComponent<RunnerController>();
        }

        SetupAudio();
        animator.SetBool("isRunning", true);
    }

    void Update()
    {
        if (player == null) return;

        if (RunnerGameManager.instance != null && RunnerGameManager.instance.isGameOver)
        {
            SetRunning(false);
            return;
        }

        if (damageTimer > 0f) damageTimer -= Time.deltaTime;

        // ── واقف على عائق ──
        if (isStopped)
        {
            SetRunning(false);
            return;
        }

        // ── كشف العوائق ──
        if (IsObstacleAhead())
        {
            isStopped = true;
            SetRunning(false);
            return;
        }

        // ── يلاحق أوال مع مسافة طبيعية ──
        float targetX = player.position.x - normalGap;
        float newX = Mathf.MoveTowards(transform.position.x, targetX, chaseSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, groundY, transform.position.z);

        SetRunning(true);
        UpdateAudio();

        // ── لمس أوال ──
        float dist = Mathf.Abs(transform.position.x - player.position.x);
        if (dist <= touchDistance && damageTimer <= 0f)
        {
            damageTimer = damageCooldown;
            RunnerGameManager.instance?.PlayerHit();
        }
    }

    bool IsObstacleAhead()
    {
        Vector3 center = transform.position + Vector3.right * obstacleCheckDistance
                                            + Vector3.up * (obstacleCheckHeight / 2f);
        Vector3 size = new Vector3(0.3f, obstacleCheckHeight, 0.5f);

        if (Physics.OverlapBox(center, size / 2f, Quaternion.identity, obstacleLayer).Length > 0)
            return true;

        foreach (Collider col in Physics.OverlapBox(center, size / 2f))
            foreach (string tag in obstacleTags)
                if (col.CompareTag(tag)) return true;

        return false;
    }

    void SetRunning(bool running)
    {
        animator.SetBool("isRunning", running);
        if (!running) StopAudio();
    }

    void SetupAudio()
    {
        if (enemyAudioSource == null || chaseSound == null) return;
        enemyAudioSource.clip = chaseSound;
        enemyAudioSource.loop = true;
        enemyAudioSource.spatialBlend = 1f;
        enemyAudioSource.Play();
    }

    void UpdateAudio()
    {
        if (enemyAudioSource == null || chaseSound == null) return;

        if (transform.position.x < player.position.x)
        {
            if (!enemyAudioSource.isPlaying) enemyAudioSource.Play();
            float dist = Vector3.Distance(transform.position, player.position);
            enemyAudioSource.volume = Mathf.Clamp01(
                Mathf.InverseLerp(minHearDistance, maxHearDistance, dist));
        }
        else StopAudio();
    }

    void StopAudio()
    {
        if (enemyAudioSource != null && enemyAudioSource.isPlaying)
            enemyAudioSource.Stop();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + Vector3.right * obstacleCheckDistance
                                            + Vector3.up * (obstacleCheckHeight / 2f);
        Gizmos.DrawWireCube(center, new Vector3(0.3f, obstacleCheckHeight, 0.5f));
    }
}