using UnityEngine;

public class SideMoveObstacleAmwaj : MonoBehaviour
{
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        // Save starting position
        startPos = transform.position;
    }

    void Update()
    {
        // Move left and right
        float newX = startPos.x + Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        transform.position = new Vector3(
            newX,
            transform.position.y,
            transform.position.z
        );
    }
}