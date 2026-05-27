using System.Collections;
using UnityEngine;

public class LivesIconDisplay : MonoBehaviour
{
    [Header("Heart Icons (IMPORTANT order: Heart1, Heart2, Heart3)")]
    public GameObject[] lifeIcons;

    private int lastLives = -1;

    void Start()
    {
        RefreshAll();
        if (RunnerGameManager.instance != null)
            lastLives = RunnerGameManager.instance.currentLives;
    }

    void Update()
    {
        if (RunnerGameManager.instance == null) return;

        int lives = RunnerGameManager.instance.currentLives;

        // First time safety
        if (lastLives == -1)
        {
            lastLives = lives;
            RefreshAll();
            return;
        }

        // Lives decreased -> animate out the lost heart
        if (lives < lastLives)
        {
            int lostIndex = lastLives - 1; // the last active heart
            if (lostIndex >= 0 && lostIndex < lifeIcons.Length)
                StartCoroutine(AnimateHeartOut(lostIndex));
        }
        // Lives increased -> show/animate in the gained heart(s)
        else if (lives > lastLives)
        {
            for (int i = lastLives; i < lives; i++)
            {
                if (i >= 0 && i < lifeIcons.Length && lifeIcons[i] != null)
                    StartCoroutine(AnimateHeartIn(i));
            }
        }

        lastLives = lives;
    }

    void RefreshAll()
    {
        if (RunnerGameManager.instance == null || lifeIcons == null) return;

        int lives = Mathf.Clamp(RunnerGameManager.instance.currentLives, 0, lifeIcons.Length);

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null)
                lifeIcons[i].SetActive(i < lives);
        }
    }

    IEnumerator AnimateHeartIn(int index)
    {
        GameObject heart = lifeIcons[index];
        if (heart == null) yield break;

        heart.SetActive(true);

        float duration = 0.25f;
        float timer = 0f;
        Vector3 originalScale = heart.transform.localScale;

        // pop-in
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float scale = Mathf.Lerp(0f, 1f, t);
            heart.transform.localScale = originalScale * scale;
            yield return null;
        }

        heart.transform.localScale = originalScale;
    }

    IEnumerator AnimateHeartOut(int index)
    {
        GameObject heart = lifeIcons[index];
        if (heart == null) yield break;

        float duration = 0.4f;
        float timer = 0f;
        Vector3 originalScale = heart.transform.localScale;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            float scale = t < 0.3f
                ? Mathf.Lerp(1f, 1.4f, t / 0.3f)
                : Mathf.Lerp(1.4f, 0f, (t - 0.3f) / 0.7f);

            heart.transform.localScale = originalScale * scale;
            yield return null;
        }

        heart.SetActive(false);
        heart.transform.localScale = originalScale;
    }
}