// LevelProgressBar.cs
// Attach this to the ProgressBar GameObject
// Automatically fills based on player's position in the level

using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    [Header("Slider Reference")]
    public Slider progressSlider;

    [Header("Level Boundaries")]
    public float levelStartX = 0f;    // X position at the start of the level
    public float levelEndX = 200f;    // X position at the end — check your FinishZone X

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
        }
    }

    void Update()
    {
        if (player == null || progressSlider == null) return;

        float progress = Mathf.InverseLerp(levelStartX, levelEndX, player.position.x);
        progressSlider.value = Mathf.Clamp01(progress);
    }
}