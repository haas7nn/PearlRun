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
    public float groundCheckRadius = 0.2f; // Smaller radius is more accurate
    public LayerMask groundLayer;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    public LayerMask breakableLayer;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private bool isGrounded;
    private int jumpCount = 0; // Explicitly track jump numbers: 0=ground, 1=first, 2=double
    private bool isSliding;
    private float slideTimer;
    private bool isSprinting;
    private float sprintTimer;
    private float sprintCooldownTimer;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;
    private bool triggerRollFallOnLand;
    private float jumpTimestamp; // Prevents instant re-grounding logic

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
        UpdateAnimationFlags();

        currentSpeed = Mathf.Abs(rb.linearVelocity.x);
    }

    void FixedUpdate()
    {
        if (isDead || (GameManager.instance != null && GameManager.instance.isGameOver))
            return;

        HandleMovement();
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;

        // Physics check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // RESET LOGIC: 
        // Only reset jumps if we are grounded AND we haven't just pressed the jump button (cooldown)
        if (isGrounded && Time.time > jumpTimestamp + 0.1f)
        {
            if (!wasGrounded && jumpCount >= 2)
            {
                triggerRollFallOnLand = true;
            }

            jumpCount = 0;
            isJumping = false;
            isDoubleJumping = false;
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Case 1: First Jump from ground
            if (isGrounded && jumpCount == 0)
            {
                PerformJump(jumpForce);
                jumpCount = 1;
                isJumping = true;
                jumpTimestamp = Time.time; // Record time to prevent instant reset
            }
            // Case 2: Second Jump (Double Jump)
            else if (jumpCount == 1)
            {
                PerformJump(doubleJumpForce);
                jumpCount = 2; // Now jumpCount is 2, logic will block any further jumps
                isDoubleJumping = true;
                isJumping = true;
            }
        }
    }

    void PerformJump(float force)
    {
        // Zero out Y velocity before jump for consistent height
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
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

    // --- OTHER METHODS UNCHANGED BUT INCLUDED FOR COMPLETION ---

    void HandleSlide()
    {
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && isGrounded && !isSliding)
            StartSlide();

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f) StopSlide();
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
        if (sprintCooldownTimer > 0f) sprintCooldownTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprinting && sprintCooldownTimer <= 0f)
        {
            isSprinting = true;
            sprintTimer = sprintDuration;
        }
        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer <= 0f) { isSprinting = false; sprintCooldownTimer = sprintCooldown; }
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
        {
            isPunching = true;
            if (attackPoint != null)
            {
                Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
                foreach (Collider enemy in hitEnemies) enemy.GetComponent<EnemyBase>()?.TakeDamage(1);
                Collider[] hitBreakables = Physics.OverlapSphere(attackPoint.position, attackRange, breakableLayer);
                foreach (Collider b in hitBreakables) Destroy(b.gameObject);
            }
            Invoke(nameof(ResetPunch), 0.3f);
        }
    }

    void ResetPunch() => isPunching = false;
    void UpdateAnimationFlags() => isRunningBackward = Input.GetAxisRaw("Horizontal") < -0.1f && isGrounded && !isSliding;

    public bool ConsumeRollFallTrigger() { if (triggerRollFallOnLand) { triggerRollFallOnLand = false; return true; } return false; }

    public void TakeDamage() { if (isDead) return; isHurt = true; Invoke(nameof(ResetHurt), 0.5f); GameManager.instance?.PlayerHit(); }
    void ResetHurt() => isHurt = false;

    public void Die() { isDead = true; rb.linearVelocity = Vector3.zero; rb.useGravity = false; }

    public void Respawn(Vector3 pos) { isDead = false; isHurt = false; isJumping = false; isDoubleJumping = false; rb.useGravity = true; transform.position = pos; rb.linearVelocity = Vector3.zero; }

    void OnDrawGizmosSelected()
    {
        if (groundCheck) { Gizmos.color = Color.green; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); }
        if (attackPoint) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(attackPoint.position, attackRange); }
    }
}