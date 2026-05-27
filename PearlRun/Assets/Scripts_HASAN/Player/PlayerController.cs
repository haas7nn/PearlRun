using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float doubleJumpForce = 10f;
    public float slideTime = 0.6f;
    public float sprintMultiplier = 1.5f;
    public float sprintDuration = 2f;
    public float sprintCooldown = 8f;

    [Header("Better Jump")]
    public float fallMultiplier = 4f;
    public float lowJumpMultiplier = 2.5f;
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

    [Header("Player Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource runningAudioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landingSound;
    [SerializeField] private AudioClip runningSound;
    [SerializeField] private AudioClip deathSound;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private bool isGrounded;
    private bool wasGrounded;
    private int jumpCount = 0;
    private float groundLockoutTimer; // Prevents instant ground re-detection
    
    private bool isSliding;
    private float slideTimer;
    private bool isSprinting;
    private float sprintTimer;
    private float sprintCooldownTimer;
    
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;

    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isPunching;
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead;
    [HideInInspector] public float currentSpeed;

    public bool IsSliding => isSliding;
    public bool IsGrounded => isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (runningAudioSource != null && runningSound != null)
        {
            runningAudioSource.clip = runningSound;
            runningAudioSource.loop = true;
            runningAudioSource.playOnAwake = false;
        }

        if (capsuleCollider != null)
        {
            originalColliderHeight = capsuleCollider.height;
            originalColliderCenter = capsuleCollider.center;
        }

        sprintCooldownTimer = 0f;

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionZ | 
                             RigidbodyConstraints.FreezeRotation;
        }
    }

    void Update()
    {
        if (isDead || (Level3GameManager.instance != null && Level3GameManager.instance.isGameOver))
        {
            StopRunningSound();
            return;
        }

        wasGrounded = isGrounded;

        CheckGround();
        CheckLandingSound();
        HandleJump();
        HandleSlide();
        HandleSprint();
        HandleAttack();
        HandleRunningSound();

        if (rb != null)
            currentSpeed = Mathf.Abs(rb.linearVelocity.x);
    }

    void FixedUpdate()
    {
        if (isDead || (Level3GameManager.instance != null && Level3GameManager.instance.isGameOver))
            return;

        ApplyJumpPhysics();
        HandleMovement();
    }

    void CheckGround()
    {
        if (groundLockoutTimer > 0)
        {
            groundLockoutTimer -= Time.deltaTime;
            isGrounded = false;
            return;
        }

        bool hittingGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (hittingGround && rb.linearVelocity.y <= 0.01f)
        {
            isGrounded = true;
            jumpCount = 0;
            isJumping = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    void CheckLandingSound()
    {
        if (!wasGrounded && isGrounded)
        {
            PlaySound(landingSound);
        }
    }

    void HandleJump()
    {
        if (rb == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                ExecuteJump(jumpForce);
                jumpCount = 1;
            }
            else if (jumpCount < 2)
            {
                ExecuteJump(doubleJumpForce);
                jumpCount = 2;
            }
        }
    }

    void ExecuteJump(float force)
    {
        isJumping = true;
        groundLockoutTimer = 0.15f;
        
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        
        PlaySound(jumpSound);
    }

    void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        if (rb.linearVelocity.y < -maxFallSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxFallSpeed, 0f);
    }

    void HandleMovement()
    {
        if (rb == null || isSliding) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float speed = moveSpeed;

        if (isSprinting) speed *= sprintMultiplier;

        float moveX = speed + (horizontalInput * speed * 0.5f);
        rb.linearVelocity = new Vector3(moveX, rb.linearVelocity.y, 0f);
    }

    void HandleSlide()
    {
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && isGrounded && !isSliding)
            StartSlide();

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0) StopSlide();
        }
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideTime;
        
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
        if (sprintCooldownTimer > 0) 
            sprintCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isSprinting && sprintCooldownTimer <= 0)
        {
            isSprinting = true;
            sprintTimer = sprintDuration;
        }

        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer <= 0 || !isGrounded)
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
            
            if (attackPoint != null)
            {
                Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer | breakableLayer);
                
                foreach (Collider hit in hits)
                {
                    if (((1 << hit.gameObject.layer) & enemyLayer) != 0)
                    {
                        hit.GetComponent<EnemyBase>()?.TakeDamage(1);
                    }
                    else if (((1 << hit.gameObject.layer) & breakableLayer) != 0)
                    {
                        Destroy(hit.gameObject);
                    }
                }
            }
            
            Invoke(nameof(ResetPunch), 0.3f);
        }
    }

    void ResetPunch() => isPunching = false;

    public void TakeDamage()
    {
        if (isDead || isHurt) return;

        isHurt = true;
        rb.linearVelocity = new Vector3(-5f, 8f, 0f);

        Invoke(nameof(ResetHurt), 0.5f);

        if (RunnerGameManager.instance != null)
            RunnerGameManager.instance.PlayerHit();
    }

    void ResetHurt() => isHurt = false;

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        StopRunningSound();
        PlaySound(deathSound);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.useGravity = false;
        }
    }

    public void Respawn(Vector3 pos)
    {
        isDead = false;
        isHurt = false;
        isJumping = false;
        isPunching = false;
        isSliding = false;
        jumpCount = 0;

        if (rb != null)
        {
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
        }

        transform.position = pos;

        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalColliderHeight;
            capsuleCollider.center = originalColliderCenter;
        }
    }

    void HandleRunningSound()
    {
        if (rb == null || runningAudioSource == null || runningSound == null)
            return;

        bool shouldPlayRunSound = isGrounded && !isSliding && !isDead && Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        if (shouldPlayRunSound)
        {
            if (!runningAudioSource.isPlaying)
                runningAudioSource.Play();
        }
        else
        {
            StopRunningSound();
        }
    }

    void StopRunningSound()
    {
        if (runningAudioSource != null && runningAudioSource.isPlaying)
            runningAudioSource.Stop();
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
