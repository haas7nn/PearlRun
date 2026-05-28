using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class VictoryPanelAnimator : MonoBehaviour
{
    [Header("Texts unique to this panel")]
    public TMP_Text titleText;
    public TMP_Text toBeContinuedText;

    [Header("Stat texts on THIS panel")]
    public TMP_Text pearlsText;
    public TMP_Text timeText;
    public TMP_Text livesText;
    public TMP_Text gradeText;

    [Header("Stat colors")]
    public Color pearlsColor = new Color(1f, 0.85f, 0.3f);
    public Color timeColor = new Color(0.4f, 0.85f, 1f);
    public Color livesColor = new Color(1f, 0.45f, 0.45f);
    public Color gradeColor = new Color(0.7f, 1f, 0.55f);

    [Header("Buttons")]
    public Button replayButton;
    public Button nextLevelButton;
    public Button mainMenuButton;

    [Header("Scene Names")]
    public string nextLevelSceneName;
    public string mainMenuSceneName = "MainMenu";

    [Header("Button highlight")]
    public Color buttonNormalColor = Color.white;
    public Color buttonSelectedColor = new Color(1f, 0.85f, 0.3f);
    public float buttonSelectedScale = 1.12f;

    [Header("Particle Effect")]
    public ParticleSystem celebrationParticles;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip statAppearSound;
    public AudioClip navMoveSound;
    public AudioClip confirmSound;

    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Header("Timing")]
    public float delayBeforeStart = 0.3f;
    public float delayBetweenStats = 0.4f;
    public float popDuration = 0.35f;
    public float popOvershoot = 1.35f;

    private int selectedIndex = 1;
    private bool inputEnabled = false;

    private Button[] buttons;
    private Vector3[] buttonBaseScales;

    private void OnEnable()
    {
        if (titleText != null)
            titleText.text = "Awal Made It!";

        if (toBeContinuedText != null)
            toBeContinuedText.gameObject.SetActive(false);

        if (celebrationParticles != null)
            celebrationParticles.Play();

        ApplyStatColors();
        FillStatsFromHUDSources();
        SetupButtons();

        StartCoroutine(RevealOneByOne());
    }

    private void Update()
    {
        if (!inputEnabled)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            MoveSelection(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            MoveSelection(1);

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmSelection();
        }
    }

    // --------------------------------------------------
    // SETUP
    // --------------------------------------------------

    private void ApplyStatColors()
    {
        if (pearlsText != null)
            pearlsText.color = pearlsColor;

        if (timeText != null)
            timeText.color = timeColor;

        if (livesText != null)
            livesText.color = livesColor;

        if (gradeText != null)
            gradeText.color = gradeColor;
    }

    private void FillStatsFromHUDSources()
    {
        RunnerGameManager gm = RunnerGameManager.instance;
        ScoreManager sm = ScoreManager.Instance;

        if (pearlsText != null)
        {
            int pearls = (sm != null) ? sm.currentPearls : 0;
            pearlsText.text = "Score: " + pearls;
        }

        if (timeText != null && gm != null)
        {
            float t = gm.timeElapsed;

            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);

            timeText.text = "Time: " +
                string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (livesText != null && gm != null)
        {
            livesText.text = "Lives: " + gm.currentLives;
        }

        if (gradeText != null && gm != null)
        {
            gradeText.text = "Grade: " + gm.GetGrade();
        }
    }

    private void SetupButtons()
    {
        buttons = new Button[]
        {
            replayButton,
            nextLevelButton,
            mainMenuButton
        };

        buttonBaseScales = new Vector3[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
                buttonBaseScales[i] = buttons[i].transform.localScale;
            else
                buttonBaseScales[i] = Vector3.one;
        }

        UpdateButtonHighlight();
    }

    // --------------------------------------------------
    // REVEAL ANIMATION
    // --------------------------------------------------

    private IEnumerator RevealOneByOne()
    {
        Hide(pearlsText);
        Hide(timeText);
        Hide(livesText);
        Hide(gradeText);

        yield return new WaitForSecondsRealtime(delayBeforeStart);

        yield return StartCoroutine(PopIn(pearlsText));
        yield return new WaitForSecondsRealtime(delayBetweenStats);

        yield return StartCoroutine(PopIn(timeText));
        yield return new WaitForSecondsRealtime(delayBetweenStats);

        yield return StartCoroutine(PopIn(livesText));
        yield return new WaitForSecondsRealtime(delayBetweenStats);

        yield return StartCoroutine(PopIn(gradeText));
        yield return new WaitForSecondsRealtime(delayBetweenStats * 2f);

        if (toBeContinuedText != null)
            toBeContinuedText.gameObject.SetActive(true);

        inputEnabled = true;
    }

    private IEnumerator PopIn(TMP_Text t)
    {
        if (t == null)
            yield break;

        Transform tr = t.transform;

        Vector3 baseScale = tr.localScale;

        if (baseScale == Vector3.zero)
            baseScale = Vector3.one;

        tr.localScale = Vector3.zero;

        t.gameObject.SetActive(true);

        PlaySound(statAppearSound);

        float elapsed = 0f;

        while (elapsed < popDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float p = Mathf.Clamp01(elapsed / popDuration);

            float s = EaseOutBack(p, popOvershoot);

            tr.localScale = baseScale * s;

            yield return null;
        }

        tr.localScale = baseScale;
    }

    private float EaseOutBack(float t, float overshoot)
    {
        float c1 = (overshoot - 1f) * 2.7f;
        float c3 = c1 + 1f;

        float inv = t - 1f;

        return 1f + c3 * inv * inv * inv + c1 * inv * inv;
    }

    // --------------------------------------------------
    // BUTTON NAVIGATION
    // --------------------------------------------------

    private void MoveSelection(int direction)
    {
        selectedIndex += direction;

        if (selectedIndex < 0)
            selectedIndex = buttons.Length - 1;

        if (selectedIndex >= buttons.Length)
            selectedIndex = 0;

        UpdateButtonHighlight();

        PlaySound(navMoveSound);
    }

    private void UpdateButtonHighlight()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
                continue;

            Transform tr = buttons[i].transform;

            Image img = buttons[i].GetComponent<Image>();

            if (i == selectedIndex)
            {
                tr.localScale =
                    buttonBaseScales[i] * buttonSelectedScale;

                if (img != null)
                    img.color = buttonSelectedColor;
            }
            else
            {
                tr.localScale = buttonBaseScales[i];

                if (img != null)
                    img.color = buttonNormalColor;
            }
        }
    }

    private void ConfirmSelection()
    {
        PlaySound(confirmSound);

        switch (selectedIndex)
        {
            // Replay
            case 0:
                SceneManager.LoadScene(
                    SceneManager.GetActiveScene().buildIndex
                );
                break;

            // Next Level
            case 1:

                if (!string.IsNullOrEmpty(nextLevelSceneName))
                {
                    SceneManager.LoadScene(nextLevelSceneName);
                }
                else
                {
                    Debug.LogWarning("Next Level Scene Name is EMPTY!");
                }

                break;

            // Main Menu
            case 2:

                SceneManager.LoadScene(mainMenuSceneName);

                break;
        }
    }

    // --------------------------------------------------
    // HELPERS
    // --------------------------------------------------

    private void Hide(TMP_Text t)
    {
        if (t != null)
            t.gameObject.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }
}