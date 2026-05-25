using UnityEngine;

public class EnemyChase_Rana : MonoBehaviour
{
    [Header("Chase Settings")]
    public Transform player;
    public float chaseSpeed = 6f;

    [Header("Reset After Player Hit")]
    public float resetOffsetBehindPlayer = 5f;

    [Header("Obstacle Detection")]
    public float obstacleCheckDistance = 1.0f;
    public float obstacleCheckHeight = 1.0f;
    public LayerMask obstacleLayer;
    public string[] obstacleTags = { "Obstacle", "JumpObstacle" };

    [Header("Touch Detection")]
    public float touchDistance = 1.0f;
    private float damageCooldown = 1.5f;
    private float damageTimer = 0f;

    [Header("Audio (Optional)")]
    public AudioSource enemyAudioSource;
    public AudioClip chaseSound;
    public float maxHearDistance = 10f;
    public float minHearDistance = 20f;

    private Animator animator;
    private bool isStopped = false;
    private RunnerController runnerController;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

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

        if (enemyAudioSource != null && chaseSound != null)
        {
            enemyAudioSource.clip = chaseSound;
            enemyAudioSource.loop = true;
            enemyAudioSource.spatialBlend = 1f;
            enemyAudioSource.Play();
        }

        animator.SetBool("isRunning", true);
    }

    void Update()
    {
        if (player == null) return;

        if (RunnerGameManager.instance != null && RunnerGameManager.instance.isGameOver)
        {
            animator.SetBool("isRunning", false);
            StopAudio();
            return;
        }

        if (damageTimer > 0f)
            damageTimer -= Time.deltaTime;

        if (isStopped)
        {
            animator.SetBool("isRunning", false);
            StopAudio();
            return;
        }

        // ── Check obstacle ahead ──
        if (IsObstacleAhead())
        {
            isStopped = true;
            animator.SetBool("isRunning", false);
            StopAudio();
            return;
        }

        // ── Chase Awal ──
        float newX = Mathf.MoveTowards(
            transform.position.x,
            player.position.x,
            chaseSpeed * Time.deltaTime
        );
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        animator.SetBool("isRunning", true);
        UpdateAudio();

        // ── Touch detection ──
        float dist = Mathf.Abs(transform.position.x - player.position.x);
        if (dist <= touchDistance && damageTimer <= 0f)
        {
            damageTimer = damageCooldown;
            RunnerGameManager.instance?.PlayerHit();
        }
    }

    bool IsObstacleAhead()
    {
        Vector3 boxCenter = transform.position
                          + Vector3.right * obstacleCheckDistance
                          + Vector3.up * (obstacleCheckHeight / 2f);
        Vector3 boxSize = new Vector3(0.3f, obstacleCheckHeight, 0.5f);

        Collider[] hits = Physics.OverlapBox(boxCenter, boxSize / 2f, Quaternion.identity, obstacleLayer);
        if (hits.Length > 0) return true;

        Collider[] allHits = Physics.OverlapBox(boxCenter, boxSize / 2f);
        foreach (Collider col in allHits)
        {
            foreach (string tag in obstacleTags)
            {
                if (col.CompareTag(tag)) return true;
            }
        }

        return false;
    }

    // ── Called from RunnerCollisionHandler when Awal hits obstacle ──
    public void TriggerReset()
    {
        ResetBehindPlayer();
    }

    void ResetBehindPlayer()
    {
        if (player == null) return;

        transform.position = new Vector3(
            player.position.x - resetOffsetBehindPlayer,
            player.position.y,
            player.position.z
        );

        isStopped = false;
        animator.SetBool("isRunning", true);
        UpdateAudio();
    }

    void UpdateAudio()
    {
        if (enemyAudioSource == null || chaseSound == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (transform.position.x < player.position.x)
        {
            if (!enemyAudioSource.isPlaying)
                enemyAudioSource.Play();

            float volume = Mathf.InverseLerp(minHearDistance, maxHearDistance, dist);
            enemyAudioSource.volume = Mathf.Clamp01(volume);
        }
        else
        {
            StopAudio();
        }
    }

    void StopAudio()
    {
        if (enemyAudioSource != null && enemyAudioSource.isPlaying)
            enemyAudioSource.Stop();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position
                          + Vector3.right * obstacleCheckDistance
                          + Vector3.up * (obstacleCheckHeight / 2f);
        Vector3 boxSize = new Vector3(0.3f, obstacleCheckHeight, 0.5f);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}