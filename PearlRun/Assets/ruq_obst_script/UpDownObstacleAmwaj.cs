using UnityEngine;

public class UpDownObstacleAmwaj : MonoBehaviour
{
    public float moveDistance = 2f;
    public float moveSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        // Save the starting position
        startPos = transform.position;
    }

    void Update()
    {
        // Move the obstacle up and down
        float newY = startPos.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }
}