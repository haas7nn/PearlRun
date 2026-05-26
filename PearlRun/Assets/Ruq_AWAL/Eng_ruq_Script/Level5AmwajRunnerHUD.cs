using UnityEngine;
using TMPro;

public class Level5AmwajRunnerHUD : MonoBehaviour
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
        if (Level5AmwajRunnerGameManager.instance == null)
            return;

        Level5AmwajRunnerGameManager gm = Level5AmwajRunnerGameManager.instance;

        if (scoreText != null)
        {
            int pearls = Level5AmwajScoreManager.Instance != null
                ? Level5AmwajScoreManager.Instance.currentPearls
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

        if (freezeAfterPanel && Level5AmwajRunnerGameManager.instance != null)
        {
            Level5AmwajRunnerGameManager.instance.FreezeGameAfterDeath();
        }
    }
}