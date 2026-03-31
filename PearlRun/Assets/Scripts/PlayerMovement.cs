using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Forward Auto Run")]
    public float baseForwardSpeed = 5f;
    public float sideSpeed = 4f;

    [Header("Side Limits")]
    public float minZ = -2f;
    public float maxZ = 2f;

    [Header("Jump")]
    public float jumpHeight = 2f;
    public float gravity = -20f;
    public int maxJumps = 2;

    [Header("Sprint")]
    public float sprintMultiplier = 1.8f;
    public float sprintDuration = 2f;
    public float sprintCooldown = 8f;

    [Header("Slide")]
    public float slideDuration = 0.8f;
    public float slideHeight = 1f;

    [Header("Lives")]
    public int maxLives = 2;
    private int currentLives;

    private CharacterController controller;
    private Animator anim;
    private Vector3 velocity;

    private float sideInput;
    private bool jumpPressed;
    private bool slidePressed;
    private bool sprintPressed;

    private int jumpCount;
    private bool isSliding;
    private bool isSprinting;
    private bool isGameFinished = false;
    private bool isDead = false;

    private float sprintTimer;
    private float sprintCooldownTimer;
    private float slideTimer;

    private float normalControllerHeight;
    private Vector3 normalControllerCenter;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        normalControllerHeight = controller.height;
        normalControllerCenter = controller.center;

        currentLives = maxLives;
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
            jumpCount = 0;
        }

        // مؤقتًا للاختبار
        if (Keyboard.current != null)
        {
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                TakeHit();
            }

            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                Die();
            }

            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                isGameFinished = true;
            }
        }

        if (!isDead)
        {
            HandleSprint();
            HandleSlide();
            HandleJump(isGrounded);
        }

        float currentForwardSpeed = baseForwardSpeed;

        if (isSprinting)
            currentForwardSpeed *= sprintMultiplier;

        if (isGameFinished || isDead)
            currentForwardSpeed = 0f;

        Vector3 move = new Vector3(currentForwardSpeed, 0f, sideInput * sideSpeed);
        controller.Move(move * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        transform.position = pos;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        anim.SetFloat("Speed", currentForwardSpeed);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("isGameFinished", isGameFinished);

        jumpPressed = false;
        slidePressed = false;
        sprintPressed = false;
    }

    void HandleJump(bool isGrounded)
    {
        if (!jumpPressed) return;
        if (isDead) return;

        if (isGrounded || jumpCount < maxJumps)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpCount++;

            anim.SetTrigger("Jump");
        }
    }

    void HandleSprint()
    {
        if (sprintCooldownTimer > 0f)
            sprintCooldownTimer -= Time.deltaTime;

        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;

            if (sprintTimer <= 0f)
            {
                isSprinting = false;
                sprintCooldownTimer = sprintCooldown;
            }
        }

        if (sprintPressed && !isSprinting && sprintCooldownTimer <= 0f)
        {
            isSprinting = true;
            sprintTimer = sprintDuration;
        }
    }

    void HandleSlide()
    {
        if (slidePressed && !isSliding)
        {
            isSliding = true;
            slideTimer = slideDuration;

            controller.height = slideHeight;
            controller.center = new Vector3(
                normalControllerCenter.x,
                slideHeight / 2f,
                normalControllerCenter.z
            );
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;

            if (slideTimer <= 0f)
            {
                isSliding = false;
                controller.height = normalControllerHeight;
                controller.center = normalControllerCenter;
            }
        }
    }

    public void TakeHit()
    {
        if (isDead) return;

        currentLives--;

        if (currentLives > 0)
        {
            anim.SetTrigger("GetHit");
        }
        else
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isGameFinished = true;

        isSliding = false;
        isSprinting = false;
        sideInput = 0f;
        velocity = Vector3.zero;

        controller.height = normalControllerHeight;
        controller.center = normalControllerCenter;

        anim.SetTrigger("Death");
    }

    public void RestoreLife(int amount = 1)
    {
        currentLives += amount;
        if (currentLives > maxLives)
            currentLives = maxLives;
    }

    public void FinishGame()
    {
        isGameFinished = true;
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    public void OnMove(InputValue value)
    {
        if (isDead) return;
        sideInput = value.Get<float>();
    }

    public void OnJump(InputValue value)
    {
        if (isDead) return;

        if (value.isPressed)
            jumpPressed = true;
    }

    public void OnSlide(InputValue value)
    {
        if (isDead) return;

        if (value.isPressed)
            slidePressed = true;
    }

    public void OnSprint(InputValue value)
    {
        if (isDead) return;

        if (value.isPressed)
            sprintPressed = true;
    }
}