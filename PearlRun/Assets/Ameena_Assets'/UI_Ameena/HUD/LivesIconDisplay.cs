using UnityEngine;
using System.Collections;

public class LivesIconDisplay : MonoBehaviour
{
    public GameObject[] lifeIcons;

    private int lastLives = -1;

    void Start()
    {
        RefreshHearts();
    }

    void Update()
    {
        if (Level3RunnerGameManager.instance == null)
            return;

        int lives = Level3RunnerGameManager.instance.currentLives;

        if (lives != lastLives)
        {
            RefreshHearts();
        }
    }

    void RefreshHearts()
    {
        if (Level3RunnerGameManager.instance == null)
            return;

        int lives = Level3RunnerGameManager.instance.currentLives;

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null)
                lifeIcons[i].SetActive(i < lives);
        }

        lastLives = lives;
    }
}