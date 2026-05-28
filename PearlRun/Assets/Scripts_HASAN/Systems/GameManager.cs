using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ─────────────────────────────────────
    //  Singleton
    // ─────────────────────────────────────
    public static GameManager instance;

    // ─────────────────────────────────────
    //  Game State
    // ─────────────────────────────────────
    [Header("Game State")]
    public bool isGameOver = false;
    public bool isLevelComplete = false;
    public bool isPaused = false;

    // ─────────────────────────────────────
    //  Lives
    //  Brief says start with 2 lives
    // ─────────────────────────────────────
    [Header("Lives")]
    public int maxLives = 2;
    public int currentLives;

    // ─────────────────────────────────────
    //  Score
    // ─────────────────────────────────────
    [Header("Score")]
    public int score = 0;
    public int pearlsCollected = 0;
    public float timeElapsed = 0f;

    // ─────────────────────────────────────
    //  Checkpoint
    // ─────────────────────────────────────
    [Header("Checkpoint")]
    private Vector3 lastCheckpointPosition;
    private bool hasCheckpoint = false;

    // ─────────────────────────────────────
    //  Level Start Position
    //  Used if no checkpoint exists
    // ─────────────────────────────────────
    private Vector3 levelStartPosition;

    // ─────────────────────────────────────
    //  Unity Lifecycle
    // ─────────────────────────────────────
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ← THIS WAS MISSING
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
        isGameOver = false;
        isLevelComplete = false;
        isPaused = false;
        score = 0;
        pearlsCollected = 0;
        timeElapsed = 0f;
        Time.timeScale = 1f;

        // Save player start position for respawn
        // if no checkpoint exists
        PlayerController player =
            FindAnyObjectByType<PlayerController>();

        if (player != null)
            levelStartPosition = player.transform.position;
    }

    void Update()
    {
        if (isGameOver || isLevelComplete || isPaused)
            return;

        // Track time
        timeElapsed += Time.deltaTime;

        // Pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // ─────────────────────────────────────
    //  Score
    // ─────────────────────────────────────
    public void AddScore(int points)
    {
        score += points;
        pearlsCollected++;
    }

    // ─────────────────────────────────────
    //  Lives
    // ─────────────────────────────────────
    public void AddLife()
    {
        // Golden pearl - cap at maxLives
        if (currentLives < maxLives)
            currentLives++;
    }

    public void AddExtraLife()
    {
        // Red pearl - can go ABOVE maxLives
        currentLives++;
    }

    // ─────────────────────────────────────
    //  Damage Pipeline
    //  PlayerController.TakeDamage()
    //    calls this
    // ─────────────────────────────────────
    public void PlayerHit()
    {
        if (isGameOver) return;

        // Each hit = lose a life directly
        // (matching the brief: 2 lives system)
        PlayerDied();
    }

    // ─────────────────────────────────────
    //  Death & Respawn
    // ─────────────────────────────────────
    public void PlayerDied()
    {
        if (isGameOver) return;

        currentLives--;

        // Play death sound
        if (AudioManager.instance != null)
            AudioManager.instance.PlayDeath();

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
        PlayerController player =
            FindAnyObjectByType<PlayerController>();

        if (player == null) return;

        Vector3 spawnPos;

        if (hasCheckpoint)
            spawnPos = lastCheckpointPosition;
        else
            spawnPos = levelStartPosition; // ← Fixed: use saved start pos

        player.Respawn(spawnPos);
    }

    // ─────────────────────────────────────
    //  Checkpoint
    // ─────────────────────────────────────
    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        hasCheckpoint = true;
        Debug.Log($"Checkpoint set at {position}");
    }

    // ─────────────────────────────────────
    //  Game Over
    // ─────────────────────────────────────
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        // Play game over sound
        if (AudioManager.instance != null)
            AudioManager.instance.PlayGameOver();

        PlayerController player =
            FindAnyObjectByType<PlayerController>();

        if (player != null)
            player.Die();

        Debug.Log("GAME OVER");
        // GameOverUI will read isGameOver and show itself
    }

    // ─────────────────────────────────────
    //  Level Complete
    // ─────────────────────────────────────
    public void LevelComplete()
    {
        if (isLevelComplete) return;

        isLevelComplete = true;

        // Play victory sound
        if (AudioManager.instance != null)
            AudioManager.instance.PlayLevelComplete();

        // Save best score and time
        string scene = SceneManager.GetActiveScene().name;

        int bestScore = PlayerPrefs.GetInt(scene + "_BestScore", 0);
        if (score > bestScore)
            PlayerPrefs.SetInt(scene + "_BestScore", score);

        float bestTime = PlayerPrefs.GetFloat(scene + "_BestTime", 999f);
        if (timeElapsed < bestTime)
            PlayerPrefs.SetFloat(scene + "_BestTime", timeElapsed);

        PlayerPrefs.SetInt(scene + "_Completed", 1);
        PlayerPrefs.Save();

        Debug.Log($"Level Complete! Grade: {GetGrade()}");
    }

    // ─────────────────────────────────────
    //  Pause
    // ─────────────────────────────────────
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (AudioManager.instance != null)
            AudioManager.instance.PauseMusic();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (AudioManager.instance != null)
            AudioManager.instance.ResumeMusic();
    }

    // ─────────────────────────────────────
    //  Scene Loading
    //  Do NOT set instance = null before load
    //  Let Awake() handle duplicates
    // ─────────────────────────────────────
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene("Victory");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel(string levelName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelName);
    }

    // ─────────────────────────────────────
    //  Grade System
    //  Based on score (pearl values) not count
    //  S → All lives + 50+ score
    //  A → 2+ lives + 30+ score
    //  B → 1+ lives + 15+ score
    //  C → anything below
    // ─────────────────────────────────────
    public string GetGrade()
    {
        if (currentLives == maxLives && score >= 50)
            return "S";
        else if (currentLives >= 2 && score >= 30)
            return "A";
        else if (currentLives >= 1 && score >= 15)
            return "B";
        else
            return "C";
    }

    // ─────────────────────────────────────
    //  Getters (for HUD)
    // ─────────────────────────────────────
    public int GetScore() => score;
    public int GetLives() => currentLives;
    public int GetPearls() => pearlsCollected;
    public float GetTime() => timeElapsed;
    public bool HasCheckpoint() => hasCheckpoint;
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}