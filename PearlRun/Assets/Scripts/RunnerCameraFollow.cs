using UnityEngine;

public class RunnerCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Side View Offset")]
    public Vector3 offset = new Vector3(-8f, 3f, 0f);

    [Header("Follow Smoothness")]
    public float followSpeed = 6f;

    [Header("Look Settings")]
    public bool lookAtTarget = true;
    public Vector3 lookOffset = new Vector3(0f, 1.2f, 0f);

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            target.position.z + offset.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        if (lookAtTarget)
        {
            transform.LookAt(target.position + lookOffset);
        }
    }
}