using System.Collections;
using UnityEngine;

public class RunnerAirDamageUnstuck : MonoBehaviour
{
    private RunnerController runner;
    private Animator anim;
    private Rigidbody rb;

    [Header("Animator State Names")]
    public string rollFallState = "RollFall";
    public string runState = "RunForward";

    [Header("Timing")]
    public float rollTime = 0.45f;

    private bool wasDamagedInAir;
    private bool fixing;

    void Start()
    {
        runner = GetComponent<RunnerController>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (runner == null || anim == null) return;
        if (runner.isDead) return;

        if (runner.isHurt && !runner.IsGrounded)
            wasDamagedInAir = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!wasDamagedInAir) return;
        if (fixing) return;

        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("JumpObstacle") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            StartCoroutine(FixAnimationNow());
        }
    }

    IEnumerator FixAnimationNow()
    {
        fixing = true;
        wasDamagedInAir = false;

        if (rb != null)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        anim.SetBool("isHurt", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("isDoubleJumping", false);
        anim.SetBool("isSliding", false);
        anim.SetBool("isPunching", false);
        anim.SetBool("isRunningBackward", false);

        anim.SetBool("isRollFall", true);
        anim.Play(rollFallState, 0, 0f);

        yield return new WaitForSeconds(rollTime);

        anim.SetBool("isRollFall", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("isDoubleJumping", false);
        anim.SetBool("isHurt", false);

        anim.Play(runState, 0, 0f);

        fixing = false;
    }
}