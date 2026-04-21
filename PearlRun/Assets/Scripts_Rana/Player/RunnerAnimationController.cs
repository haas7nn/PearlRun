using UnityEngine;

public class RunnerAnimationController : MonoBehaviour
{
    private Animator animator;
    private RunnerController runnerController;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        runnerController = GetComponent<RunnerController>();
    }

    void Update()
    {
        if (animator == null || runnerController == null)
            return;

        animator.SetFloat("speed", runnerController.currentSpeed);
        animator.SetBool("isJumping", runnerController.isJumping);
        animator.SetBool("isSliding", runnerController.IsSliding);
        animator.SetBool("isPunching", runnerController.isPunching);
        animator.SetBool("isHurt", runnerController.isHurt);
        animator.SetBool("isDead", runnerController.isDead);
    }
}