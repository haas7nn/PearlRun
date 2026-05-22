using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AreaTitleDisplay : MonoBehaviour
{
    public static AreaTitleDisplay instance;

    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public RectTransform backgroundRect;   // drag your Background here

    [Header("Timing")]
    public float slideInDuration = 0.4f;
    public float holdDuration = 3.0f;
    public float slideOutDuration = 0.4f;

    [Header("Slide Distance")]
    public float slideOffsetX = 1400f;   // how far off-screen it starts/ends

    private Coroutine _current;
    private Vector2 _bgStartPos;       // the center position you set in editor

    void Awake()
    {
        instance = this;

        // Remember the designed center position
        if (backgroundRect != null)
            _bgStartPos = backgroundRect.anchoredPosition;

        // Hide everything at start
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

        // ── SLIDE IN (background comes from right) ──
        Vector2 offRight = _bgStartPos + new Vector2(slideOffsetX, 0f);

        // Start text invisible
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

        // ── FADE IN text after background lands ──
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

        // ── FADE OUT text first ──
        t = 0f;
        float textFadeOut = 0.2f;
        while (t < textFadeOut)
        {
            t += Time.deltaTime;
            SetTextAlpha(Mathf.Lerp(1f, 0f, t / textFadeOut));
            yield return null;
        }
        SetTextAlpha(0f);

        // ── SLIDE OUT (background exits to left) ──
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

        // Reset position for next time
        backgroundRect.anchoredPosition = _bgStartPos;
        _current = null;
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