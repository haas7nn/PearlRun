using UnityEngine;
using TMPro;

public class RunnerHUD : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;      // ← اسحب هنا الـ Text الجديد
    public GameObject gameOverPanel;
    public float gameOverDelay = 1.8f;
    public bool freezeAfterPanel = true;

    private bool gameOverStarted = false;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (RunnerGameManager.instance == null)
            return;

        if (scoreText != null)
            scoreText.text = "Score: " + ScoreManager.Instance.currentPearls;

        if (livesText != null)
            livesText.text = "Lives: " + RunnerGameManager.instance.currentLives;

        // التايمر
        if (timerText != null)
        {
            float t = RunnerGameManager.instance.timeElapsed;
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);
            timerText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (gameOverPanel != null && RunnerGameManager.instance.isGameOver && !gameOverStarted)
        {
            gameOverStarted = true;
            Invoke(nameof(ShowGameOverPanel), gameOverDelay);
        }

        if (RunnerGameManager.instance.isGameOver && Input.GetKeyDown(KeyCode.R))
            RunnerGameManager.instance.RestartLevel();

        if (RunnerGameManager.instance.isGameOver && Input.GetKeyDown(KeyCode.Escape))
            RunnerGameManager.instance.LoadMainMenu();
    }

    void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (freezeAfterPanel && RunnerGameManager.instance != null)
            RunnerGameManager.instance.FreezeGameAfterDeath();
    }
}