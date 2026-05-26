using UnityEngine;

public class EnemyPatrol : EnemyBase
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float speed = 5f;
    public float reachDistance = 0.1f;

    [Header("Fixed Position")]
    public bool lockY = true;
    public bool lockZ = true;
    public float fixedY = 0f;
    public float fixedZ = 0f;

    [Header("Direction")]
    public bool faceRightWhenMovingToB = true;

    [Header("Animation")]
    public Animator animator;
    public string runningParameterName = "isRunning";

    [Header("Visual Model")]
    public Transform visualModel;
    public Vector3 visualLocalRotation = new Vector3(0f, 0f, 0f);

    private Transform target;
    private Vector3 startScale;

    private void Start()
    {
        target = pointB;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (visualModel == null && animator != null)
            visualModel = animator.transform;

        startScale = transform.localScale;

        fixedY = transform.position.y;
        fixedZ = transform.position.z;

        SetRunning(true);
        ApplyFixedVisualRotation();
    }

    private void Update()
    {
        if (pointA == null || pointB == null)
        {
            SetRunning(false);
            return;
        }

        MovePatrol();
    }

    private void MovePatrol()
    {
        SetRunning(true);

        Vector3 targetPosition = target.position;

        if (lockY)
            targetPosition.y = fixedY;

        if (lockZ)
            targetPosition.z = fixedZ;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        FixCurrentPosition();
        FaceMoveDirection();

        if (Vector3.Distance(transform.position, targetPosition) <= reachDistance)
        {
            target = target == pointA ? pointB : pointA;
        }
    }

    private void FixCurrentPosition()
    {
        Vector3 pos = transform.position;

        if (lockY)
            pos.y = fixedY;

        if (lockZ)
            pos.z = fixedZ;

        transform.position = pos;
    }

    private void FaceMoveDirection()
    {
        if (target == null)
            return;

        bool movingToB = target == pointB;

        Vector3 scale = startScale;

        if (faceRightWhenMovingToB)
        {
            scale.x = movingToB ? Mathf.Abs(startScale.x) : -Mathf.Abs(startScale.x);
        }
        else
        {
            scale.x = movingToB ? -Mathf.Abs(startScale.x) : Mathf.Abs(startScale.x);
        }

        transform.localScale = scale;
    }

    private void ApplyFixedVisualRotation()
    {
        if (visualModel != null)
        {
            visualModel.localRotation = Quaternion.Euler(visualLocalRotation);
        }
    }

    private void SetRunning(bool value)
    {
        if (animator != null)
        {
            animator.SetBool(runningParameterName, value);
        }
    }
}