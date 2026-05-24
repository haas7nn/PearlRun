using UnityEngine;
using TMPro;

public class Level3RunnerHUD : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public float gameOverDelay = 1.8f;
    public bool freezeAfterPanel = true;

    private bool gameOverStarted = false;

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Level3RunnerGameManager.instance == null)
            return;

        Level3RunnerGameManager gm = Level3RunnerGameManager.instance;

        if (scoreText != null)
        {
            scoreText.text = "Score: " + gm.pearlsCollected;
        }

        if (livesText != null)
        {
            livesText.text = "Lives: " + gm.currentLives;
        }

        if (timerText != null)
        {
            float time = gm.timeElapsed;

            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);

            timerText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (gameOverPanel != null && gm.isGameOver && !gameOverStarted)
        {
            gameOverStarted = true;
            Invoke(nameof(ShowGameOverPanel), gameOverDelay);
        }

        if (gm.isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            gm.RestartLevel();
        }

        if (gm.isGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            gm.LoadMainMenu();
        }
    }

    void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (freezeAfterPanel && Level3RunnerGameManager.instance != null)
        {
            Level3RunnerGameManager.instance.FreezeGameAfterDeath();
        }
    }
}