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

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    public LayerMask breakableLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool isSliding;
    private float slideTimer;
    private bool isSprinting;
    private float sprintTimer;
    private float sprintCooldownTimer;
    private Vector3 originalScale;
    private CapsuleCollider capsuleCollider;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;

    // Animation parameters
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

        originalScale = transform.localScale;
        sprintCooldownTimer = 0f;

        // Freeze Z position and all rotations for 2.5D
        rb.constraints = RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver)
            return;

        if (isDead)
            return;

        CheckGround();
        HandleJump();
        HandleSlide();
        HandleSprint();
        HandleAttack();

        // Update animation speed parameter
        currentSpeed = Mathf.Abs(rb.linearVelocity.x);
    }

    void FixedUpdate()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver)
            return;

        if (isDead)
            return;

        HandleMovement();
    }

    void CheckGround()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            // Fallback raycast ground check
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
        }

        if (isGrounded)
        {
            isJumping = false;
            canDoubleJump = true;
        }
    }

    void HandleMovement()
    {
        if (isSliding)
            return;

        // Automatic forward movement + player left/right control
        float horizontalInput = Input.GetAxis("Horizontal");
        float speed = moveSpeed;

        if (isSprinting)
            speed *= sprintMultiplier;

        // Move right automatically + player can adjust left/right
        float moveX = speed + (horizontalInput * speed * 0.5f);

        rb.linearVelocity = new Vector3(moveX, rb.linearVelocity.y, 0f);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0f);
                isJumping = true;
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, doubleJumpForce, 0f);
                canDoubleJump = false;
            }
        }
    }

    void HandleSlide()
    {
        // Start slide
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && isGrounded && !isSliding)
        {
            StartSlide();
        }

        // Slide timer
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                StopSlide();
            }
        }
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideTime;

        // Shrink collider for sliding under obstacles
        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalColliderHeight * 0.4f;
            capsuleCollider.center = new Vector3(originalColliderCenter.x, originalColliderCenter.y * 0.4f, originalColliderCenter.z);
        }
    }

    void StopSlide()
    {
        isSliding = false;

        // Restore collider
        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalColliderHeight;
            capsuleCollider.center = originalColliderCenter;
        }
    }

    void HandleSprint()
    {
        // Cooldown timer
        if (sprintCooldownTimer > 0)
        {
            sprintCooldownTimer -= Time.deltaTime;
        }

        // Start sprint
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprinting && sprintCooldownTimer <= 0)
        {
            isSprinting = true;
            sprintTimer = sprintDuration;
        }

        // Sprint timer
        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer <= 0)
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

            // Detect enemies in range
            if (attackPoint != null)
            {
                Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
                foreach (Collider enemy in hitEnemies)
                {
                    EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
                    if (enemyScript != null)
                    {
                        enemyScript.TakeDamage(1);
                    }
                }

                // Detect breakable obstacles
                Collider[] hitBreakables = Physics.OverlapSphere(attackPoint.position, attackRange, breakableLayer);
                foreach (Collider breakable in hitBreakables)
                {
                    Destroy(breakable.gameObject);
                }
            }

            // Reset punch animation after short delay
            Invoke("ResetPunch", 0.3f);
        }
    }

    void ResetPunch()
    {
        isPunching = false;
    }

    public void TakeDamage()
    {
        if (isDead)
            return;

        isHurt = true;
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

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;
    }

    public void Respawn(Vector3 respawnPosition)
    {
        isDead = false;
        isHurt = false;
        rb.useGravity = true;
        transform.position = respawnPosition;
        rb.linearVelocity = Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        // Show ground check radius
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Show attack range
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}