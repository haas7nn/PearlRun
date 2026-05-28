using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip backgroundMusic;

    [Header("Button Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Buttons")]
    public Button[] buttons;

    [Header("Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject instructionsCanvas;
    public GameObject settingsCanvas;
    public GameObject levelSelectCanvas;

    private CanvasGroup mainMenuCG;
    private int selectedIndex = 0;

    void Start()
    {
        mainMenuCanvas.SetActive(true);
        instructionsCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        levelSelectCanvas.SetActive(false);

        mainMenuCG = GetOrAddCanvasGroup(mainMenuCanvas);
        mainMenuCG.blocksRaycasts = true;

        if (musicSource != null)
        {
            musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);

            if (backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                musicSource.Play();
            }
        }

        if (sfxSource != null)
            sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (buttons.Length > 0)
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
    }

    void Update()
    {
        if (!mainMenuCanvas.activeSelf || !mainMenuCG.blocksRaycasts) return;
        if (buttons == null || buttons.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            MoveSelection(1);

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            MoveSelection(-1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            PlayClick();
            buttons[selectedIndex].onClick.Invoke();
        }
    }

    void MoveSelection(int direction)
    {
        selectedIndex = (selectedIndex + direction + buttons.Length) % buttons.Length;
        EventSystem.current.SetSelectedGameObject(buttons[selectedIndex].gameObject);
        PlayHover();
    }

    public void PlayHover()
    {
        if (sfxSource != null && hoverSound != null)
            sfxSource.PlayOneShot(hoverSound);
    }

    public void PlayClick()
    {
        if (sfxSource != null && clickSound != null)
            sfxSource.PlayOneShot(clickSound);
    }

    public void NewGame()
    {
        PlayClick();
        SceneManager.LoadScene("Level1_Muharraq");
    }

    public void LevelSelect()
    {
        PlayClick();
        mainMenuCG.blocksRaycasts = false;
        levelSelectCanvas.SetActive(true);
    }

    public void Instructions()
    {
        PlayClick();
        mainMenuCG.blocksRaycasts = false;
        instructionsCanvas.SetActive(true);
    }

    public void Settings()
    {
        PlayClick();
        mainMenuCG.blocksRaycasts = false;
        settingsCanvas.SetActive(true);
    }

    public void QuitGame()
    {
        PlayClick();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();

        return cg;
    }
}