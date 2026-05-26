using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class RanaPausePanelAnimator : MonoBehaviour
{
    [Header("Panel")]
    public GameObject pausePanelGame;
    public GameObject hudPanel;

    [Header("Title")]
    public TMP_Text titleText;

    [Header("Buttons")]
    public Button resumeButton;
    public Button retryButton;
    public Button mainMenuButton;
    public Button quitButton;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip navMoveSound;
    public AudioClip confirmSound;
    public AudioClip pauseOpenSound;

    [Header("Hover")]
    public Color buttonNormalColor = Color.white;
    public Color buttonSelectedColor = new Color(1f, 0.85f, 0.3f);
    public float buttonSelectedScale = 1.12f;

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";

    private bool isPaused = false;
    private bool inputEnabled = false;
    private bool actionInProgress = false;
    private int selectedIndex = 0;

    private Button[] buttons;
    private TMP_Text[] buttonTexts;
    private Vector3[] buttonBaseScales;

    private void Start()
    {
        buttons = new Button[] { resumeButton, retryButton, mainMenuButton, quitButton };
        buttonTexts = new TMP_Text[buttons.Length];
        buttonBaseScales = new Vector3[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            buttonTexts[i] = buttons[i].GetComponentInChildren<TMP_Text>();
            buttonBaseScales[i] = buttons[i].transform.localScale;
        }

        pausePanelGame.SetActive(false);
    }

    private void Update()
    {
        // enforce state every frame — no external script can break it
        EnforceState();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) OpenPause();
            else if (inputEnabled) ActivateButton(0);
        }

        if (!isPaused || !inputEnabled || actionInProgress) return;
        HandleNavigation();
    }

    private void EnforceState()
    {
        if (isPaused)
        {
            if (Time.timeScale != 0f) Time.timeScale = 0f;
            if (AudioListener.pause != true) AudioListener.pause = true;
            if (!pausePanelGame.activeSelf) pausePanelGame.SetActive(true);
            if (hudPanel != null && hudPanel.activeSelf) hudPanel.SetActive(false);
        }
        else
        {
            if (Time.timeScale != 1f) Time.timeScale = 1f;
            if (AudioListener.pause != false) AudioListener.pause = false;
            if (pausePanelGame.activeSelf) pausePanelGame.SetActive(false);
            if (hudPanel != null && !hudPanel.activeSelf) hudPanel.SetActive(true);
        }
    }

    private void OpenPause()
    {
        isPaused = true;
        inputEnabled = false;
        actionInProgress = false;
        selectedIndex = 0;

        PlaySFX(pauseOpenSound);
        UpdateButtonHighlight();
        inputEnabled = true;
    }

    private void HandleNavigation()
    {
        bool moved = false;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        { selectedIndex = (selectedIndex + 1) % buttons.Length; moved = true; }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        { selectedIndex = (selectedIndex - 1 + buttons.Length) % buttons.Length; moved = true; }

        if (moved) { PlaySFX(navMoveSound); UpdateButtonHighlight(); }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Space))
            ActivateButton(selectedIndex);
    }

    private void UpdateButtonHighlight()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            bool sel = (i == selectedIndex);

            if (buttonTexts[i] != null)
                buttonTexts[i].color = sel ? buttonSelectedColor : buttonNormalColor;

            Vector3 target = sel
                ? buttonBaseScales[i] * buttonSelectedScale
                : buttonBaseScales[i];

            StartCoroutine(ScaleSmooth(buttons[i].transform, target, 0.08f));
        }
    }

    private IEnumerator ScaleSmooth(Transform tr, Vector3 target, float dur)
    {
        Vector3 start = tr.localScale;
        float elapsed = 0f;
        while (elapsed < dur)
        {
            elapsed += Time.unscaledDeltaTime;
            float e = 1f - Mathf.Pow(1f - Mathf.Clamp01(elapsed / dur), 3f);
            tr.localScale = Vector3.Lerp(start, target, e);
            yield return null;
        }
        tr.localScale = target;
    }

    private void ActivateButton(int index)
    {
        if (actionInProgress) return;
        actionInProgress = true;
        PlaySFX(confirmSound);
        switch (index)
        {
            case 0: DoResume(); break;
            case 1: DoRetry(); break;
            case 2: DoMainMenu(); break;
            case 3: DoQuit(); break;
        }
    }

    private void DoResume()
    {
        isPaused = false;
        inputEnabled = false;

        for (int i = 0; i < buttons.Length; i++)
            if (buttons[i] != null)
                buttons[i].transform.localScale = buttonBaseScales[i];
    }

    private void DoRetry()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void DoMainMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(mainMenuScene);
    }

    private void DoQuit()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.ignoreListenerPause = true;
            sfxSource.PlayOneShot(clip);
        }
    }

    public void OnResumeClicked() => ActivateButton(0);
    public void OnRetryClicked() => ActivateButton(1);
    public void OnMainMenuClicked() => ActivateButton(2);
    public void OnQuitClicked() => ActivateButton(3);
}