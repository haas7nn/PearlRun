using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private int levelNumber = 1;

    private TMP_Text timerText;
    private float elapsedTime;

    void Start()
    {
        timerText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        timerText.text = FormatTime(elapsedTime);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"Time: {minutes:00}:{seconds:00}";
    }

    public float GetElapsedTime() => elapsedTime;
}