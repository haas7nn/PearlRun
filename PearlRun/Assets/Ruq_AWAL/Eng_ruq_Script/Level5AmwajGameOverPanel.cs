using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class Level5AmwajGameOverPanel : MonoBehaviour
{
    [Header("Title")]
    public TMP_Text titleText;
    public Color titleColor = new Color(0.95f, 0.2f, 0.2f);

    [Header("Buttons")]
    public Button replayButton;
    public Button mainMenuButton;
    public Button quitButton;

    [Header("Button Highlight")]
    public Color buttonNormalColor = Color.white;
    public Color buttonSelectedColor = new Color(1f, 0.55f, 0.3f);
    public float buttonSelectedScale = 1.12f;

    [Header("HUD To Hide")]
    public GameObject[] hudToHide;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip titleAppearSound;
    public AudioClip navMoveSound;
    public AudioClip confirmSound;
    public AudioClip gameOverMusic;

    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float gameOverVolume = 0.7f;

    [Header("Timing")]
    public float delayBeforeStart = 0.3f;
    public float delayBetweenSteps = 0.3f;
    public float popDuration = 0.35f;
    public float popOvershoot = 1.35f;
    public float mainMenuUnlockDelay = 1.5f;

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";

    private int selectedIndex = 0;
    private bool inputEnabled = false;
    private bool mainMenuUnlocked = false;
    private bool actionInProgress = false;

    private Button[] buttons;
    private TMP_Text[] buttonTexts;
    private Vector3[] buttonBaseScales;

    private void OnEnable()
    {
        inputEnabled = false;
        mainMenuUnlocked = false;
        actionInProgress = false;
        selectedIndex = 0;

        StopAllCoroutines();

        StopAllGameAudio();
        HideHUD();
        PlayGameOverMusic();

        if (titleText != null)
        {
            titleText.text = "GAME OVER";
            titleText.color = titleColor;
        }

        SetupButtons();
        StartCoroutine(RevealSequence());
    }

    private void Update()
    {
        if (!inputEnabled || actionInProgress)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            MoveSelection(-1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveSelection(1);
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmSelection();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            selectedIndex = 0;
            ConfirmSelection();
        }
    }

    private void HideHUD()
    {
        if (hudToHide == null)
            return;

        foreach (GameObject obj in hudToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void StopAllGameAudio()
    {
        Level5AmwajPlayerController player = FindAnyObjectByType<Level5AmwajPlayerController>();

        if (player != null)
        {
            /*
             * If your Level5AmwajPlayerController has musicSource/runSource,
             * this will work after you uncomment them.
             *
             * if (player.musicSource != null && player.musicSource.isPlaying)
             *     player.musicSource.Stop();
             *
             * if (player.runSource != null && player.runSource.isPlaying)
             *     player.runSource.Stop();
             */
        }

        AudioSource[] allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource src in allSources)
        {
            if (src == null)
                continue;

            if (src == sfxSource)
                continue;

            if (src.isPlaying && src.loop)
                src.Stop();
        }
    }

    private void PlayGameOverMusic()
    {
        if (sfxSource != null && gameOverMusic != null)
        {
            sfxSource.ignoreListenerPause = true;
            sfxSource.PlayOneShot(gameOverMusic, gameOverVolume);
        }
    }

    private void SetupButtons()
    {
        buttons = new Button[] { replayButton, mainMenuButton, quitButton };
        buttonTexts = new TMP_Text[buttons.Length];
        buttonBaseScales = new Vector3[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttonTexts[i] = buttons[i].GetComponentInChildren<TMP_Text>();
                buttonBaseScales[i] = buttons[i].transform.localScale;
                buttons[i].gameObject.SetActive(false);
            }
            else
            {
                buttonBaseScales[i] = Vector3.one;
            }
        }
    }

    private IEnumerator RevealSequence()
    {
        HideText(titleText);

        yield return new WaitForSecondsRealtime(delayBeforeStart);

        yield return StartCoroutine(PopInText(titleText, 1f));
        PlaySound(titleAppearSound);

        yield return new WaitForSecondsRealtime(delayBetweenSteps);

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                yield return StartCoroutine(PopInButton(i));
                yield return new WaitForSecondsRealtime(delayBetweenSteps * 0.5f);
            }
        }

        UpdateButtonHighlight();
        inputEnabled = true;

        yield return new WaitForSecondsRealtime(mainMenuUnlockDelay);
        mainMenuUnlocked = true;
    }

    private void HideText(TMP_Text text)
    {
        if (text == null)
            return;

        text.transform.localScale = Vector3.zero;
        text.gameObject.SetActive(true);
    }

    private IEnumerator PopInText(TMP_Text text, float baseScaleMultiplier)
    {
        if (text == null)
            yield break;

        Transform target = text.transform;
        Vector3 finalScale = Vector3.one * baseScaleMultiplier;

        target.localScale = Vector3.zero;
        text.gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < popDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / popDuration);
            float scale = EaseOutBack(progress, popOvershoot);

            target.localScale = finalScale * scale;

            yield return null;
        }

        target.localScale = finalScale;
    }

    private IEnumerator PopInButton(int index)
    {
        if (buttons[index] == null)
            yield break;

        Transform target = buttons[index].transform;
        Vector3 finalScale = buttonBaseScales[index];

        target.localScale = Vector3.zero;
        buttons[index].gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < popDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / popDuration);
            float scale = EaseOutBack(progress, popOvershoot);

            target.localScale = finalScale * scale;

            yield return null;
        }

        target.localScale = finalScale;
    }

    private float EaseOutBack(float x, float overshoot)
    {
        float c1 = overshoot;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }

    private void MoveSelection(int direction)
    {
        selectedIndex += direction;

        if (selectedIndex < 0)
            selectedIndex = buttons.Length - 1;

        if (selectedIndex >= buttons.Length)
            selectedIndex = 0;

        PlaySound(navMoveSound);
        UpdateButtonHighlight();
    }

    private void UpdateButtonHighlight()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
                continue;

            bool selected = i == selectedIndex;

            if (buttonTexts[i] != null)
                buttonTexts[i].color = selected ? buttonSelectedColor : buttonNormalColor;

            Vector3 targetScale = selected
                ? buttonBaseScales[i] * buttonSelectedScale
                : buttonBaseScales[i];

            buttons[i].transform.localScale = targetScale;
        }
    }

    private void ConfirmSelection()
    {
        if (actionInProgress)
            return;

        PlaySound(confirmSound);

        switch (selectedIndex)
        {
            case 0:
                ReplayLevel();
                break;

            case 1:
                if (mainMenuUnlocked)
                    GoToMainMenu();
                break;

            case 2:
                QuitGame();
                break;
        }
    }

    private void ReplayLevel()
    {
        actionInProgress = true;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        actionInProgress = true;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene(mainMenuScene);
    }

    private void QuitGame()
    {
        actionInProgress = true;

        Time.timeScale = 1f;
        AudioListener.pause = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PlaySound(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.ignoreListenerPause = true;
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void OnReplayClicked()
    {
        selectedIndex = 0;
        ConfirmSelection();
    }

    public void OnMainMenuClicked()
    {
        selectedIndex = 1;
        mainMenuUnlocked = true;
        ConfirmSelection();
    }

    public void OnQuitClicked()
    {
        selectedIndex = 2;
        ConfirmSelection();
    }
}