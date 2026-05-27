using UnityEngine;
using TMPro;

public class Level3SimpleTimer : MonoBehaviour
{
    private TMP_Text timerText;
    private float time;

    void Start()
    {
        timerText = GetComponent<TMP_Text>();
        time = 0f;
    }

    void Update()
    {
        time += Time.deltaTime;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}