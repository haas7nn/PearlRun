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

    [Header("Buttons (for keyboard navigation)")]
    public Button[] buttons;

    [Header("Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject instructionsCanvas;

    private CanvasGroup mainMenuCG;
    private int selectedIndex = 0;

    void Start()
    {
        mainMenuCanvas.SetActive(true);
        instructionsCanvas.SetActive(false);

        mainMenuCG = mainMenuCanvas.GetComponent<CanvasGroup>();
        if (mainMenuCG == null)
            mainMenuCG = mainMenuCanvas.AddComponent<CanvasGroup>();
        mainMenuCG.blocksRaycasts = true;

        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        if (buttons.Length > 0)
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            selectedIndex = (selectedIndex + 1) % buttons.Length;
            EventSystem.current.SetSelectedGameObject(buttons[selectedIndex].gameObject);
            PlayHover();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selectedIndex = (selectedIndex - 1 + buttons.Length) % buttons.Length;
            EventSystem.current.SetSelectedGameObject(buttons[selectedIndex].gameObject);
            PlayHover();
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            PlayClick();
            buttons[selectedIndex].onClick.Invoke();
        }
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
        SceneManager.LoadScene("LevelSelect");
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
        Debug.Log("Settings clicked");
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
}