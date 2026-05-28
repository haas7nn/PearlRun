using UnityEngine;

public class CarSpawnTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject carPrefab;
    public Transform player;                   // drag Awal here (or leave empty)

    [Header("Spawn relative to player")]
    public Vector3 forwardDirection = Vector3.right; // set to the direction Awal runs (X+ usually)
    public float spawnDistance = 35f;               // how far in front (increase if still visible)
    public float laneOffsetZ = 0f;                  // move car left/right across lane if needed
    public float spawnHeightOffset = 0f;            // usually 0

    [Header("Car")]
    public float carSpeed = 25f;
    public float carYaw = -90f;                     // car facing rotation
    public bool spawnInFrontAndDriveToPlayer = true;

    [Header("Ground Snap")]
    public LayerMask groundLayer;                   // set to Ground in inspector
    public float rayHeight = 5f;
    public float snapOffsetY = 0.05f;

    private bool used;

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.Find("Awal");
            if (p != null) player = p.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;
        if (carPrefab == null) return;
        if (player == null) return;

        used = true;
        SpawnCar();
    }

    void SpawnCar()
    {
        Vector3 fwd = forwardDirection.normalized;

        // X/Z from player, Y NOT from player
        Vector3 spawnPos = player.position + (spawnInFrontAndDriveToPlayer ? fwd : -fwd) * spawnDistance;
        spawnPos.z += laneOffsetZ;

        // Raycast from high above the spawn X/Z so it works even if player is airborne
        Vector3 rayStart = new Vector3(spawnPos.x, 50f, spawnPos.z);

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 200f, groundLayer))
            spawnPos.y = hit.point.y + snapOffsetY;
        else
            spawnPos.y = 0f; // fallback if ground not found

        Vector3 moveDir = spawnInFrontAndDriveToPlayer ? -fwd : fwd;

        Quaternion rot = Quaternion.Euler(0f, carYaw, 0f);
        GameObject car = Instantiate(carPrefab, spawnPos, rot);

        CarMover mover = car.GetComponent<CarMover>();
        if (mover != null)
        {
            mover.moveDirection = moveDir;
            mover.speed = carSpeed;
            mover.active = true;
        }
    }
}