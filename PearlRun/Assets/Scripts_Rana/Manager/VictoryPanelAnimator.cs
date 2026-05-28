using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    [Header("Stat colors (visual differentiation)")]
    public Color pearlsColor = new Color(1f, 0.85f, 0.3f);   // gold
    public Color timeColor = new Color(0.4f, 0.85f, 1f);   // cyan
    public Color livesColor = new Color(1f, 0.45f, 0.45f);  // red/pink
    public Color gradeColor = new Color(0.7f, 1f, 0.55f);   // green

    [Header("Buttons (order: 0=Left, 1=Center, 2=Right)")]
    public Button replayButton;     // Left
    public Button nextLevelButton;  // Center
    public Button mainMenuButton;   // Right

    [Header("Button highlight")]
    public Color buttonNormalColor = Color.white;
    public Color buttonSelectedColor = new Color(1f, 0.85f, 0.3f); // gold tint
    public float buttonSelectedScale = 1.12f;

    [Header("Particle Effect")]
    public ParticleSystem celebrationParticles;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip statAppearSound;
    public AudioClip navMoveSound;
    public AudioClip confirmSound;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Timing")]
    public float delayBeforeStart = 0.3f;
    public float delayBetweenStats = 0.4f;
    public float popDuration = 0.35f;
    public float popOvershoot = 1.35f;   // peak scale during pop

    private int selectedIndex = 1;       // start with Center (Next Level)
    private bool inputEnabled = false;

    private Button[] buttons;
    private Vector3[] buttonBaseScales;

    private void OnEnable()
    {
        // Title
        if (titleText != null)
            titleText.text = "Awal Made It!";

        // Hide "To Be Continued" until the end
        if (toBeContinuedText != null)
            toBeContinuedText.gameObject.SetActive(false);

        // Particle burst
        if (celebrationParticles != null)
            celebrationParticles.Play();

        // Apply stat colors
        ApplyStatColors();

        // Fill stat values from the live HUD sources
        FillStatsFromHUDSources();

        // Cache buttons + base scales, init highlight
        SetupButtons();

        // Start the reveal animation
        StartCoroutine(RevealOneByOne());
    }

    private void Update()
    {
        if (!inputEnabled) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            MoveSelection(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            MoveSelection(+1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
            ConfirmSelection();
    }

    // ---------- Setup ----------

    private void ApplyStatColors()
    {
        if (pearlsText != null) pearlsText.color = pearlsColor;
        if (timeText != null) timeText.color = timeColor;
        if (livesText != null) livesText.color = livesColor;
        if (gradeText != null) gradeText.color = gradeColor;
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
            timeText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (livesText != null && gm != null)
            livesText.text = "Lives: " + gm.currentLives;

        if (gradeText != null && gm != null)
            gradeText.text = "Grade: " + gm.GetGrade();
    }

    private void SetupButtons()
    {
        buttons = new Button[] { replayButton, nextLevelButton, mainMenuButton };
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

    // ---------- Reveal coroutine ----------

    private IEnumerator RevealOneByOne()
    {
        // Hide all stats
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

        // Allow keyboard input only after everything is shown
        inputEnabled = true;
    }

    private IEnumerator PopIn(TMP_Text t)
    {
        if (t == null) yield break;

        Transform tr = t.transform;
        Vector3 baseScale = tr.localScale;
        if (baseScale == Vector3.zero) baseScale = Vector3.one;

        tr.localScale = Vector3.zero;
        t.gameObject.SetActive(true);

        PlaySound(statAppearSound);

        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(elapsed / popDuration);
            // Ease-out back: overshoots then settles
            float s = EaseOutBack(p, popOvershoot);
            tr.localScale = baseScale * s;
            yield return null;
        }

        tr.localScale = baseScale;
    }

    private float EaseOutBack(float t, float overshoot)
    {
        // Standard "back" easing
        float c1 = (overshoot - 1f) * 2.7f;
        float c3 = c1 + 1f;
        float inv = t - 1f;
        return 1f + c3 * inv * inv * inv + c1 * inv * inv;
    }

    // ---------- Keyboard navigation ----------

    private void MoveSelection(int direction)
    {
        int newIndex = Mathf.Clamp(selectedIndex + direction, 0, buttons.Length - 1);
        if (newIndex == selectedIndex) return;

        selectedIndex = newIndex;
        PlaySound(navMoveSound);
        UpdateButtonHighlight();
    }

    private void UpdateButtonHighlight()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            bool isSelected = (i == selectedIndex);

            // Scale
            buttons[i].transform.localScale =
                buttonBaseScales[i] * (isSelected ? buttonSelectedScale : 1f);

            // Color tint via the button's target Image
            Image img = buttons[i].targetGraphic as Image;
            if (img == null) img = buttons[i].GetComponent<Image>();
            if (img != null)
                img.color = isSelected ? buttonSelectedColor : buttonNormalColor;
        }
    }

    private void ConfirmSelection()
    {
        PlaySound(confirmSound);
        inputEnabled = false;

        switch (selectedIndex)
        {
            case 0: OnReplayButton(); break;
            case 1: OnNextLevelButton(); break;
            case 2: OnMainMenuButton(); break;
        }
    }

    // ---------- Button callbacks (also called by mouse clicks) ----------

    public void OnReplayButton()
    {
        if (RunnerGameManager.instance != null)
            RunnerGameManager.instance.RestartLevel();
    }

    public void OnNextLevelButton()
    {
        if (RunnerGameManager.instance != null)
            RunnerGameManager.instance.LoadNextLevel();
    }

    public void OnMainMenuButton()
    {
        if (RunnerGameManager.instance != null)
            RunnerGameManager.instance.LoadMainMenu();
    }

    // ---------- Helpers ----------

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        if (sfxSource != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
        else
            AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : Vector3.zero, sfxVolume);
    }

    private void Hide(TMP_Text t) { if (t != null) t.gameObject.SetActive(false); }
}