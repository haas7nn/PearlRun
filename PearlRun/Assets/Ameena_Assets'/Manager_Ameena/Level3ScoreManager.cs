using UnityEngine;
using TMPro;

public class Level3ScoreManager : MonoBehaviour
{
    public static Level3ScoreManager Instance;

    [Header("Level 3 Data")]
    public int currentPearls = 0;
    public int maxPearlsInLevel = 144;
    public float elapsedTime = 0f;
    public bool levelCompleted = false;

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
            elapsedTime += Time.deltaTime;
    }

    public void AddPearls(int amount)
    {
        currentPearls += amount;
        Debug.Log("Level 3 Pearls Collected: " + currentPearls);
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
        if (levelCompleted)
            return;

        levelCompleted = true;

        string finalGrade = CalculateGrade();

        Debug.Log("LEVEL 3 COMPLETE");
        Debug.Log("Pearls Collected: " + currentPearls);
        Debug.Log("Time Taken: " + elapsedTime.ToString("F2"));
        Debug.Log("Lives Remaining: " + livesRemaining);
        Debug.Log("Grade: " + finalGrade);
    }

    public void ResetLevelData()
    {
        currentPearls = 0;
        elapsedTime = 0f;
        levelCompleted = false;
    }
}