using UnityEngine;

public static class Level3ProgressSystem
{
    private const string KEY_X = "Level3Checkpoint_X";
    private const string KEY_Y = "Level3Checkpoint_Y";
    private const string KEY_Z = "Level3Checkpoint_Z";
    private const string KEY_HAS = "Level3Checkpoint_HasSave";

    /// <summary>
    /// Saves a checkpoint position to PlayerPrefs.
    /// </summary>
    public static void SaveCheckpoint(Vector3 position)
    {
        PlayerPrefs.SetFloat(KEY_X, position.x);
        PlayerPrefs.SetFloat(KEY_Y, position.y);
        PlayerPrefs.SetFloat(KEY_Z, position.z);
        PlayerPrefs.SetInt(KEY_HAS, 1);

        PlayerPrefs.Save();

        Debug.Log("Level3ProgressSystem: Saved checkpoint at " + position);
    }

    /// <summary>
    /// Returns the saved checkpoint position, or Vector3.zero if none exists.
    /// </summary>
    public static Vector3 LoadCheckpoint()
    {
        if (!HasCheckpoint())
            return Vector3.zero;

        float x = PlayerPrefs.GetFloat(KEY_X, 0f);
        float y = PlayerPrefs.GetFloat(KEY_Y, 0f);
        float z = PlayerPrefs.GetFloat(KEY_Z, 0f);

        return new Vector3(x, y, z);
    }

    public static bool HasCheckpoint()
    {
        return PlayerPrefs.GetInt(KEY_HAS, 0) == 1;
    }

    public static void ClearCheckpoint()
    {
        PlayerPrefs.DeleteKey(KEY_X);
        PlayerPrefs.DeleteKey(KEY_Y);
        PlayerPrefs.DeleteKey(KEY_Z);
        PlayerPrefs.DeleteKey(KEY_HAS);

        PlayerPrefs.Save();

        Debug.Log("Level3ProgressSystem: Cleared checkpoint.");
    }
}