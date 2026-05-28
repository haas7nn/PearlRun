using UnityEngine;

public class NetSideMoveAmwaj : MonoBehaviour
{
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Move right and left
        float movement = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        transform.position = new Vector3(
            startPos.x + movement,
            startPos.y,
            startPos.z
        );
    }
}