using UnityEngine;

public class InstructionsController : MonoBehaviour
{
    [Header("References")]
    public GameObject mainMenuCanvas;
    public GameObject instructionsCanvas;

    [Header("Sound")]
    public AudioSource sfxSource;
    public AudioClip clickSound;

    [Header("Animation Settings")]
    public float fadeDuration = 0.4f;

    private CanvasGroup canvasGroup;
    private CanvasGroup mainMenuCG;
    private bool isAnimating = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        mainMenuCG = mainMenuCanvas.GetComponent<CanvasGroup>();
        if (mainMenuCG == null)
            mainMenuCG = mainMenuCanvas.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateIn());
    }

    public void BackToMenu()
    {
        if (isAnimating) return;
        PlayClick();
        StartCoroutine(AnimateOut());
    }

    void PlayClick()
    {
        if (sfxSource != null && clickSound != null)
            sfxSource.PlayOneShot(clickSound);
    }

    System.Collections.IEnumerator AnimateIn()
    {
        isAnimating = true;
        canvasGroup.alpha = 0f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            canvasGroup.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        isAnimating = false;
    }

    System.Collections.IEnumerator AnimateOut()
    {
        isAnimating = true;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (fadeDuration * 0.7f);
            canvasGroup.alpha = 1f - Mathf.Clamp01(t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        instructionsCanvas.SetActive(false);
        mainMenuCG.blocksRaycasts = true;
        isAnimating = false;
    }
}