using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    [Header("Scene")]
    public string sceneName;

    [Header("Visuals")]
    public Image darkOverlay;
    public Image textBackground;
    public TextMeshProUGUI levelText;

    [Header("Sound")]
    public AudioSource sfxSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Animation")]
    public float fadeSpeed = 10f;
    public float slideAmount = 18f;
    public float darkAlpha = 0.45f;

    private RectTransform textRT;
    private RectTransform bgRT;

    private Vector2 textStartPos;
    private Vector2 bgStartPos;

    private bool isHovered;

    void Start()
    {
        textRT = levelText.GetComponent<RectTransform>();
        bgRT = textBackground.GetComponent<RectTransform>();

        textStartPos = textRT.anchoredPosition;
        bgStartPos = bgRT.anchoredPosition;

        SetInstant(false);

        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OpenLevel);
    }

    void Update()
    {
        float target = isHovered ? 1f : 0f;

        SetImageAlpha(darkOverlay, Mathf.Lerp(darkOverlay.color.a, isHovered ? darkAlpha : 0f, Time.deltaTime * fadeSpeed));
        SetImageAlpha(textBackground, Mathf.Lerp(textBackground.color.a, target, Time.deltaTime * fadeSpeed));
        SetTextAlpha(levelText, Mathf.Lerp(levelText.color.a, target, Time.deltaTime * fadeSpeed));

        Vector2 textHidden = textStartPos + new Vector2(0, -slideAmount);
        Vector2 bgHidden = bgStartPos + new Vector2(0, -slideAmount);

        textRT.anchoredPosition = Vector2.Lerp(textRT.anchoredPosition, isHovered ? textStartPos : textHidden, Time.deltaTime * fadeSpeed);
        bgRT.anchoredPosition = Vector2.Lerp(bgRT.anchoredPosition, isHovered ? bgStartPos : bgHidden, Time.deltaTime * fadeSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        PlayHover();
        HoverOn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
            HoverOff();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayHover();
        HoverOn();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        HoverOff();
    }

    void HoverOn()
    {
        isHovered = true;
    }

    void HoverOff()
    {
        isHovered = false;
    }

    void OpenLevel()
    {
        PlayClick();

        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }

    void PlayHover()
    {
        if (sfxSource != null && hoverSound != null)
            sfxSource.PlayOneShot(hoverSound);
    }

    void PlayClick()
    {
        if (sfxSource != null && clickSound != null)
            sfxSource.PlayOneShot(clickSound);
    }

    void SetInstant(bool show)
    {
        float alpha = show ? 1f : 0f;

        SetImageAlpha(darkOverlay, show ? darkAlpha : 0f);
        SetImageAlpha(textBackground, alpha);
        SetTextAlpha(levelText, alpha);

        textRT.anchoredPosition = textStartPos + (show ? Vector2.zero : new Vector2(0, -slideAmount));
        bgRT.anchoredPosition = bgStartPos + (show ? Vector2.zero : new Vector2(0, -slideAmount));
    }

    void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;

        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    void SetTextAlpha(TextMeshProUGUI txt, float alpha)
    {
        if (txt == null) return;

        Color c = txt.color;
        c.a = alpha;
        txt.color = c;
    }
}