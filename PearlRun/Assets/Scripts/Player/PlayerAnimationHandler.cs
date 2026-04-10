using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (animator == null || playerController == null)
            return;

        animator.SetFloat("speed", playerController.currentSpeed);
        animator.SetBool("isJumping", playerController.isJumping);
        animator.SetBool("isSliding", playerController.IsSliding);
        animator.SetBool("isPunching", playerController.isPunching);
        animator.SetBool("isHurt", playerController.isHurt);
        animator.SetBool("isDead", playerController.isDead);
    }
}