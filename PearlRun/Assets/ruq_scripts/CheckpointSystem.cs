using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public static Vector3 lastCheckpointPosition;

    public static void SaveCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        Debug.Log("Checkpoint position saved: " + lastCheckpointPosition);
    }
}