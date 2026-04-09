using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static void SaveBestScore(int level, int newScore)
    {
        int currentBest = PlayerPrefs.GetInt(SaveKeys.BestScore(level), 0);

        if (newScore > currentBest)
        {
            PlayerPrefs.SetInt(SaveKeys.BestScore(level), newScore);
            PlayerPrefs.Save();
            Debug.Log("New best score saved for Level " + level + ": " + newScore);
        }
    }

    public static int LoadBestScore(int level)
    {
        return PlayerPrefs.GetInt(SaveKeys.BestScore(level), 0);
    }

    public static void SaveBestTime(int level, float newTime)
    {
        float currentBest = PlayerPrefs.GetFloat(SaveKeys.BestTime(level), -1f);

        if (currentBest < 0f || newTime < currentBest)
        {
            PlayerPrefs.SetFloat(SaveKeys.BestTime(level), newTime);
            PlayerPrefs.Save();
            Debug.Log("New best time saved for Level " + level + ": " + newTime);
        }
    }

    public static float LoadBestTime(int level)
    {
        return PlayerPrefs.GetFloat(SaveKeys.BestTime(level), -1f);
    }

    public static void SaveBestGrade(int level, string newGrade)
    {
        string currentGrade = PlayerPrefs.GetString(SaveKeys.BestGrade(level), "");

        if (IsNewGradeBetter(newGrade, currentGrade))
        {
            PlayerPrefs.SetString(SaveKeys.BestGrade(level), newGrade);
            PlayerPrefs.Save();
            Debug.Log("New best grade saved for Level " + level + ": " + newGrade);
        }
    }

    public static string LoadBestGrade(int level)
    {
        return PlayerPrefs.GetString(SaveKeys.BestGrade(level), "None");
    }

    public static void SaveLevelCompleted(int level)
    {
        PlayerPrefs.SetInt(SaveKeys.LevelCompleted(level), 1);
        PlayerPrefs.Save();
        Debug.Log("Level " + level + " marked as completed.");
    }

    public static bool IsLevelCompleted(int level)
    {
        return PlayerPrefs.GetInt(SaveKeys.LevelCompleted(level), 0) == 1;
    }

    public static void UnlockLevel(int level)
    {
        PlayerPrefs.SetInt(SaveKeys.LevelUnlocked(level), 1);
        PlayerPrefs.Save();
        Debug.Log("Level " + level + " unlocked.");
    }

    public static bool IsLevelUnlocked(int level)
    {
        if (level == 1) return true;
        return PlayerPrefs.GetInt(SaveKeys.LevelUnlocked(level), 0) == 1;
    }

    public static LevelData LoadLevelData(int level)
    {
        LevelData data = new LevelData();
        data.bestScore = LoadBestScore(level);
        data.bestTime = LoadBestTime(level);
        data.bestGrade = LoadBestGrade(level);
        data.isUnlocked = IsLevelUnlocked(level);
        data.isCompleted = IsLevelCompleted(level);
        return data;
    }

    public static void ResetAllSaves()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All save data deleted.");
    }

    private static bool IsNewGradeBetter(string newGrade, string currentGrade)
    {
        return GradeValue(newGrade) > GradeValue(currentGrade);
    }

    private static int GradeValue(string grade)
    {
        switch (grade)
        {
            case "S": return 5;
            case "A": return 4;
            case "B": return 3;
            case "C": return 2;
            case "D": return 1;
            default: return 0;
        }
    }
}