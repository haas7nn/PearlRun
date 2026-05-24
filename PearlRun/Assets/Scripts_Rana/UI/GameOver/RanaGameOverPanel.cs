using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RanaGameOverPanel : MonoBehaviour
{
    [Header("Title")]
    public TMP_Text titleText;
    public Color titleColor = new Color(0.95f, 0.2f, 0.2f); // أحمر

    [Header("Buttons (order: 0=Top, 1=Middle, 2=Bottom)")]
    public Button replayButton;     // ReplayButton (Top)
    public Button mainMenuButton;   // MainMenuBtn  (Middle)
    public Button quitButton;       // QuitBtn      (Bottom)

    [Header("Button highlight")]
    public Color buttonNormalColor = Color.white;
    public Color buttonSelectedColor = new Color(1f, 0.55f, 0.3f);
    public float buttonSelectedScale = 1.12f;

    [Header("HUD to Hide (اسحبي هنا كل عناصر الـ HUD)")]
    public GameObject[] hudToHide;   // ScoreText, TimerText, ProgressBar, LevelNameText, LivesIcons, etc.

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip titleAppearSound;
    public AudioClip navMoveSound;
    public AudioClip confirmSound;
    public AudioClip gameOverMusic;   // صوت Game Over الجديد
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float gameOverVolume = 0.7f;

    [Header("Timing")]
    public float delayBeforeStart = 0.3f;
    public float delayBetweenSteps = 0.3f;
    public float popDuration = 0.35f;
    public float popOvershoot = 1.35f;
    public float mainMenuUnlockDelay = 1.5f;

    private int selectedIndex = 0;
    private bool inputEnabled = false;
    private bool mainMenuUnlocked = false;

    private Button[] buttons;
    private Vector3[] buttonBaseScales;

    private void OnEnable()
    {
        // 🔇 وقف كل أصوات اللعبة
        StopAllGameAudio();

        // 👻 خفي الـ HUD
        HideHUD();

        // 🎵 شغل موسيقى Game Over
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
        if (!inputEnabled) return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            MoveSelection(-1);

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            MoveSelection(+1);

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Space))
            ConfirmSelection();

        if (Input.GetKeyDown(KeyCode.R))
        {
            selectedIndex = 0;
            ConfirmSelection();
        }
    }

    // ---------- HUD & Audio ----------

    private void HideHUD()
    {
        if (hudToHide == null) return;
        foreach (GameObject obj in hudToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void StopAllGameAudio()
    {
        // وقف موسيقى الخلفية + صوت الجري من اللاعب
        RunnerController player = FindAnyObjectByType<RunnerController>();
        if (player != null)
        {
            if (player.musicSource != null && player.musicSource.isPlaying)
                player.musicSource.Stop();

            if (player.runSource != null && player.runSource.isPlaying)
                player.runSource.Stop();
        }

        // وقف أي AudioSource ثاني شغّال في المشهد (ما عدا sfxSource اللي عندنا)
        AudioSource[] allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource src in allSources)
        {
            if (src == sfxSource) continue; // لا توقف صوت الـ panel
            if (src.isPlaying && src.loop)  // وقف فقط الأصوات اللوبية (الموسيقى/الجري)
                src.Stop();
        }
    }

    private void PlayGameOverMusic()
    {
        if (sfxSource != null && gameOverMusic != null)
            sfxSource.PlayOneShot(gameOverMusic, gameOverVolume);
    }

    // ---------- Setup ----------

    private void SetupButtons()
    {
        buttons = new Button[] { replayButton, mainMenuButton, quitButton };
        buttonBaseScales = new Vector3[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttonBaseScales[i] = buttons[i].transform.localScale;
                buttons[i].gameObject.SetActive(false);
            }
            else
            {
                buttonBaseScales[i] = Vector3.one;
            }
        }
    }

    // ---------- Reveal coroutine ----------

    private IEnumerator RevealSequence()
    {
        Hide(titleText);

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

    private void Hide(TMP_Text t)
    {
        if (t == null) return;
        t.transform.localScale = Vector3.zero;
        t.gameObject.SetActive(true);
    }

    private IEnumerator PopInText(TMP_Text t, float baseScaleMultiplier)
    {
        if (t == null) yield break;

        Transform tr = t.transform;
        Vector3 finalScale = Vector3.one * baseScaleMultiplier;

        tr.localScale = Vector3.zero;
        t.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(elapsed / popDuration);
            float s = EaseOutBack(p, popOvershoot);
            tr.localScale = finalScale * s;
            yield return null;
        }
        tr.localScale = finalScale;
    }

    private IEnumerator PopInButton(int index)
    {
        if (buttons[index] == null) yield break;

        Transform tr = buttons[index].transform;
        Vector3 finalScale = buttonBaseScales[index];

        tr.localScale = Vector3.zero;
        buttons[index].gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(elapsed / popDuration);
            float s = EaseOutBack(p, popOvershoot);
            tr.localScale = finalScale * s;
            yield return null;
        }
        tr.localScale = finalScale;
    }

    private float EaseOutBack(float t, float overshoot)
    {
        float c1 = (overshoot - 1f) * 2.7f;
        float c3 = c1 + 1f;
        float inv = t - 1f;
        return 1f + c3 * inv * inv * inv + c1 * inv * inv;
    }

    // ---------- Keyboard navigation ----------

    private void MoveSelection(int direction)
    {
        int newIndex = Mathf.Clamp(selectedIndex + direction, 0, buttons.Length - 1);
        if (newIndex != selectedIndex)
        {
            selectedIndex = newIndex;
            PlaySound(navMoveSound);
            UpdateButtonHighlight();
        }
    }

    private void UpdateButtonHighlight()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            bool isSelected = (i == selectedIndex);
            TMP_Text label = buttons[i].GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.color = isSelected ? buttonSelectedColor : buttonNormalColor;

            float scale = isSelected ? buttonSelectedScale : 1f;
            buttons[i].transform.localScale = buttonBaseScales[i] * scale;
        }
    }

    private void ConfirmSelection()
    {
        if (selectedIndex == 1 && !mainMenuUnlocked)
            return;

        PlaySound(confirmSound);
        inputEnabled = false;

        switch (selectedIndex)
        {
            case 0: // Replay
                if (RunnerGameManager.instance != null)
                    RunnerGameManager.instance.RestartLevel();
                break;

            case 1: // Main Menu
                if (RunnerGameManager.instance != null)
                    RunnerGameManager.instance.LoadMainMenu();
                break;

            case 2: // Quit
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }
}