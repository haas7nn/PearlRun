using UnityEngine;
using System.Collections;

public class Level3AnimationHandler : MonoBehaviour
{
    private Animator animator;
    private Level3PlayerController playerController;
    private bool isPlayingRollFall;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerController = GetComponent<Level3PlayerController>();

        if (animator == null)
        {
            Debug.LogWarning("Level3AnimationHandler: Animator was not found in children.");
        }

        if (playerController == null)
        {
            Debug.LogWarning("Level3AnimationHandler: Level3PlayerController was not found on this GameObject.");
        }
    }

    void Update()
    {
        if (animator == null || playerController == null)
            return;

        bool dead = playerController.isDead;
        bool hurt = playerController.isHurt && !dead;

        bool rollFall = isPlayingRollFall && !dead && !hurt;

        bool slide =
            playerController.IsSliding &&
            playerController.IsGrounded &&
            !dead &&
            !hurt &&
            !rollFall;

        bool punch =
            playerController.isPunching &&
            playerController.IsGrounded &&
            !dead &&
            !hurt &&
            !slide &&
            !rollFall;

        bool doubleJump =
            playerController.isDoubleJumping &&
            !playerController.IsGrounded &&
            !dead &&
            !hurt &&
            !slide &&
            !punch &&
            !rollFall;

        bool jump =
            playerController.isJumping &&
            !playerController.IsGrounded &&
            !doubleJump &&
            !dead &&
            !hurt &&
            !slide &&
            !punch &&
            !rollFall;

        bool runBack =
            playerController.isRunningBackward &&
            playerController.IsGrounded &&
            !dead &&
            !hurt &&
            !slide &&
            !punch &&
            !jump &&
            !doubleJump &&
            !rollFall;

        animator.SetFloat("speed", playerController.currentSpeed, 0.08f, Time.deltaTime);

        animator.SetBool("isDead", dead);
        animator.SetBool("isHurt", hurt);
        animator.SetBool("isSliding", slide);
        animator.SetBool("isPunching", punch);
        animator.SetBool("isJumping", jump);
        animator.SetBool("isDoubleJumping", doubleJump);
        animator.SetBool("isRunningBackward", runBack);
        animator.SetBool("isRollFall", rollFall);

        if (playerController.ConsumeRollFallTrigger() && !isPlayingRollFall && !dead && !hurt)
        {
            StartCoroutine(PlayRollFall());
        }
    }

    IEnumerator PlayRollFall()
    {
        isPlayingRollFall = true;

        animator.SetBool("isJumping", false);
        animator.SetBool("isDoubleJumping", false);
        animator.SetBool("isSliding", false);
        animator.SetBool("isPunching", false);
        animator.SetBool("isRunningBackward", false);
        animator.SetBool("isRollFall", true);

        yield return new WaitForSeconds(0.45f);

        animator.SetBool("isRollFall", false);
        isPlayingRollFall = false;
    }
}