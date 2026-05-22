// MainMenuManager.cs
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
    public Button[] buttons;   // Drag all buttons here in order: NewGame, LevelSelect, etc.

    private int selectedIndex = 0;

    void Start()
    {
        // Play background music
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        // Select first button by default
        if (buttons.Length > 0)
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
    }

    void Update()
    {
        // Keyboard navigation
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

    // ── Sound helpers ─────────────────────────────────────────────────────────

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

    // ── Button Functions ──────────────────────────────────────────────────────

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
        Debug.Log("Instructions button clicked");
    }

    public void Settings()
    {
        PlayClick();
        Debug.Log("Settings button clicked");
    }

    public void Credits()
    {
        PlayClick();
        Debug.Log("Credits button clicked");
    }

    public void QuitGame()
    {
        PlayClick();
        Application.Quit();
        Debug.Log("Quit");
    }
}