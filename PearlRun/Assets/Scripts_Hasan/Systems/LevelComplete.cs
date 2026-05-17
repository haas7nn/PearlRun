using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    [Header("Next Level")]
    [SerializeField] private string nextSceneName = "Level2_Manama";
    [SerializeField] private float delayBeforeLoad = 2f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (!other.CompareTag("Player"))
            return;

        hasTriggered = true;
        CompleteLevel();
    }

    private void CompleteLevel()
    {
        // Use lowercase instance, use named shortcut
        if (AudioManager.instance != null)
            AudioManager.instance.PlayLevelComplete();

        Debug.Log("LEVEL COMPLETE!");

        Invoke(nameof(LoadNextLevel), delayBeforeLoad);
    }

    private void LoadNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
}