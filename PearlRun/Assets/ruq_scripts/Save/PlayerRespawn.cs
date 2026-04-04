using UnityEngine;

// Respawn system only.
// Called when the player dies to return them to the last saved checkpoint.

public class PlayerRespawn : MonoBehaviour
{
    public void RespawnPlayer()
    {
        Vector3 checkpointPos = CheckpointSystem.LoadCheckpoint();

        if (checkpointPos != Vector3.zero)
        {
            transform.position = checkpointPos;
            Debug.Log("Player respawned at checkpoint.");
        }
        else
        {
            Debug.Log("No checkpoint found.");
        }
    }
}