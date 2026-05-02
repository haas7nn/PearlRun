using UnityEngine;
using TMPro;

public class RunnerHUD : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
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
            scoreText.text = "Score: " + RunnerGameManager.instance.score;

        if (livesText != null)
            livesText.text = "Lives: " + RunnerGameManager.instance.currentLives;

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