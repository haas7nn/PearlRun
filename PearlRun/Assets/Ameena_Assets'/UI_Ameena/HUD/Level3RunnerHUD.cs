using UnityEngine;
using TMPro;

public class Level3RunnerHUD : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    //public TextMeshProUGUI timerText;

    //private float hudTime = 0f;

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
            int pearls = Level3ScoreManager.Instance != null
        ? Level3ScoreManager.Instance.currentPearls
        : 0;

            scoreText.text = "Score " + pearls;
        }

        if (livesText != null)
        {
            livesText.text = "Lives: " + gm.currentLives;
        }

        /*if (!gm.isGameOver && !gm.isLevelComplete)
        {
            hudTime += Time.deltaTime;
        }*/

        /*if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(hudTime / 60f);
            int seconds = Mathf.FloorToInt(hudTime % 60f);

            timerText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }*/

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

    public void UpdateScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score " + score;
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