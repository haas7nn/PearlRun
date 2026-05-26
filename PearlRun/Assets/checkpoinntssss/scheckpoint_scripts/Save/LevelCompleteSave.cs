using UnityEngine;

public class LevelCompleteSave : MonoBehaviour
{
    public int levelNumber = 1;

    public void CompleteLevel()
    {
        int score = 100;
        float timeTaken = 45f;
        string grade = "A";

        SaveSystem.SaveBestScore(levelNumber, score);
        SaveSystem.SaveBestTime(levelNumber, timeTaken);
        SaveSystem.SaveBestGrade(levelNumber, grade);

        SaveSystem.SaveLevelCompleted(levelNumber);
        SaveSystem.UnlockLevel(levelNumber + 1);

        CheckpointSystem.ClearCheckpoint();

        Debug.Log("Level complete data saved.");
    }
}