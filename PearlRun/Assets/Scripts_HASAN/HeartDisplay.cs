using UnityEngine;
using UnityEngine.UI;

public class HeartDisplay : MonoBehaviour
{
    [Header("Heart Images")]
    [SerializeField] private Image[] hearts;

    [Header("Heart Sprites")]
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    void Update()
    {
        if (RunnerGameManager.instance == null) return;

        int lives = RunnerGameManager.instance.currentLives;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].sprite = i < lives ? fullHeart : emptyHeart;
        }
    }
}