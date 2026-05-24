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
        if (Level3GameManager.instance == null)
            return;

        // Update score
        if (scoreText != null)
            scoreText.text = "Score: " + Level3GameManager.instance.score;

        // Update lives
        if (livesText != null)
            livesText.text = "Lives: " + Level3GameManager.instance.currentLives;

        // Show game over panel
        if (gameOverPanel != null)
        {
            if (Level3GameManager.instance.isGameOver && !gameOverPanel.activeSelf)
            {
                gameOverPanel.SetActive(true);
            }
        }

        // Retry
        if (Level3GameManager.instance.isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            Level3GameManager.instance.RestartLevel();
        }

        // Back to menu
        if (Level3GameManager.instance.isGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            Level3GameManager.instance.LoadMainMenu();
        }
    }
}