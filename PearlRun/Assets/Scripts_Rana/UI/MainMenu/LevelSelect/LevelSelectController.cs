using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class LevelSelectController : MonoBehaviour
{
    [Header("References")]
    public GameObject mainMenuCanvas;
    public GameObject levelSelectCanvas;

    [Header("Level Buttons")]
    public Button[] levelButtons;

    [Header("Sound")]
    public AudioSource sfxSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Animation")]
    public float fadeDuration = 0.35f;
    public float startScale = 0.94f;

    private CanvasGroup canvasGroup;
    private CanvasGroup mainMenuCG;
    private RectTransform rt;

    private int selectedIndex = 0;
    private bool isAnimating = false;

    void Awake()
    {
        canvasGroup = GetOrAddCanvasGroup(levelSelectCanvas);
        mainMenuCG = GetOrAddCanvasGroup(mainMenuCanvas);
        rt = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateIn());

        if (levelButtons != null && levelButtons.Length > 0)
            SelectButton(0, false);
    }

    void Update()
    {
        if (isAnimating) return;
        if (levelButtons == null || levelButtons.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            MoveSelection(1);

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            MoveSelection(-1);

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            MoveSelection(2);

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            MoveSelection(-2);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            PlayClick();
            levelButtons[selectedIndex].onClick.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            BackToMenu();
    }

    void MoveSelection(int direction)
    {
        int newIndex = selectedIndex + direction;

        if (newIndex < 0)
            newIndex = levelButtons.Length - 1;

        if (newIndex >= levelButtons.Length)
            newIndex = 0;

        SelectButton(newIndex, true);
    }

    void SelectButton(int index, bool playSound)
    {
        selectedIndex = index;
        EventSystem.current.SetSelectedGameObject(levelButtons[selectedIndex].gameObject);

        if (playSound)
            PlayHover();
    }

    public void BackToMenu()
    {
        if (isAnimating) return;

        PlayClick();
        StartCoroutine(AnimateOut());
    }

    IEnumerator AnimateIn()
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

    IEnumerator AnimateOut()
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
        levelSelectCanvas.SetActive(false);

        mainMenuCG.blocksRaycasts = true;
        EventSystem.current.SetSelectedGameObject(null);

        isAnimating = false;
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

    CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();

        return cg;
    }
}