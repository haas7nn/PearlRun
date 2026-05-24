using UnityEngine;
using System.Collections;

public class LivesIconDisplay : MonoBehaviour
{
    [Header("Heart Icons")]
    public GameObject[] lifeIcons;

    [Header("Animation")]
    public float loseDuration = 0.4f;
    public float gainDuration = 0.4f;

    private int lastLives = -1;

    void Update()
    {
        if (RunnerGameManager.instance == null) return;

        int lives = RunnerGameManager.instance.currentLives;

        // First frame — just sync icons to current lives, no animation
        if (lastLives == -1)
        {
            SyncIconsInstant(lives);
            lastLives = lives;
            return;
        }

        // Lost one or more hearts → animate them OUT (highest index first)
        if (lives < lastLives)
        {
            for (int i = lastLives - 1; i >= lives; i--)
            {
                if (i >= 0 && i < lifeIcons.Length)
                    StartCoroutine(AnimateHeartOut(i));
            }
        }
        // Gained one or more hearts (Red / Golden pearl) → animate them IN
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

    // Snap icons to the correct on/off state with no animation (used on first frame)
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

    // Heart lost: pop slightly bigger, then shrink to zero & hide
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

    // Heart gained: appear, overshoot bigger, settle back to normal size
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
            // 0 → 1.4 (overshoot) → 1.0 (settle)
            float scale = t < 0.6f
                ? Mathf.Lerp(0f, 1.4f, t / 0.6f)
                : Mathf.Lerp(1.4f, 1f, (t - 0.6f) / 0.4f);
            heart.transform.localScale = originalScale * scale;
            yield return null;
        }

        heart.transform.localScale = originalScale;
    }
}