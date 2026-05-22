using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpHUD : MonoBehaviour
{
    [Header("References")]
    public GameObject powerUpDisplay;
    public Image powerUpIcon;
    public TMP_Text powerUpTimerText;

    [Header("Power-up Sprites")]
    public Sprite shieldSprite;
    public Sprite magnetSprite;
    public Sprite slowMotionSprite;
    public Sprite doublePointsSprite;

    [HideInInspector] public float currentTimer = 0f;
    [HideInInspector] public Sprite currentSprite = null;

    void Update()
    {
        if (currentTimer > 0f)
        {
            currentTimer -= Time.deltaTime;

            if (powerUpDisplay != null)
                powerUpDisplay.SetActive(true);

            if (powerUpIcon != null && currentSprite != null)
                powerUpIcon.sprite = currentSprite;

            if (powerUpTimerText != null)
                powerUpTimerText.text = Mathf.Ceil(currentTimer) + "s";
        }
        else
        {
            if (powerUpDisplay != null)
                powerUpDisplay.SetActive(false);
        }
    }

    public void ActivatePowerUp(Sprite icon, float duration)
    {
        currentSprite = icon;
        currentTimer = duration;
    }
}