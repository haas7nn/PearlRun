using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class Level3PausePanelAnimator : MonoBehaviour
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
            if (buttons[i] == null)
                continue;

            buttonTexts[i] = buttons[i].GetComponentInChildren<TMP_Text>();
            buttonBaseScales[i] = buttons[i].transform.localScale;
        }

        if (pausePanelGame != null)
            pausePanelGame.SetActive(false);

        if (hudPanel != null)
            hudPanel.SetActive(true);

        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void Update()
    {
        EnforceState();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                OpenPause();
            else if (inputEnabled)
                ActivateButton(0);
        }

        if (!isPaused || !inputEnabled || actionInProgress)
            return;

        HandleNavigation();
    }

    private void EnforceState()
    {
        if (isPaused)
        {
            if (Time.timeScale != 0f)
                Time.timeScale = 0f;

            if (AudioListener.pause != true)
                AudioListener.pause = true;

            if (pausePanelGame != null && !pausePanelGame.activeSelf)
                pausePanelGame.SetActive(true);

            if (hudPanel != null && hudPanel.activeSelf)
                hudPanel.SetActive(false);
        }
        else
        {
            if (Time.timeScale != 1f)
                Time.timeScale = 1f;

            if (AudioListener.pause != false)
                AudioListener.pause = false;

            if (pausePanelGame != null && pausePanelGame.activeSelf)
                pausePanelGame.SetActive(false);

            if (hudPanel != null && !hudPanel.activeSelf)
                hudPanel.SetActive(true);
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
        {
            selectedIndex = (selectedIndex + 1) % buttons.Length;
            moved = true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selectedIndex = (selectedIndex - 1 + buttons.Length) % buttons.Length;
            moved = true;
        }

        if (moved)
        {
            PlaySFX(navMoveSound);
            UpdateButtonHighlight();
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            ActivateButton(selectedIndex);
        }
    }

    private void UpdateButtonHighlight()
    {
        StopAllCoroutines();

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

            StartCoroutine(ScaleSmooth(buttons[i].transform, targetScale, 0.08f));
        }
    }

    private IEnumerator ScaleSmooth(Transform target, Vector3 targetScale, float duration)
    {
        Vector3 startScale = target.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float smooth = 1f - Mathf.Pow(1f - t, 3f);

            target.localScale = Vector3.Lerp(startScale, targetScale, smooth);

            yield return null;
        }

        target.localScale = targetScale;
    }

    private void ActivateButton(int index)
    {
        if (actionInProgress)
            return;

        actionInProgress = true;
        PlaySFX(confirmSound);

        switch (index)
        {
            case 0:
                DoResume();
                break;

            case 1:
                DoRetry();
                break;

            case 2:
                DoMainMenu();
                break;

            case 3:
                DoQuit();
                break;
        }
    }

    private void DoResume()
    {
        isPaused = false;
        inputEnabled = false;
        actionInProgress = false;

        ResetButtons();
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

    private void ResetButtons()
    {
        StopAllCoroutines();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
                buttons[i].transform.localScale = buttonBaseScales[i];

            if (buttonTexts[i] != null)
                buttonTexts[i].color = buttonNormalColor;
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.ignoreListenerPause = true;
            sfxSource.PlayOneShot(clip);
        }
    }

    public void OnResumeClicked()
    {
        selectedIndex = 0;
        ActivateButton(0);
    }

    public void OnRetryClicked()
    {
        selectedIndex = 1;
        ActivateButton(1);
    }

    public void OnMainMenuClicked()
    {
        selectedIndex = 2;
        ActivateButton(2);
    }

    public void OnQuitClicked()
    {
        selectedIndex = 3;
        ActivateButton(3);
    }
}