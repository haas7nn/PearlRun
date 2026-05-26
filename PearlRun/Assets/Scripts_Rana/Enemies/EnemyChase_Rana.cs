using UnityEngine;

public class EnemyChase_Rana : MonoBehaviour
{
    [Header("Chase Settings")]
    public Transform player;
    public float chaseSpeed = 5f;

    [Header("Reset Settings")]
    public float resetGap = 8f;
    public float resetImmunityTime = 2f;

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
    private float immunityTimer = 0f;

    private bool wasHurt = false;
    private int previousLives = -1;

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

        if (RunnerGameManager.instance != null)
            previousLives = RunnerGameManager.instance.currentLives;

        SetupAudio();
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

        if (damageTimer > 0f) damageTimer -= Time.deltaTime;
        if (immunityTimer > 0f) immunityTimer -= Time.deltaTime;

        if (runnerController != null)
        {
            bool hurtNow = runnerController.isHurt;
            if (hurtNow && !wasHurt)
            {
                wasHurt = true;
                ResetBehindPlayer();
                return;
            }
            if (!hurtNow) wasHurt = false;
        }

        if (RunnerGameManager.instance != null)
        {
            int lives = RunnerGameManager.instance.currentLives;
            if (previousLives != -1 && lives < previousLives)
            {
                previousLives = lives;
                ResetBehindPlayer();
                return;
            }
            previousLives = lives;
        }

        if (isStopped)
        {
            animator.SetBool("isRunning", false);
            StopAudio();
            return;
        }

        if (IsObstacleAhead())
        {
            isStopped = true;
            animator.SetBool("isRunning", false);
            StopAudio();
            return;
        }

        float newX = Mathf.MoveTowards(transform.position.x, player.position.x, chaseSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        animator.SetBool("isRunning", true);
        UpdateAudio();

        float dist = Mathf.Abs(transform.position.x - player.position.x);
        if (dist <= touchDistance && damageTimer <= 0f && immunityTimer <= 0f)
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

    public void TriggerReset() => ResetBehindPlayer();

    void ResetBehindPlayer()
    {
        if (player == null) return;

        transform.position = new Vector3(
            player.position.x - resetGap,
            transform.position.y,
            transform.position.z
        );

        isStopped = false;
        immunityTimer = resetImmunityTime;

        animator.SetBool("isRunning", true);
        UpdateAudio();
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