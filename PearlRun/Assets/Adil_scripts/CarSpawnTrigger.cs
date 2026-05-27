using UnityEngine;

public class CarSpawnTrigger : MonoBehaviour
{
    public GameObject carPrefab;
    public Transform player;

    [Header("Directions")]
    public Vector3 forwardDirection = Vector3.right; // set this to the direction Awal runs
    public bool spawnInFrontAndComeTowardsPlayer = true;

    [Header("Spawn")]
    public float spawnDistance = 35f; // increase until it's off-camera
    public float spawnHeightOffset = 0f;

    [Header("Car")]
    public float carSpeed = 25f;
    public float carYaw = -90f; // you asked for this

    bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;
        SpawnCar();
    }

    void SpawnCar()
    {
        Vector3 fwd = forwardDirection.normalized;

        // Spawn point relative to player
        Vector3 spawnPos = player.position + (spawnInFrontAndComeTowardsPlayer ? fwd : -fwd) * spawnDistance;
        spawnPos.y = player.position.y + spawnHeightOffset;

        // Move direction (toward player if spawned in front)
        Vector3 moveDir = spawnInFrontAndComeTowardsPlayer ? -fwd : fwd;

        // Spawn with requested rotation
        Quaternion rot = Quaternion.Euler(0f, carYaw, 0f);

        GameObject car = Instantiate(carPrefab, spawnPos, rot);

        var mover = car.GetComponent<CarMover>();
        if (mover != null)
        {
            mover.speed = carSpeed;
            mover.moveDirection = moveDir;
        }
    }
}