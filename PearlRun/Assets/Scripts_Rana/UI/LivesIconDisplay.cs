// LivesIconDisplay.cs
using UnityEngine;

public class LivesIconDisplay : MonoBehaviour
{
    [Header("Heart Icons")]
    public GameObject[] lifeIcons;

    private int lastLives = -1;

    void Update()
    {
        if (RunnerGameManager.instance == null) return;

        int lives = RunnerGameManager.instance.currentLives;

        if (lastLives != -1 && lives < lastLives)
        {
            int lostIndex = lastLives - 1;
            if (lostIndex >= 0 && lostIndex < lifeIcons.Length)
                StartCoroutine(AnimateHeartOut(lostIndex));
        }

        lastLives = lives;
    }

    System.Collections.IEnumerator AnimateHeartOut(int index)
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