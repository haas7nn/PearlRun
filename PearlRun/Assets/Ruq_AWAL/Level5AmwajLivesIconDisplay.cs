using UnityEngine;
using System.Collections;

public class Level5AmwajLivesIconDisplay : MonoBehaviour
{
    [Header("Heart Icons")]
    public GameObject[] lifeIcons;

    [Header("Animation")]
    public float loseDuration = 0.4f;
    public float gainDuration = 0.4f;

    private int lastLives = -1;

    void Update()
    {
        if (Level5AmwajRunnerGameManager.instance == null) return;

        int lives = Level5AmwajRunnerGameManager.instance.currentLives;

        if (lastLives == -1)
        {
            SyncIconsInstant(lives);
            lastLives = lives;
            return;
        }

        if (lives < lastLives)
        {
            for (int i = lastLives - 1; i >= lives; i--)
            {
                if (i >= 0 && i < lifeIcons.Length)
                    StartCoroutine(AnimateHeartOut(i));
            }
        }
        else if (lives > lastLives)
        {
            for (int i = lastLives; i < lives; i++)
            {
                if (i >= 0 && i < lifeIcons.Length)
                    StartCoroutine(AnimateHeartIn(i));
            }
        }

        lastLives = lives;
    }

    private void SyncIconsInstant(int lives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] == null) continue;

            bool shouldBeOn = i < lives;
            lifeIcons[i].SetActive(shouldBeOn);
            lifeIcons[i].transform.localScale = Vector3.one;
        }
    }

    IEnumerator AnimateHeartOut(int index)
    {
        GameObject heart = lifeIcons[index];
        if (heart == null) yield break;

        Vector3 originalScale = Vector3.one;
        float timer = 0f;

        while (timer < loseDuration)
        {
            timer += Time.deltaTime;
            float t = timer / loseDuration;

            float scale = t < 0.3f
                ? Mathf.Lerp(1f, 1.4f, t / 0.3f)
                : Mathf.Lerp(1.4f, 0f, (t - 0.3f) / 0.7f);

            heart.transform.localScale = originalScale * scale;
            yield return null;
        }

        heart.SetActive(false);
        heart.transform.localScale = originalScale;
    }

    IEnumerator AnimateHeartIn(int index)
    {
        GameObject heart = lifeIcons[index];
        if (heart == null) yield break;

        Vector3 originalScale = Vector3.one;
        heart.transform.localScale = Vector3.zero;
        heart.SetActive(true);

        float timer = 0f;

        while (timer < gainDuration)
        {
            timer += Time.deltaTime;
            float t = timer / gainDuration;

            float scale = t < 0.6f
                ? Mathf.Lerp(0f, 1.4f, t / 0.6f)
                : Mathf.Lerp(1.4f, 1f, (t - 0.6f) / 0.4f);

            heart.transform.localScale = originalScale * scale;
            yield return null;
        }

        heart.transform.localScale = originalScale;
    }
}