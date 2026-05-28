using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("References")]
    public GameObject mainMenuCanvas;
    public GameObject settingsCanvas;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Sound")]
    public AudioClip clickSound;

    [Header("Animation")]
    public float fadeDuration = 0.35f;
    public float startScale = 0.92f;

    private CanvasGroup canvasGroup;
    private CanvasGroup mainMenuCG;
    private RectTransform rt;
    private bool isAnimating = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rt = GetComponent<RectTransform>();

        mainMenuCG = mainMenuCanvas.GetComponent<CanvasGroup>();
        if (mainMenuCG == null)
            mainMenuCG = mainMenuCanvas.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        StopAllCoroutines();

        float musicValue = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxValue = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSlider != null)
        {
            musicSlider.value = musicValue;
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxValue;
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        SetMusicVolume(musicValue);
        SetSFXVolume(sfxValue);

        StartCoroutine(AnimateIn());
    }

    public void SetMusicVolume(float value)
    {
        if (musicSource != null)
            musicSource.volume = value;

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        if (sfxSource != null)
            sfxSource.volume = value;

        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
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
        canvasGroup.blocksRaycasts = false;

        if (rt != null)
            rt.localScale = Vector3.one * startScale;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            canvasGroup.alpha = smooth;

            if (rt != null)
                rt.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one, smooth);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (rt != null)
            rt.localScale = Vector3.one;

        isAnimating = false;
    }

    System.Collections.IEnumerator AnimateOut()
    {
        isAnimating = true;

        canvasGroup.blocksRaycasts = false;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / (fadeDuration * 0.75f);
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            canvasGroup.alpha = 1f - smooth;

            if (rt != null)
                rt.localScale = Vector3.Lerp(Vector3.one, Vector3.one * startScale, smooth);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        settingsCanvas.SetActive(false);

        mainMenuCG.blocksRaycasts = true;

        isAnimating = false;
    }
}