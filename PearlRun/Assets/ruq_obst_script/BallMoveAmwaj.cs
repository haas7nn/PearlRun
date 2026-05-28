using UnityEngine;

public class BallMoveAmwaj : MonoBehaviour
{
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        // Save the starting position
        startPos = transform.position;
    }

    void Update()
    {
        // Move the ball forward and backward
        float movement = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        transform.position = new Vector3(
            startPos.x,
            startPos.y,
            startPos.z + movement
        );
    }
}