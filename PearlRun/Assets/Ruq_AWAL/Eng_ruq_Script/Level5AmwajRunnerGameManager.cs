using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level5AmwajRunnerGameManager : MonoBehaviour
{
    public static Level5AmwajRunnerGameManager instance;

    [Header("Game State")]
    public bool isGameOver = false;
    public bool isLevelComplete = false;
    public bool isPaused = false;

    [Header("Lives")]
    public int maxLives = 3;
    public int currentLives;
    public int maxHitsPerLife = 1;
    private int currentHits;

    [Header("Respawn")]
    public float respawnDelay = 1.2f;
    public float respawnYOffset = 0.5f;

    [Header("Score")]
    public int score = 0;
    public int pearlsCollected = 0;
    public float timeElapsed = 0f;

    [Header("Checkpoint")]
    private Vector3 lastCheckpointPosition;
    private bool hasCheckpoint = false;
    private Vector3 levelStartPosition;

    private Level5AmwajPlayerController player;
    private Level5AmwajObstacleHitTrigger[] obstacles;
    private bool isRespawning = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
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
        isRespawning = false;

        score = 0;
        pearlsCollected = 0;
        timeElapsed = 0f;

        Time.timeScale = 1f;

        player = FindAnyObjectByType<Level5AmwajPlayerController>();

        if (player != null)
        {
            levelStartPosition = player.transform.position;

            lastCheckpointPosition = levelStartPosition;
            hasCheckpoint = true;

            Debug.Log("LEVEL START CHECKPOINT = " + lastCheckpointPosition);
        }

        obstacles = FindObjectsByType<Level5AmwajObstacleHitTrigger>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (isGameOver || isLevelComplete || isRespawning)
            return;

        timeElapsed += Time.deltaTime;

        if (player != null && player.transform.position.y < -20f)
            PlayerDied();

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

        if (Level5AmwajScoreManager.Instance != null)
            Level5AmwajScoreManager.Instance.AddPearls(1);
    }

    public void AddLife()
    {
        currentLives++;

        if (currentLives > maxLives)
            currentLives = maxLives;
    }

    public void PlayerHit()
    {
        if (isGameOver || isRespawning)
            return;

        currentHits++;

        if (currentHits >= maxHitsPerLife)
            PlayerDied();
    }

    public void PlayerDied()
    {
        if (isGameOver || isRespawning)
            return;

        currentLives--;
        currentHits = 0;

        if (player == null)
            player = FindAnyObjectByType<Level5AmwajPlayerController>();

        if (player != null)
            player.Die();

        if (currentLives <= 0)
            GameOver();
        else
            StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        isRespawning = true;

        yield return new WaitForSeconds(respawnDelay);

        RespawnPlayer();

        isRespawning = false;
    }

    public void RestoreFullLives()
    {
        currentLives = maxLives;
        currentHits = 0;
    }

    void RespawnPlayer()
    {
        if (player == null)
            player = FindAnyObjectByType<Level5AmwajPlayerController>();

        if (player == null)
            return;

        Vector3 respawnPosition;

        if (hasCheckpoint)
            respawnPosition = lastCheckpointPosition;
        else
            respawnPosition = levelStartPosition;

        respawnPosition.y += respawnYOffset;

        Debug.Log("RESPAWNING AT = " + respawnPosition);

        player.Respawn(respawnPosition);

        StartCoroutine(ResetObstaclesDelayed());
    }

    private IEnumerator ResetObstaclesDelayed()
    {
        yield return new WaitForSeconds(0.15f);

        if (obstacles == null || obstacles.Length == 0)
            obstacles = FindObjectsByType<Level5AmwajObstacleHitTrigger>(FindObjectsSortMode.None);

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

        Debug.Log("CHECKPOINT SAVED = " + position);
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        Debug.Log("Game Over");
    }

    public void FreezeGameAfterDeath()
    {
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
            PlayerPrefs.SetInt(currentScene + "_BestScore", score);

        float bestTime = PlayerPrefs.GetFloat(currentScene + "_BestTime", 999f);

        if (timeElapsed < bestTime)
            PlayerPrefs.SetFloat(currentScene + "_BestTime", timeElapsed);

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