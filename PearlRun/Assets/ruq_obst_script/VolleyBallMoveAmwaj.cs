using UnityEngine;

public class VolleyBallMoveAmwaj : MonoBehaviour
{
    public float moveDistance = 3f;
    public float moveSpeed = 2f;
    public float arcHeight = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Move the ball from one side to the other
        float t = (Mathf.Sin(Time.time * moveSpeed) + 1f) / 2f;

        float newX = Mathf.Lerp(startPos.x - moveDistance, startPos.x + moveDistance, t);
        float newY = startPos.y + Mathf.Sin(t * Mathf.PI) * arcHeight;

        transform.position = new Vector3(newX, newY, startPos.z);
    }
}