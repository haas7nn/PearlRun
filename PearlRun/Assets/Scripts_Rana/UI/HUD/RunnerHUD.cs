using UnityEngine;
using TMPro;

public class RunnerHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Settings")]
    [SerializeField] private float gameOverDelay = 1.8f;
    [SerializeField] private bool freezeAfterPanel = true;

    private bool gameOverStarted;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (RunnerGameManager.instance == null) return;

        UpdateScore();
        UpdateTimer();
        CheckGameOver();
    }

    void UpdateScore()
    {
        if (scoreText == null || ScoreManager.Instance == null) return;
        scoreText.text = $"Score: {ScoreManager.Instance.currentPearls}";
    }

    void UpdateTimer()
    {
        if (timerText == null) return;

        float t = RunnerGameManager.instance.timeElapsed;
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    void CheckGameOver()
    {
        if (gameOverPanel == null || gameOverStarted) return;
        if (!RunnerGameManager.instance.isGameOver) return;

        gameOverStarted = true;
        Invoke(nameof(ShowGameOverPanel), gameOverDelay);
    }

    void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (freezeAfterPanel && RunnerGameManager.instance != null)
            RunnerGameManager.instance.FreezeGameAfterDeath();
    }

    public void ResetHUD()
    {
        gameOverStarted = false;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}