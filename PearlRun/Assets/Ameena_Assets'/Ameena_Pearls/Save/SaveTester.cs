using UnityEngine;

public class SaveTester : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== SAVE SYSTEM TEST START ===");

        SaveSystem.SaveBestScore(1, 150);
        SaveSystem.SaveBestTime(1, 42.5f);
        SaveSystem.SaveBestGrade(1, "A");
        SaveSystem.SaveLevelCompleted(1);
        SaveSystem.UnlockLevel(2);

        CheckpointSystem.SaveCheckpoint(new Vector3(5f, 2f, 0f));

        Debug.Log("Best Score: " + SaveSystem.LoadBestScore(1));
        Debug.Log("Best Time: " + SaveSystem.LoadBestTime(1));
        Debug.Log("Best Grade: " + SaveSystem.LoadBestGrade(1));
        Debug.Log("Level 1 Completed: " + SaveSystem.IsLevelCompleted(1));
        Debug.Log("Level 2 Unlocked: " + SaveSystem.IsLevelUnlocked(2));

        Vector3 checkpointPos = CheckpointSystem.LoadCheckpoint();
        Debug.Log("Checkpoint Position: " + checkpointPos);

        Debug.Log("=== SAVE SYSTEM TEST END ===");
    }
}