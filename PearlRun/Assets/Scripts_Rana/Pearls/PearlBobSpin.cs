using UnityEngine;

public class PearlBobSpin : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 60f;

    [Header("Floating")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.25f;

    private Vector3 startLocalPos;
    private float timeOffset;

    void Start()
    {
        // Local space → works correctly when parented to a moving boat platform.
        startLocalPos = transform.localPosition;

        // Random phase so a row of pearls doesn't bob in perfect sync.
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // Spin
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Bob (relative to parent)
        float yOffset = Mathf.Sin((Time.time + timeOffset) * floatSpeed) * floatHeight;
        Vector3 lp = transform.localPosition;
        lp.y = startLocalPos.y + yOffset;
        transform.localPosition = lp;
    }
}