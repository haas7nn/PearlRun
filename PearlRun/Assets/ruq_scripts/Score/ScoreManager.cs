using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Current Level Data")]
    public int currentLevel = 1;
    public int currentPearls = 0;
    public float elapsedTime = 0f;
    public bool levelCompleted = false;

    [Header("Pearl Settings")]
    public int maxPearlsInLevel = 20;

    [Header("Grade Time Settings")]
    public float sTime = 60f;
    public float aTime = 90f;
    public float bTime = 120f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (!levelCompleted)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void AddPearls(int amount)
    {
        currentPearls += amount;
        Debug.Log("Pearls Collected: " + currentPearls);
    }

    public string CalculateGrade()
    {
        float pearlPercent = 0f;

        if (maxPearlsInLevel > 0)
            pearlPercent = (float)currentPearls / maxPearlsInLevel;

        if (pearlPercent >= 0.8f && elapsedTime <= sTime)
            return "S";
        else if (pearlPercent >= 0.6f && elapsedTime <= aTime)
            return "A";
        else if (pearlPercent >= 0.4f && elapsedTime <= bTime)
            return "B";
        else
            return "C";
    }

    public void CompleteLevel(int livesRemaining)
    {
        if (levelCompleted) return;

        levelCompleted = true;

        string finalGrade = CalculateGrade();

        Debug.Log(" LEVEL COMPLETE ");
        Debug.Log("Pearls Collected: " + currentPearls);
        Debug.Log("Time Taken: " + elapsedTime.ToString("F2"));
        Debug.Log("Lives Remaining: " + livesRemaining);
        Debug.Log("Grade: " + finalGrade);

        SaveSystem.SaveBestScore(currentLevel, currentPearls);
        SaveSystem.SaveBestTime(currentLevel, elapsedTime);
        SaveSystem.SaveBestGrade(currentLevel, finalGrade);
        SaveSystem.SaveLevelCompleted(currentLevel);
        SaveSystem.UnlockLevel(currentLevel + 1);

        Debug.Log("Save data updated for Level " + currentLevel);
    }

    public void ResetLevelData()
    {
        currentPearls = 0;
        elapsedTime = 0f;
        levelCompleted = false;
    }
}