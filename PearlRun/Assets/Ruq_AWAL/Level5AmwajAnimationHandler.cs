using UnityEngine;
using System.Collections;

public class Level5AmwajAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private Level5AmwajPlayerController playerController;

    private bool isPlayingRollFallAmwaj;
    private bool deathCoroutineStartedAmwaj;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerController = GetComponent<Level5AmwajPlayerController>();

        if (animator == null)
            Debug.LogWarning("Level5AmwajAnimationHandler: Animator was not found in children.");

        if (playerController == null)
            Debug.LogWarning("Level5AmwajAnimationHandler: Level5AmwajPlayerController was not found on this GameObject.");
    }

    void Update()
    {
        if (animator == null || playerController == null)
            return;

        if (!animator.enabled)
            return;

        bool dead = playerController.isDead;
        bool hurt = playerController.isHurt && !dead;
        bool rollFall = isPlayingRollFallAmwaj && !dead && !hurt;

        bool slide =
            playerController.IsSliding &&
            playerController.IsGrounded &&
            !dead && !hurt && !rollFall;

        bool punch =
            playerController.isPunching &&
            playerController.IsGrounded &&
            !dead && !hurt && !slide && !rollFall;

        bool doubleJump =
            playerController.isDoubleJumping &&
            !playerController.IsGrounded &&
            !dead && !hurt && !slide && !punch && !rollFall;

        bool jump =
            playerController.isJumping &&
            !playerController.IsGrounded &&
            !doubleJump &&
            !dead && !hurt && !slide && !punch && !rollFall;

        bool runBack =
            playerController.isRunningBackward &&
            playerController.IsGrounded &&
            !dead && !hurt && !slide && !punch &&
            !jump && !doubleJump && !rollFall;

        if (playerController.IsGrounded && !dead && !hurt && !rollFall)
        {
            jump = false;
            doubleJump = false;
        }

        animator.SetFloat("speed", playerController.currentSpeed, 0.08f, Time.deltaTime);

        animator.SetBool("isDead", dead);
        animator.SetBool("isHurt", hurt);
        animator.SetBool("isSliding", slide);
        animator.SetBool("isPunching", punch);
        animator.SetBool("isJumping", jump);
        animator.SetBool("isDoubleJumping", doubleJump);
        animator.SetBool("isRunningBackward", runBack);
        animator.SetBool("isRollFall", rollFall);

        if (dead && !deathCoroutineStartedAmwaj)
        {
            deathCoroutineStartedAmwaj = true;
            StartCoroutine(StopAnimatorAfterDeathAmwaj());
        }

        if (playerController.ConsumeRollFallTrigger() && !isPlayingRollFallAmwaj && !dead && !hurt)
        {
            StartCoroutine(PlayRollFallAmwaj());
        }
    }

    IEnumerator StopAnimatorAfterDeathAmwaj()
    {
        yield return new WaitForSeconds(1.5f);

        if (playerController != null && playerController.isDead && animator != null)
        {
            animator.enabled = false;
        }
    }

    public void EnableAnimatorAmwaj()
    {
        if (animator != null)
        {
            animator.enabled = true;

            animator.Rebind();
            animator.Update(0f);

            animator.SetBool("isDead", false);
            animator.SetBool("isHurt", false);
            animator.SetBool("isSliding", false);
            animator.SetBool("isPunching", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isDoubleJumping", false);
            animator.SetBool("isRunningBackward", false);
            animator.SetBool("isRollFall", false);
        }

        deathCoroutineStartedAmwaj = false;
        isPlayingRollFallAmwaj = false;
    }

    IEnumerator PlayRollFallAmwaj()
    {
        isPlayingRollFallAmwaj = true;

        animator.SetBool("isJumping", false);
        animator.SetBool("isDoubleJumping", false);
        animator.SetBool("isSliding", false);
        animator.SetBool("isPunching", false);
        animator.SetBool("isRunningBackward", false);
        animator.SetBool("isRollFall", true);

        yield return new WaitForSeconds(0.45f);

        animator.SetBool("isRollFall", false);
        isPlayingRollFallAmwaj = false;
    }
}