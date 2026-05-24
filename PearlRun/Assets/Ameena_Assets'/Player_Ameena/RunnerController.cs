using UnityEngine;

public class RunnerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 10f;
    public float doubleJumpForce = 9f;
    public float slideTime = 0.6f;
    public float sprintMultiplier = 1.5f;
    public float sprintDuration = 2f;
    public float sprintCooldown = 8f;

    [Header("Better Jump")]
    public float fallMultiplier = 4f;
    public float lowJumpMultiplier = 2f;
    public float maxFallSpeed = 22f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    public LayerMask breakableLayer;
    public float attackAnimationDuration = 0.3f;
    public float attackStopDuration = 0.15f;
    public float attackRecoverDuration = 0.35f;

    [Header("Obstacle Hit")]
    public float obstacleSlowMultiplier = 0.35f;
    public float obstacleSlowDuration = 0.45f;
    public float obstacleRecoverDuration = 0.35f;

    [Header("Input Feel")]
    public float jumpBufferTime = 0.18f;
    public float actionBufferTime = 0.10f;
    public float coyoteTime = 0.14f;

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
    private bool wasGrounded;
    private int jumpCount;
    private bool hasReachedApex;

    private bool isSliding;
    private float slideTimer;

    private bool isSprinting;
    private float sprintTimer;
    private float sprintCooldownTimer;

    private float originalColliderHeight;
    private Vector3 originalColliderCenter;

    private bool triggerRollFallOnLand;

    private float attackAnimTimer;
    private float attackStopTimer;
    private float attackRecoverTimer;

    private float obstacleSlowTimer;
    private float obstacleRecoverTimer;
    private float obstacleCurrentMultiplier = 1f;
    private float obstacleStartRecoverMultiplier = 1f;

    private float jumpBufferTimer;
    private float slideBufferTimer;
    private float attackBufferTimer;
    private float coyoteTimer;

    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isPunching;
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public bool isRunningBackward;
    [HideInInspector] public bool isDoubleJumping;

    public bool IsSliding => isSliding;
    public bool IsGrounded => isGrounded;

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

        isGrounded = true;
        wasGrounded = true;

        SetupAudioSources();
        PlayBackgroundMusic();
    }

    void Update()
    {
        if (isDead || (RunnerGameManager.instance != null && RunnerGameManager.instance.isGameOver))
        {
            StopRunningSound();
            return;
        }

        UpdateTimers();
        CheckGround();
        ReadInput();
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
        if (isDead || (RunnerGameManager.instance != null && RunnerGameManager.instance.isGameOver))
            return;

        BetterJump();
        HandleMovement();
    }

    void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            slideBufferTimer = actionBufferTime;

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.F))
            attackBufferTimer = actionBufferTime;
    }

    void UpdateTimers()
    {
        jumpBufferTimer -= Time.deltaTime;
        slideBufferTimer -= Time.deltaTime;
        attackBufferTimer -= Time.deltaTime;

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

        if (sprintCooldownTimer > 0f)
            sprintCooldownTimer -= Time.deltaTime;

        UpdateObstacleSlowdown();
    }

    void CheckGround()
    {
        wasGrounded = isGrounded;

        LayerMask groundAndObstacle = groundLayer | LayerMask.GetMask("Obstacle");

        bool sphereCheck = false;
        bool castCheck = false;

        if (groundCheck != null)
        {
            sphereCheck = Physics.CheckSphere(
                groundCheck.position,
                groundCheckRadius,
                groundAndObstacle,
                QueryTriggerInteraction.Ignore
            );

            castCheck = Physics.SphereCast(
                groundCheck.position + Vector3.up * 0.15f,
                groundCheckRadius,
                Vector3.down,
                out RaycastHit hit,
                0.45f,
                groundAndObstacle,
                QueryTriggerInteraction.Ignore
            );
        }

        isGrounded = sphereCheck || castCheck;

        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        if (jumpCount > 0 && rb.linearVelocity.y < -1f)
            hasReachedApex = true;

        if (!wasGrounded && isGrounded && jumpCount > 0 && hasReachedApex)
        {
            // Don't fire the heavy-landing animation if the player is hurt —
            // it conflicts with the Hurt state and freezes the animator.
            if (jumpCount >= 2 && !isHurt)
                triggerRollFallOnLand = true;

            jumpCount = 0;
            hasReachedApex = false;
            isJumping = false;
            isDoubleJumping = false;
        }
    }

    void HandleJump()
    {
        if (jumpBufferTimer <= 0f) return;
        if (isSliding) return;

        if ((isGrounded || coyoteTimer > 0f) && jumpCount == 0)
        {
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            CancelAttackLock();

            jumpCount = 1;
            hasReachedApex = false;
            isJumping = true;
            isDoubleJumping = false;
            isGrounded = false;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            StopRunningSound();
            PlaySFX(jumpClip);
        }
        else if (!isGrounded && jumpCount == 1)
        {
            jumpBufferTimer = 0f;
            CancelAttackLock();

            jumpCount = 2;
            hasReachedApex = false;
            isJumping = true;
            isDoubleJumping = true;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
            rb.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);

            StopRunningSound();
            PlaySFX(jumpClip);
        }
    }

    void CancelAttackLock()
    {
        isPunching = false;
        attackAnimTimer = 0f;
        attackStopTimer = 0f;
        attackRecoverTimer = 0f;
    }

    void BetterJump()
    {
        if (rb.linearVelocity.y < 0f)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y > 0f && !Input.GetKey(KeyCode.Space))
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;

        if (rb.linearVelocity.y < -maxFallSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxFallSpeed, rb.linearVelocity.z);
    }

    void HandleSlide()
    {
        if (slideBufferTimer > 0f && isGrounded && !isSliding && !isJumping && !isDoubleJumping)
        {
            slideBufferTimer = 0f;
            StartSlide();
        }

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

    void HandleAttack()
    {
        if (attackBufferTimer <= 0f) return;
        if (!isGrounded || isSliding || isPunching || isJumping || isDoubleJumping) return;

        attackBufferTimer = 0f;
        isPunching = true;
        attackAnimTimer = attackAnimationDuration;
        attackStopTimer = attackStopDuration;
        attackRecoverTimer = attackRecoverDuration;

        StopRunningSound();
        PlaySFX(attackClip);

        if (attackPoint != null)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, enemyLayer);
            foreach (Collider enemy in hitEnemies)
                enemy.GetComponent<EnemyBase>()?.TakeDamage(1);

            Collider[] hitBreakables = Physics.OverlapSphere(attackPoint.position, attackRange, breakableLayer);
            foreach (Collider b in hitBreakables)
                Destroy(b.gameObject);
        }
    }

    void HandleSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isSprinting && sprintCooldownTimer <= 0f && !isSliding && !isPunching)
        {
            isSprinting = true;
            sprintTimer = sprintDuration;
        }

        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer <= 0f || !isGrounded)
            {
                isSprinting = false;
                sprintCooldownTimer = sprintCooldown;
            }
        }
    }

    void HandleMovement()
    {
        if (isSliding) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float speed = moveSpeed;

        if (isSprinting) speed *= sprintMultiplier;

        float attackSpeedMultiplier = 1f;

        if (attackStopTimer > 0f)
            attackSpeedMultiplier = 0f;
        else if (attackRecoverTimer > 0f)
        {
            float t = 1f - (attackRecoverTimer / attackRecoverDuration);
            attackSpeedMultiplier = Mathf.Clamp01(t);
        }

        float totalMultiplier = attackSpeedMultiplier * obstacleCurrentMultiplier;
        float moveX = (speed + horizontalInput * speed * 0.5f) * totalMultiplier;

        rb.linearVelocity = new Vector3(moveX, rb.linearVelocity.y, 0f);
    }

    void UpdateAnimationFlags()
    {
        isRunningBackward =
            Input.GetAxisRaw("Horizontal") < -0.1f &&
            isGrounded &&
            !isSliding &&
            !isPunching &&
            !isJumping &&
            !isDoubleJumping &&
            !isHurt;
    }

    void UpdateObstacleSlowdown()
    {
        if (obstacleSlowTimer > 0f)
        {
            obstacleSlowTimer -= Time.deltaTime;
            obstacleCurrentMultiplier = obstacleSlowMultiplier;

            if (obstacleSlowTimer <= 0f)
            {
                obstacleRecoverTimer = obstacleRecoverDuration;
                obstacleStartRecoverMultiplier = obstacleCurrentMultiplier;
            }
        }
        else if (obstacleRecoverTimer > 0f)
        {
            obstacleRecoverTimer -= Time.deltaTime;
            float t = 1f - (obstacleRecoverTimer / obstacleRecoverDuration);
            obstacleCurrentMultiplier = Mathf.Lerp(obstacleStartRecoverMultiplier, 1f, t);

            if (obstacleRecoverTimer <= 0f)
                obstacleCurrentMultiplier = 1f;
        }
        else
        {
            obstacleCurrentMultiplier = 1f;
        }
    }

    void HandleRunningSound()
    {
        bool shouldRunSound =
            isGrounded &&
            !isSliding &&
            !isPunching &&
            !isHurt &&
            !isDead &&
            !isJumping &&
            !isDoubleJumping &&
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
        if (musicSource == null || backgroundMusic == null) return;

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

    public void ApplyObstacleSlowdown(float slowMultiplier, float slowDuration)
    {
        obstacleSlowMultiplier = Mathf.Clamp01(slowMultiplier);
        obstacleSlowDuration = Mathf.Max(0.05f, slowDuration);

        obstacleSlowTimer = obstacleSlowDuration;
        obstacleRecoverTimer = 0f;
        obstacleCurrentMultiplier = obstacleSlowMultiplier;
    }

    public void TakeDamage()
    {
        if (isDead) return;

        isHurt = true;
        isPunching = false;
        isSliding = false;

        // FIX: clear all airborne / landing flags so the animator
        // never has Hurt + Jump + DoubleJump + RollFall true at once.
        isJumping = false;
        isDoubleJumping = false;
        jumpCount = 0;
        hasReachedApex = false;
        triggerRollFallOnLand = false;

        // FIX: also clear attack timers so movement isn't locked while hurt.
        attackAnimTimer = 0f;
        attackStopTimer = 0f;
        attackRecoverTimer = 0f;

        StopSlide();
        PlaySFX(hurtClip);

        CancelInvoke(nameof(ResetHurt));
        Invoke(nameof(ResetHurt), 0.2f);

        RunnerGameManager.instance?.PlayerHit();
    }

    public void ForceGrounded()
    {
        isJumping = false;
        isDoubleJumping = false;
        jumpCount = 0;
        hasReachedApex = false;
        isGrounded = true;
        isHurt = false;

        // FIX: clear the pending heavy-landing trigger so it
        // doesn't fire after a forced snap onto an obstacle.
        triggerRollFallOnLand = false;

        CancelInvoke(nameof(ResetHurt));

        obstacleSlowTimer = 0f;
        obstacleRecoverTimer = 0f;
        obstacleCurrentMultiplier = 1f;
    }

    void ResetHurt()
    {
        isHurt = false;

        // FIX: guarantee a clean run state when hurt ends.
        // Scrub any leftover landing trigger or jump residue that
        // could re-freeze the animator on the next frame.
        if (isGrounded)
        {
            triggerRollFallOnLand = false;
            isJumping = false;
            isDoubleJumping = false;
            jumpCount = 0;
            hasReachedApex = false;
        }
    }

    public void Die()
    {
        isDead = true;
        isHurt = false;
        isPunching = false;
        isSliding = false;
        isJumping = false;
        isDoubleJumping = false;
        isRunningBackward = false;

        jumpBufferTimer = 0f;
        slideBufferTimer = 0f;
        attackBufferTimer = 0f;

        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalColliderHeight;
            capsuleCollider.center = originalColliderCenter;
        }

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
        isPunching = false;
        isSliding = false;
        isRunningBackward = false;

        jumpCount = 0;
        hasReachedApex = false;
        isGrounded = true;
        wasGrounded = true;
        triggerRollFallOnLand = false;

        jumpBufferTimer = 0f;
        slideBufferTimer = 0f;
        attackBufferTimer = 0f;
        coyoteTimer = 0f;

        obstacleCurrentMultiplier = 1f;
        obstacleSlowTimer = 0f;
        obstacleRecoverTimer = 0f;

        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalColliderHeight;
            capsuleCollider.center = originalColliderCenter;
        }

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