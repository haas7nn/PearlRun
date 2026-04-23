using UnityEngine;

public class RunnerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float doubleJumpForce = 10f;
    public float slideTime = 0.6f;
    public float sprintMultiplier = 1.5f;
    public float sprintDuration = 2f;
    public float sprintCooldown = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    public LayerMask breakableLayer;
    public float attackAnimationDuration = 0.3f;
    public float attackStopDuration = 0.15f;
    public float attackRecoverDuration = 0.35f;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource runSource;
    public AudioSource musicSource;

    [Header("Player Sound Effects")]
    public AudioClip jumpClip;
    public AudioClip rollClip;
    public AudioClip deathClip;
    public AudioClip hurtClip;
    public AudioClip attackClip;
    public AudioClip runningClip;

    [Header("Scene Music")]
    public AudioClip backgroundMusic;

    [Header("Audio Volumes")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float runVolume = 0.8f;
    [Range(0f, 1f)] public float musicVolume = 0.6f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private bool isGrounded;
    private int jumpCount = 0;
    private bool isSliding;
    private float slideTimer;
    private bool isSprinting;
    private float sprintTimer;
    private float sprintCooldownTimer;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;
    private bool triggerRollFallOnLand;
    private float jumpTimestamp;

    private float attackAnimTimer;
    private float attackStopTimer;
    private float attackRecoverTimer;

    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isPunching;
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public bool isRunningBackward;
    [HideInInspector] public bool isDoubleJumping;

    public bool IsSliding => isSliding;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        if (capsuleCollider != null)
        {
            originalColliderHeight = capsuleCollider.height;
            originalColliderCenter = capsuleCollider.center;
        }

        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        SetupAudioSources();
        PlayBackgroundMusic();
    }

    void Update()
    {
        if (isDead || (GameManager.instance != null && GameManager.instance.isGameOver))
        {
            StopRunningSound();
            return;
        }

        UpdateAttackTimers();
        CheckGround();
        HandleJump();
        HandleSlide();
        HandleSprint();
        HandleAttack();
        UpdateAnimationFlags();

        currentSpeed = Mathf.Abs(rb.linearVelocity.x);

        HandleRunningSound();
    }

    void FixedUpdate()
    {
        if (isDead || (GameManager.instance != null && GameManager.instance.isGameOver))
            return;

        HandleMovement();
    }

    void SetupAudioSources()
    {
        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }

        if (runSource != null)
        {
            runSource.loop = true;
            runSource.playOnAwake = false;
            runSource.volume = runVolume;
        }

        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume;
        }
    }

    void UpdateAttackTimers()
    {
        if (attackAnimTimer > 0f)
        {
            attackAnimTimer -= Time.deltaTime;
            if (attackAnimTimer <= 0f)
                isPunching = false;
        }

        if (attackStopTimer > 0f)
            attackStopTimer -= Time.deltaTime;
        else if (attackRecoverTimer > 0f)
            attackRecoverTimer -= Time.deltaTime;
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && Time.time > jumpTimestamp + 0.1f)
        {
            if (!wasGrounded && jumpCount >= 2)
                triggerRollFallOnLand = true;

            jumpCount = 0;
            isJumping = false;
            isDoubleJumping = false;
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded && jumpCount == 0)
            {
                StopRunningSound();
                PerformJump(jumpForce);
                jumpCount = 1;
                isJumping = true;
                jumpTimestamp = Time.time;
                PlaySFX(jumpClip);
            }
            else if (jumpCount == 1)
            {
                StopRunningSound();
                PerformJump(doubleJumpForce);
                jumpCount = 2;
                isDoubleJumping = true;
                isJumping = true;
                PlaySFX(jumpClip);
            }
        }
    }

    void PerformJump(float force)
    {
        isGrounded = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    void HandleMovement()
    {
        if (isSliding)
            return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float speed = moveSpeed;

        if (isSprinting)
            speed *= sprintMultiplier;

        float attackSpeedMultiplier = 1f;

        if (attackStopTimer > 0f)
        {
            attackSpeedMultiplier = 0f;
        }
        else if (attackRecoverTimer > 0f)
        {
            float t = 1f - (attackRecoverTimer / attackRecoverDuration);
            attackSpeedMultiplier = Mathf.Clamp01(t);
        }

        float moveX = (speed + (horizontalInput * speed * 0.5f)) * attackSpeedMultiplier;
        rb.linearVelocity = new Vector3(moveX, rb.linearVelocity.y, 0f);
    }

    void HandleSlide()
    {
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && isGrounded && !isSliding)
            StartSlide();

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
                StopSlide();
        }
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideTime;

        StopRunningSound();
        PlaySFX(rollClip);

        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalColliderHeight * 0.4f;
            capsuleCollider.center = new Vector3(
                originalColliderCenter.x,
                originalColliderCenter.y * 0.4f,
                originalColliderCenter.z
            );
        }
    }

    void StopSlide()
    {
        isSliding = false;

        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalColliderHeight;
            capsuleCollider.center = originalColliderCenter;
        }
    }

    void HandleSprint()
    {
        if (sprintCooldownTimer > 0f)
            sprintCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprinting && sprintCooldownTimer <= 0f)
        {
            isSprinting = true;
            sprintTimer = sprintDuration;
        }

        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;

            if (sprintTimer <= 0f)
            {
                isSprinting = false;
                sprintCooldownTimer = sprintCooldown;
            }
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
        {
            isPunching = true;
            attackAnimTimer = attackAnimationDuration;
            attackStopTimer = attackStopDuration;
            attackRecoverTimer = attackRecoverDuration;

            StopRunningSound();
            PlaySFX(attackClip);

            if (attackPoint != null)
            {
                Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
                foreach (Collider enemy in hitEnemies)
                    enemy.GetComponent<EnemyBase>()?.TakeDamage(1);

                Collider[] hitBreakables = Physics.OverlapSphere(attackPoint.position, attackRange, breakableLayer);
                foreach (Collider b in hitBreakables)
                    Destroy(b.gameObject);
            }
        }
    }

    void UpdateAnimationFlags()
    {
        isRunningBackward = Input.GetAxisRaw("Horizontal") < -0.1f && isGrounded && !isSliding;
    }

    void HandleRunningSound()
    {
        bool isActuallyOnGround =
            isGrounded &&
            Mathf.Abs(rb.linearVelocity.y) < 0.05f &&
            !isJumping &&
            !isDoubleJumping;

        bool shouldRunSound =
            isActuallyOnGround &&
            !isSliding &&
            !isDead &&
            attackStopTimer <= 0f &&
            Mathf.Abs(rb.linearVelocity.x) > 0.2f;

        if (shouldRunSound)
        {
            if (runSource != null && runningClip != null)
            {
                if (runSource.clip != runningClip)
                    runSource.clip = runningClip;

                if (!runSource.isPlaying)
                    runSource.Play();
            }
        }
        else
        {
            StopRunningSound();
        }
    }

    void StopRunningSound()
    {
        if (runSource != null && runSource.isPlaying)
            runSource.Stop();
    }

    void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusic == null)
            return;

        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        musicSource.loop = true;

        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    public bool ConsumeRollFallTrigger()
    {
        if (triggerRollFallOnLand)
        {
            triggerRollFallOnLand = false;
            return true;
        }

        return false;
    }

    public void TakeDamage()
    {
        if (isDead)
            return;

        isHurt = true;
        PlaySFX(hurtClip);
        Invoke(nameof(ResetHurt), 0.5f);
        GameManager.instance?.PlayerHit();
    }

    void ResetHurt()
    {
        isHurt = false;
    }

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;

        StopRunningSound();
        PlaySFX(deathClip);
    }

    public void Respawn(Vector3 pos)
    {
        isDead = false;
        isHurt = false;
        isJumping = false;
        isDoubleJumping = false;
        rb.useGravity = true;
        transform.position = pos;
        rb.linearVelocity = Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}