using UnityEngine;
using System.Collections;

public class RunnerAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private RunnerController runnerController;
    private bool isPlayingRollFall;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        runnerController = GetComponent<RunnerController>();
    }

    void Update()
    {
        if (animator == null || runnerController == null)
            return;

        bool dead = runnerController.isDead;
        bool hurt = runnerController.isHurt && !dead;

        bool rollFall = isPlayingRollFall && !dead && !hurt;

        bool slide =
            runnerController.IsSliding &&
            runnerController.IsGrounded &&
            !dead &&
            !hurt &&
            !rollFall;

        bool punch =
            runnerController.isPunching &&
            runnerController.IsGrounded &&
            !dead &&
            !hurt &&
            !slide &&
            !rollFall;

        bool doubleJump =
            runnerController.isDoubleJumping &&
            !runnerController.IsGrounded &&
            !dead &&
            !hurt &&
            !slide &&
            !punch &&
            !rollFall;

        bool jump =
            runnerController.isJumping &&
            !runnerController.IsGrounded &&
            !doubleJump &&
            !dead &&
            !hurt &&
            !slide &&
            !punch &&
            !rollFall;

        bool runBack =
            runnerController.isRunningBackward &&
            runnerController.IsGrounded &&
            !dead &&
            !hurt &&
            !slide &&
            !punch &&
            !jump &&
            !doubleJump &&
            !rollFall;

        animator.SetFloat("speed", runnerController.currentSpeed, 0.08f, Time.deltaTime);

        animator.SetBool("isDead", dead);
        animator.SetBool("isHurt", hurt);
        animator.SetBool("isSliding", slide);
        animator.SetBool("isPunching", punch);
        animator.SetBool("isJumping", jump);
        animator.SetBool("isDoubleJumping", doubleJump);
        animator.SetBool("isRunningBackward", runBack);
        animator.SetBool("isRollFall", rollFall);

        if (runnerController.ConsumeRollFallTrigger() && !isPlayingRollFall && !dead && !hurt)
            StartCoroutine(PlayRollFall());
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