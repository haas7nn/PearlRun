using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game State")]
    public bool isGameOver = false;
    public bool isLevelComplete = false;
    public bool isPaused = false;

    [Header("Lives")]
    public int maxLives = 3;
    public int currentLives;
    public int maxHitsPerLife = 1;
    private int currentHits;

    [Header("Score")]
    public int score = 0;
    public int pearlsCollected = 0;
    public float timeElapsed = 0f;

    [Header("Checkpoint")]
    private Vector3 lastCheckpointPosition;
    private bool hasCheckpoint = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        currentLives = maxLives;
        currentHits = 0;
        isGameOver = false;
        isLevelComplete = false;
        isPaused = false;
        score = 0;
        pearlsCollected = 0;
        timeElapsed = 0f;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (isGameOver || isLevelComplete)
            return;

        // Track time
        timeElapsed += Time.deltaTime;

        // Safety check - if player falls too far, kill them
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null && player.transform.position.y < -20f)
        {
            PlayerDied();
        }
        // Pause toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void AddScore(int points)
    {
        score += points;
        pearlsCollected++;
    }

    public void AddLife()
    {
        currentLives++;
    }

    public void PlayerHit()
    {
        if (isGameOver)
            return;

        currentHits++;

        if (currentHits >= maxHitsPerLife)
        {
            PlayerDied();
        }
    }

    public void PlayerDied()
    {
        if (isGameOver)
            return;

        currentLives--;
        currentHits = 0;

        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            // Respawn at checkpoint
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            if (hasCheckpoint)
            {
                player.Respawn(lastCheckpointPosition);
            }
            else
            {
                // Respawn at start of level
                player.Respawn(player.transform.position);
            }
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        hasCheckpoint = true;
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.Die();
        }
    }

    public void LevelComplete()
    {
        if (isLevelComplete)
            return;

        isLevelComplete = true;

        // Save level data
        string currentScene = SceneManager.GetActiveScene().name;
        int bestScore = PlayerPrefs.GetInt(currentScene + "_BestScore", 0);
        if (score > bestScore)
        {
            PlayerPrefs.SetInt(currentScene + "_BestScore", score);
        }

        float bestTime = PlayerPrefs.GetFloat(currentScene + "_BestTime", 999f);
        if (timeElapsed < bestTime)
        {
            PlayerPrefs.SetFloat(currentScene + "_BestTime", timeElapsed);
        }

        PlayerPrefs.SetInt(currentScene + "_Completed", 1);
        PlayerPrefs.Save();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        instance = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            instance = null;
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            instance = null;
            SceneManager.LoadScene("Victory");
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        instance = null;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel(string levelName)
    {
        Time.timeScale = 1f;
        instance = null;
        SceneManager.LoadScene(levelName);
    }

    public string GetGrade()
    {
        // Simple grading based on lives remaining and pearls
        if (currentLives == maxLives && pearlsCollected > 50)
            return "S";
        else if (currentLives >= 2 && pearlsCollected > 30)
            return "A";
        else if (currentLives >= 1 && pearlsCollected > 15)
            return "B";
        else
            return "C";
    }
}