using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3GameManager : MonoBehaviour
{
    public static Level3GameManager instance;

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

    private PlayerController player;
    private ObstacleHitTrigger[] obstacles;

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

        player = FindAnyObjectByType<PlayerController>();
        obstacles = FindObjectsByType<ObstacleHitTrigger>(FindObjectsSortMode.None);

        Debug.Log("GameManager: found player = " + (player != null));
        Debug.Log("GameManager: cached obstacles = " + obstacles.Length);
    }

    void Update()
    {
        if (isGameOver || isLevelComplete)
            return;

        timeElapsed += Time.deltaTime;

        // If player falls too far, count it as death
        if (player != null && player.transform.position.y < -20f)
        {
            PlayerDied();
        }

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
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        if (player == null)
            player = FindAnyObjectByType<PlayerController>();

        if (player != null)
        {
            if (hasCheckpoint)
            {
                player.Respawn(lastCheckpointPosition);
            }
            else
            {
                player.Respawn(player.transform.position);
            }
        }

        StartCoroutine(ResetObstaclesAfterRespawn());
    }

    private IEnumerator ResetObstaclesAfterRespawn()
    {
        yield return new WaitForSeconds(0.15f);

        if (obstacles == null || obstacles.Length == 0)
            obstacles = FindObjectsByType<ObstacleHitTrigger>(FindObjectsSortMode.None);

        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] != null)
                obstacles[i].ResetHit();
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        hasCheckpoint = true;
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        if (player == null)
            player = FindAnyObjectByType<PlayerController>();

        if (player != null)
        {
            player.Die();
        }

        Time.timeScale = 0f;
    }

    public void LevelComplete()
    {
        if (isLevelComplete)
            return;

        isLevelComplete = true;

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