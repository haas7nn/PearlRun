using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameHUD : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;

    void Update()
    {
        if (GameManager.instance == null)
            return;

        // Update score
        if (scoreText != null)
            scoreText.text = "Score: " + GameManager.instance.score;

        // Update lives
        if (livesText != null)
            livesText.text = "Lives: " + GameManager.instance.currentLives;

        // Show game over panel
        if (gameOverPanel != null)
        {
            if (GameManager.instance.isGameOver && !gameOverPanel.activeSelf)
            {
                gameOverPanel.SetActive(true);
            }
        }

        // Retry
        if (GameManager.instance.isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            GameManager.instance.RestartLevel();
        }

        // Back to menu
        if (GameManager.instance.isGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.LoadMainMenu();
        }
    }
}