using UnityEngine;
using TMPro;

public class RunnerHUD : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;

    public GameObject gameOverPanel;
    public GameObject victoryPanel;          // ✅ اسحب الـ Victory Panel هنا

    public float gameOverDelay = 1.8f;
    public float victoryDelay = 0.5f;        // ✅ تأخير قبل ما تظهر الفيكتوري
    public bool freezeAfterPanel = true;

    private bool gameOverStarted = false;
    private bool victoryStarted = false;     // ✅

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);   // ✅
    }

    void Update()
    {
        if (RunnerGameManager.instance == null) return;

        if (scoreText != null)
            scoreText.text = "Score: " + ScoreManager.Instance.currentPearls;

        if (livesText != null)
            livesText.text = "Lives: " + RunnerGameManager.instance.currentLives;

        if (timerText != null)
        {
            float t = RunnerGameManager.instance.timeElapsed;
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);
            timerText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Game Over
        if (gameOverPanel != null && RunnerGameManager.instance.isGameOver && !gameOverStarted)
        {
            gameOverStarted = true;
            Invoke(nameof(ShowGameOverPanel), gameOverDelay);
        }

        // ✅ Victory
        if (victoryPanel != null && RunnerGameManager.instance.isLevelComplete && !victoryStarted)
        {
            victoryStarted = true;
            Invoke(nameof(ShowVictoryPanel), victoryDelay);
        }
    }

    void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (freezeAfterPanel && RunnerGameManager.instance != null)
            RunnerGameManager.instance.FreezeGameAfterDeath();
    }

    // ✅
    void ShowVictoryPanel()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (RunnerGameManager.instance != null)
            RunnerGameManager.instance.FreezeGameAfterDeath();
    }
}