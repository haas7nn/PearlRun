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

    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isPunching;
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead;
    [HideInInspector] public float currentSpeed;

    public bool IsSliding => isSliding;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        Debug.Log("=== RunnerController Start ===");

        if (rb == null)
        {
            Debug.LogError("Rigidbody NOT FOUND on this GameObject!");
        }
        else
        {
            Debug.Log("Rigidbody found: " + rb.name);
        }

        if (capsuleCollider == null)
        {
            Debug.LogError("CapsuleCollider NOT FOUND on this GameObject!");
        }
        else
        {
            Debug.Log("CapsuleCollider found: " + capsuleCollider.name);
            originalColliderHeight = capsuleCollider.height;
            originalColliderCenter = capsuleCollider.center;
        }

        if (groundCheck == null)
        {
            Debug.LogWarning("groundCheck is NOT assigned!");
        }
        else
        {
            Debug.Log("groundCheck assigned: " + groundCheck.name);
        }

        if (attackPoint == null)
        {
            Debug.LogWarning("attackPoint is NOT assigned!");
        }
        else
        {
            Debug.Log("attackPoint assigned: " + attackPoint.name);
        }

        originalScale = transform.localScale;
        sprintCooldownTimer = 0f;

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionZ |
                             RigidbodyConstraints.FreezeRotationX |
                             RigidbodyConstraints.FreezeRotationY |
                             RigidbodyConstraints.FreezeRotationZ;

            Debug.Log("Rigidbody constraints set successfully.");
        }
    }

    void Update()
    {
        Debug.Log("RunnerController Update");

        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log("Space detected");

        if (Input.GetKeyDown(KeyCode.LeftShift))
            Debug.Log("LeftShift detected");

        if (Input.GetKeyDown(KeyCode.F))
            Debug.Log("F detected");

        if (Input.GetMouseButtonDown(0))
            Debug.Log("Mouse0 detected");

        if (GameManager.instance != null && GameManager.instance.isGameOver)
        {
            Debug.Log("Stopped: GameManager says isGameOver = true");
            return;
        }

        if (isDead)
        {
            Debug.Log("Stopped: isDead = true");
            return;
        }

        CheckGround();
        HandleJump();
        HandleSlide();
        HandleSprint();
        HandleAttack();

        if (rb != null)
        {
            currentSpeed = Mathf.Abs(rb.linearVelocity.x);
        }
        else
        {
            Debug.LogError("rb is NULL inside Update!");
        }
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
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
        }

        Debug.Log("isGrounded = " + isGrounded);

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

        float horizontalInput = Input.GetAxis("Horizontal");
        float speed = moveSpeed;

        if (isSprinting)
            speed *= sprintMultiplier;

        float moveX = speed + (horizontalInput * speed * 0.5f);

        if (rb != null)
        {
            rb.linearVelocity = new Vector3(moveX, rb.linearVelocity.y, 0f);
        }
        else
        {
            Debug.LogError("Cannot move: rb is NULL");
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump key pressed");
            Debug.Log("isGrounded: " + isGrounded);
            Debug.Log("canDoubleJump: " + canDoubleJump);

            if (rb == null)
            {
                Debug.LogError("Cannot jump: rb is NULL");
                return;
            }

            if (isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0f);
                isJumping = true;
                canDoubleJump = true;
                Debug.Log("Ground jump executed");
            }
            else if (canDoubleJump)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, doubleJumpForce, 0f);
                canDoubleJump = false;
                Debug.Log("Double jump executed");
            }
            else
            {
                Debug.Log("Jump blocked: not grounded and no double jump available");
            }
        }
    }

    void HandleSlide()
    {
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && isGrounded && !isSliding)
        {
            Debug.Log("Slide started");
            StartSlide();
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                Debug.Log("Slide stopped");
                StopSlide();
            }
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
        else
        {
            Debug.LogWarning("Cannot resize collider while sliding: CapsuleCollider is NULL");
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
        {
            sprintCooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprinting && sprintCooldownTimer <= 0)
        {
            isSprinting = true;
            sprintTimer = sprintDuration;
            Debug.Log("Sprint started");
        }

        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer <= 0)
            {
                isSprinting = false;
                sprintCooldownTimer = sprintCooldown;
                Debug.Log("Sprint ended, cooldown started");
            }
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
        {
            Debug.Log("Attack triggered");
            isPunching = true;

            if (attackPoint != null)
            {
                Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
                Debug.Log("Enemies hit: " + hitEnemies.Length);

                foreach (Collider enemy in hitEnemies)
                {
                    EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
                    if (enemyScript != null)
                    {
                        enemyScript.TakeDamage(1);
                    }
                }

                Collider[] hitBreakables = Physics.OverlapSphere(attackPoint.position, attackRange, breakableLayer);
                Debug.Log("Breakables hit: " + hitBreakables.Length);

                foreach (Collider breakable in hitBreakables)
                {
                    Destroy(breakable.gameObject);
                }
            }
            else
            {
                Debug.LogWarning("Attack triggered but attackPoint is NULL");
            }

            Invoke(nameof(ResetPunch), 0.3f);
        }
    }

    void ResetPunch()
    {
        isPunching = false;
        Debug.Log("Punch reset");
    }

    public void TakeDamage()
    {
        if (isDead)
            return;

        isHurt = true;
        Debug.Log("Player took damage");
        Invoke(nameof(ResetHurt), 0.5f);

        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerHit();
        }
    }

    void ResetHurt()
    {
        isHurt = false;
        Debug.Log("Hurt reset");
    }

    public void Die()
    {
        isDead = true;
        Debug.Log("Player died");

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.useGravity = false;
        }
    }

    public void Respawn(Vector3 respawnPosition)
    {
        isDead = false;
        isHurt = false;
        Debug.Log("Player respawned at: " + respawnPosition);

        if (rb != null)
        {
            rb.useGravity = true;
            transform.position = respawnPosition;
            rb.linearVelocity = Vector3.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}