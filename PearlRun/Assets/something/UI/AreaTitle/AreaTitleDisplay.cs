using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AreaTitleDisplay : MonoBehaviour
{
    public static AreaTitleDisplay instance;

    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public RectTransform backgroundRect;

    [Header("Timing")]
    public float slideInDuration = 0.4f;
    public float holdDuration = 3.0f;
    public float slideOutDuration = 0.4f;

    [Header("Slide Distance")]
    public float slideOffsetX = 1400f;

    [Header("Audio")]
    public AudioClip appearSound;      // plays when background slides in
    public AudioClip disappearSound;   // plays when background slides out
    [Range(0f, 1f)] public float soundVolume = 1f;
    public AudioSource audioSource;    // optional, leave empty to use PlayClipAtPoint

    private Coroutine _current;
    private Vector2 _bgStartPos;

    void Awake()
    {
        instance = this;

        if (backgroundRect != null)
            _bgStartPos = backgroundRect.anchoredPosition;

        SetVisible(false);
    }

    public void ShowTitle(string text)
    {
        if (_current != null)
            StopCoroutine(_current);
        _current = StartCoroutine(AnimateTitle(text));
    }

    private IEnumerator AnimateTitle(string text)
    {
        titleText.text = text;
        SetVisible(true);

        // Play appear sound
        PlaySound(appearSound);

        // ── SLIDE IN ──
        Vector2 offRight = _bgStartPos + new Vector2(slideOffsetX, 0f);
        SetTextAlpha(0f);

        float t = 0f;
        while (t < slideInDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / slideInDuration);
            backgroundRect.anchoredPosition = Vector2.Lerp(offRight, _bgStartPos, p);
            yield return null;
        }
        backgroundRect.anchoredPosition = _bgStartPos;

        // ── FADE IN text ──
        t = 0f;
        float textFadeIn = 0.25f;
        while (t < textFadeIn)
        {
            t += Time.deltaTime;
            SetTextAlpha(Mathf.Lerp(0f, 1f, t / textFadeIn));
            yield return null;
        }
        SetTextAlpha(1f);

        // ── HOLD ──
        yield return new WaitForSeconds(holdDuration);

        // ── FADE OUT text ──
        t = 0f;
        float textFadeOut = 0.2f;
        while (t < textFadeOut)
        {
            t += Time.deltaTime;
            SetTextAlpha(Mathf.Lerp(1f, 0f, t / textFadeOut));
            yield return null;
        }
        SetTextAlpha(0f);

        // Play disappear sound
        PlaySound(disappearSound);

        // ── SLIDE OUT ──
        Vector2 offLeft = _bgStartPos - new Vector2(slideOffsetX, 0f);
        t = 0f;
        while (t < slideOutDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / slideOutDuration);
            backgroundRect.anchoredPosition = Vector2.Lerp(_bgStartPos, offLeft, p);
            yield return null;
        }

        SetVisible(false);
        backgroundRect.anchoredPosition = _bgStartPos;
        _current = null;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        if (audioSource != null)
            audioSource.PlayOneShot(clip, soundVolume);
        else
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, soundVolume);
    }

    private void SetVisible(bool show)
    {
        if (backgroundRect != null)
            backgroundRect.gameObject.SetActive(show);
        if (titleText != null)
            titleText.gameObject.SetActive(show);
    }

    private void SetTextAlpha(float alpha)
    {
        if (titleText == null) return;
        Color c = titleText.color;
        c.a = alpha;
        titleText.color = c;
    }
}