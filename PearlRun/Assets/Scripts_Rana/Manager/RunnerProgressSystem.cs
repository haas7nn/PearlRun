using UnityEngine;

public static class RunnerProgressSystem
{
    private const string KEY_X = "RunnerCheckpoint_X";
    private const string KEY_Y = "RunnerCheckpoint_Y";
    private const string KEY_Z = "RunnerCheckpoint_Z";
    private const string KEY_HAS = "RunnerCheckpoint_HasSave";

    public static void SaveCheckpoint(Vector3 position)
    {
        PlayerPrefs.SetFloat(KEY_X, position.x);
        PlayerPrefs.SetFloat(KEY_Y, position.y);
        PlayerPrefs.SetFloat(KEY_Z, position.z);
        PlayerPrefs.SetInt(KEY_HAS, 1);
        PlayerPrefs.Save();
        Debug.Log("RunnerProgressSystem: Saved checkpoint at " + position);
    }

    public static Vector3 LoadCheckpoint()
    {
        if (PlayerPrefs.GetInt(KEY_HAS, 0) == 0)
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
        Debug.Log("RunnerProgressSystem: Cleared checkpoint.");
    }
}