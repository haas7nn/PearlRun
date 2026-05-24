using UnityEngine;

public class BoatMoveForwardAmwaj : MonoBehaviour
{
    // Place where the boat stops
    public Transform targetPoint;

    // Boat movement speed
    public float moveSpeed = 3f;

    private bool canMove = false;

    void Update()
    {
        if (!canMove || targetPoint == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );
    }

    // Called from trigger
    public void StartMoving()
    {
        canMove = true;
    }
}