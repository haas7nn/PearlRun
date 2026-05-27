using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager instance;

    public int score = 0;
    public int lives = 2;

    private void Awake()
    {
        instance = this;
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
    }

    public void AddLife(int amount)
    {
        lives += amount;
        Debug.Log("Lives: " + lives);
    }

    public void RestoreLife(int amount)
    {
        lives += amount;
        Debug.Log("Life Restored. Lives: " + lives);
    }
}