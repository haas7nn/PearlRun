// Level5AmwajProgressBar.cs
// Attach this to the ProgressBar GameObject
// Automatically fills based on player's position in the level

using UnityEngine;
using UnityEngine.UI;

public class Level5AmwajProgressBar : MonoBehaviour
{
    [Header("Slider Reference")]
    public Slider progressSlider;

    [Header("Level Boundaries")]
    public float levelStartX = 0f;
    public float levelEndX = 200f;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
        }
    }

    void Update()
    {
        if (player == null || progressSlider == null)
            return;

        float progress = Mathf.InverseLerp(
            levelStartX,
            levelEndX,
            player.position.x
        );

        progressSlider.value = Mathf.Clamp01(progress);
    }
}