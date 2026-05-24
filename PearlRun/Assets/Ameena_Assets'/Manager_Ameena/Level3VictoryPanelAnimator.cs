using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Level3VictoryPanelAnimator : MonoBehaviour
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
    [Range(0f, 1f)] public float sfxVolume = 1f;

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
        FillStatsFromGameManager();
        SetupButtons();

        StopAllCoroutines();
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

    private void FillStatsFromGameManager()
    {
        Level3RunnerGameManager gm = Level3RunnerGameManager.instance;

        if (pearlsText != null)
        {
            int pearls = gm != null ? gm.pearlsCollected : 0;
            pearlsText.text = "Score: " + pearls;
        }

        if (timeText != null)
        {
            float t = gm != null ? gm.timeElapsed : 0f;
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);

            timeText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (livesText != null)
        {
            int lives = gm != null ? gm.currentLives : 0;
            livesText.text = "Lives: " + lives;
        }

        if (gradeText != null)
        {
            string grade = gm != null ? gm.GetGrade() : "C";
            gradeText.text = "Grade: " + grade;
        }
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

    private IEnumerator RevealOneByOne()
    {
        inputEnabled = false;

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

    private IEnumerator PopIn(TMP_Text text)
    {
        if (text == null)
            yield break;

        Transform textTransform = text.transform;

        Vector3 baseScale = textTransform.localScale;
        if (baseScale == Vector3.zero)
            baseScale = Vector3.one;

        textTransform.localScale = Vector3.zero;
        text.gameObject.SetActive(true);

        PlaySound(statAppearSound);

        float elapsed = 0f;

        while (elapsed < popDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / popDuration);
            float scale = EaseOutBack(progress, popOvershoot);

            textTransform.localScale = baseScale * scale;

            yield return null;
        }

        textTransform.localScale = baseScale;
    }

    private float EaseOutBack(float t, float overshoot)
    {
        float c1 = (overshoot - 1f) * 2.7f;
        float c3 = c1 + 1f;
        float inv = t - 1f;

        return 1f + c3 * inv * inv * inv + c1 * inv * inv;
    }

    private void MoveSelection(int direction)
    {
        int newIndex = Mathf.Clamp(selectedIndex + direction, 0, buttons.Length - 1);

        if (newIndex == selectedIndex)
            return;

        selectedIndex = newIndex;

        PlaySound(navMoveSound);
        UpdateButtonHighlight();
    }

    private void UpdateButtonHighlight()
    {
        if (buttons == null || buttonBaseScales == null)
            return;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
                continue;

            bool isSelected = i == selectedIndex;

            buttons[i].transform.localScale =
                buttonBaseScales[i] * (isSelected ? buttonSelectedScale : 1f);

            Image image = buttons[i].targetGraphic as Image;

            if (image == null)
                image = buttons[i].GetComponent<Image>();

            if (image != null)
                image.color = isSelected ? buttonSelectedColor : buttonNormalColor;
        }
    }

    private void ConfirmSelection()
    {
        PlaySound(confirmSound);
        inputEnabled = false;

        switch (selectedIndex)
        {
            case 0:
                OnReplayButton();
                break;

            case 1:
                OnNextLevelButton();
                break;

            case 2:
                OnMainMenuButton();
                break;
        }
    }

    public void OnReplayButton()
    {
        if (Level3RunnerGameManager.instance != null)
            Level3RunnerGameManager.instance.RestartLevel();
    }

    public void OnNextLevelButton()
    {
        if (Level3RunnerGameManager.instance != null)
            Level3RunnerGameManager.instance.LoadNextLevel();
    }

    public void OnMainMenuButton()
    {
        if (Level3RunnerGameManager.instance != null)
            Level3RunnerGameManager.instance.LoadMainMenu();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
            return;

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
        else
        {
            Vector3 soundPosition = Camera.main != null
                ? Camera.main.transform.position
                : Vector3.zero;

            AudioSource.PlayClipAtPoint(clip, soundPosition, sfxVolume);
        }
    }

    private void Hide(TMP_Text text)
    {
        if (text != null)
            text.gameObject.SetActive(false);
    }
}