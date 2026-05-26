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

    [Header("Better Jump (from friend's script)")]
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

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private bool isGrounded;
    private int jumpCount = 0;
    private float groundLockoutTimer; // FIX: Prevents instant ground re-detection

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
    }

    void Update()
    {
        if (isDead || (GameManager.instance != null && GameManager.instance.isGameOver))
            return;

        CheckGround();
        HandleJump();
        HandleSlide();
        HandleSprint();
        HandleAttack();

        currentSpeed = Mathf.Abs(rb.linearVelocity.x);
    }

    void FixedUpdate()
    {
        if (isDead || (GameManager.instance != null && GameManager.instance.isGameOver))
            return;

        ApplyJumpPhysics();
        HandleMovement();
    }

    void CheckGround()
    {
        // 1. If we just jumped, ignore ground for 0.15s so we can't infinite jump
        if (groundLockoutTimer > 0)
        {
            groundLockoutTimer -= Time.deltaTime;
            isGrounded = false;
            return;
        }

        // 2. Check if the sphere is touching the ground layer
        bool hittingGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // 3. Only reset jumps if touching ground AND moving downwards (prevents resetting while going up)
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

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                ExecuteJump(jumpForce);
                jumpCount = 1;
            }
            else if (jumpCount < 2) // If airborne and only jumped once
            {
                ExecuteJump(doubleJumpForce);
                jumpCount = 2; // Lock jumps until grounded again
            }
        }
    }

    void ExecuteJump(float force)
    {
        isJumping = true;
        groundLockoutTimer = 0.15f; // Forces ground check to be false for a moment

        // Clear Y velocity so the jump height is consistent
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0) // Falling
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space)) // Tapping jump
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        if (rb.linearVelocity.y < -maxFallSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxFallSpeed, 0f);
    }

    void HandleMovement()
    {
        if (isSliding) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float speed = moveSpeed;

        if (isSprinting) speed *= sprintMultiplier;

        float moveX = speed + (horizontalInput * speed * 0.5f);
        rb.linearVelocity = new Vector3(moveX, rb.linearVelocity.y, 0f);
    }

    // --- RE-ADDED MISSING FUNCTIONS FOR COLLISION SCRIPT ---

    public void TakeDamage()
    {
        if (isDead || isHurt) return;

        isHurt = true;
        // Make the player fall faster/stop jump when hit
        rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y, 0f);

        Invoke("ResetHurt", 0.5f);

        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerHit();
        }
    }

    void ResetHurt()
    {
        isHurt = false;
    }

    // --- SLIDE, SPRINT, ATTACK ---

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
            capsuleCollider.center = new Vector3(originalColliderCenter.x, originalColliderCenter.y * 0.4f, originalColliderCenter.z);
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
        if (sprintCooldownTimer > 0) sprintCooldownTimer -= Time.deltaTime;
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
            Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer | breakableLayer);
            foreach (Collider hit in hits)
            {
                if (((1 << hit.gameObject.layer) & enemyLayer) != 0)
                    hit.GetComponent<EnemyBase>()?.TakeDamage(1);
                else
                    Destroy(hit.gameObject);
            }
            Invoke("ResetPunch", 0.3f);
        }
    }

    void ResetPunch() => isPunching = false;

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;
    }

    public void Respawn(Vector3 pos)
    {
        isDead = false;
        rb.useGravity = true;
        transform.position = pos;
        jumpCount = 0;
        isHurt = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}