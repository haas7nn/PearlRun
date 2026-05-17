using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public static void SaveCheckpoint(Vector3 position)
    {
        PlayerPrefs.SetFloat(SaveKeys.CheckpointX, position.x);
        PlayerPrefs.SetFloat(SaveKeys.CheckpointY, position.y);
        PlayerPrefs.SetFloat(SaveKeys.CheckpointZ, position.z);
        PlayerPrefs.SetInt(SaveKeys.HasCheckpoint, 1);
        PlayerPrefs.Save();

        Debug.Log("Checkpoint position saved: " + position);
    }

    public static Vector3 LoadCheckpoint()
    {
        bool hasCheckpoint = PlayerPrefs.GetInt(SaveKeys.HasCheckpoint, 0) == 1;

        if (!hasCheckpoint)
        {
            return Vector3.zero;
        }

        float x = PlayerPrefs.GetFloat(SaveKeys.CheckpointX, 0f);
        float y = PlayerPrefs.GetFloat(SaveKeys.CheckpointY, 0f);
        float z = PlayerPrefs.GetFloat(SaveKeys.CheckpointZ, 0f);

        return new Vector3(x, y, z);
    }

    public static void ClearCheckpoint()
    {
        PlayerPrefs.DeleteKey(SaveKeys.CheckpointX);
        PlayerPrefs.DeleteKey(SaveKeys.CheckpointY);
        PlayerPrefs.DeleteKey(SaveKeys.CheckpointZ);
        PlayerPrefs.DeleteKey(SaveKeys.HasCheckpoint);
        PlayerPrefs.Save();

        Debug.Log("Checkpoint cleared.");
    }
}