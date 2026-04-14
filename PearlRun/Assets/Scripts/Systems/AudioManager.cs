using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource footstepSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Music Tracks")]
    [Tooltip("TBD - Team decides between mainmenu or m7rg")]
    public AudioClip menuMusic;
    [Tooltip("m7rg.mp3")]
    public AudioClip level1Music;
    [Tooltip("manama.mp3")]
    public AudioClip level2Music;
    [Tooltip("Qerqi3an.mp3")]
    public AudioClip level3Music;
    [Tooltip("desert.mp3")]
    public AudioClip level4Music;
    [Tooltip("TBD - Team decides between amwaj1 or amwaj2")]
    public AudioClip level5Music;
    [Tooltip("circuit.mp3")]
    public AudioClip level6Music;
    [Tooltip("victory.mp3")]
    public AudioClip victoryMusic;

    [Header("Player SFX")]
    [Tooltip("jumping.wav")]
    public AudioClip jumpSound;
    [Tooltip("landing.mp3")]
    public AudioClip landSound;
    [Tooltip("MISSING - use whoosh or leave empty")]
    public AudioClip slideSound;
    [Tooltip("punch.wav")]
    public AudioClip punchSound;
    [Tooltip("hit.mp3")]
    public AudioClip hurtSound;
    [Tooltip("death.mp3")]
    public AudioClip deathSound;
    [Tooltip("run.mp3")]
    public AudioClip footstepSound;

    [Header("Collectible SFX")]
    [Tooltip("pearlCollect.wav")]
    public AudioClip pearlCollectSound;
    [Tooltip("lifePickup.wav")]
    public AudioClip lifePickupSound;
    [Tooltip("Powerup.wav")]
    public AudioClip powerUpCollectSound;

    [Header("UI SFX")]
    [Tooltip("click.wav")]
    public AudioClip buttonClickSound;
    [Tooltip("MISSING - leave empty for now")]
    public AudioClip buttonHoverSound;
    [Tooltip("GameOver.wav")]
    public AudioClip gameOverSound;
    [Tooltip("levelCompletion.wav")]
    public AudioClip levelCompleteSound;
    [Tooltip("checkpoint.mp3")]
    public AudioClip checkpointSound;

    // Internal state
    private bool isFootstepPlaying = false;

    // ─────────────────────────────────────────
    //  SETUP
    // ─────────────────────────────────────────

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void SetupAudioSources()
    {
        // Music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume * masterVolume;

        // SFX source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume * masterVolume;

        // Footstep source (separate so it loops independently)
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.loop = true;
        footstepSource.playOnAwake = false;
        footstepSource.volume = sfxVolume * masterVolume * 0.6f;
        footstepSource.clip = footstepSound;
    }

    // ─────────────────────────────────────────
    //  MUSIC
    // ─────────────────────────────────────────

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume * masterVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayMusicForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                PlayMusic(menuMusic);
                break;
            case "Level1_Muharraq":
                PlayMusic(level1Music);
                break;
            case "Level2_Manama":
                PlayMusic(level2Music);
                break;
            case "Level3_Qarqaoun":
                PlayMusic(level3Music);
                break;
            case "Level4_Desert":
                PlayMusic(level4Music);
                break;
            case "Level5_Amwaj":
                PlayMusic(level5Music);
                break;
            case "Level6_Circuit":
                PlayMusic(level6Music);
                break;
            case "Victory":
                PlayMusic(victoryMusic);
                break;
        }
    }

    // ─────────────────────────────────────────
    //  FOOTSTEPS
    // ─────────────────────────────────────────

    public void StartFootsteps()
    {
        if (footstepSound == null) return;
        if (isFootstepPlaying) return;

        footstepSource.clip = footstepSound;
        footstepSource.Play();
        isFootstepPlaying = true;
    }

    public void StopFootsteps()
    {
        if (!isFootstepPlaying) return;

        footstepSource.Stop();
        isFootstepPlaying = false;
    }

    // ─────────────────────────────────────────
    //  SFX
    // ─────────────────────────────────────────

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    // Player SFX
    public void PlayJump()
    {
        StopFootsteps();
        PlaySFX(jumpSound);
    }

    public void PlayLand()
    {
        PlaySFX(landSound);
        StartFootsteps();
    }

    public void PlaySlide()
    {
        StopFootsteps();
        PlaySFX(slideSound);
    }

    public void PlayPunch() => PlaySFX(punchSound);
    public void PlayHurt() => PlaySFX(hurtSound);

    public void PlayDeath()
    {
        StopFootsteps();
        StopMusic();
        PlaySFX(deathSound);
    }

    // Collectible SFX
    public void PlayPearlCollect() => PlaySFX(pearlCollectSound);
    public void PlayLifePickup() => PlaySFX(lifePickupSound);
    public void PlayPowerUpCollect() => PlaySFX(powerUpCollectSound);

    // UI SFX
    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlayButtonHover() => PlaySFX(buttonHoverSound);
    public void PlayCheckpoint() => PlaySFX(checkpointSound);

    public void PlayGameOver()
    {
        StopFootsteps();
        StopMusic();
        PlaySFX(gameOverSound);
    }

    public void PlayLevelComplete()
    {
        StopFootsteps();
        StopMusic();
        PlaySFX(levelCompleteSound);
    }

    // ─────────────────────────────────────────
    //  VOLUME CONTROLS
    // ─────────────────────────────────────────

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        UpdateVolumes();
        SaveVolumeSettings();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        UpdateVolumes();
        SaveVolumeSettings();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        UpdateVolumes();
        SaveVolumeSettings();
    }

    void UpdateVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume * masterVolume;

        if (footstepSource != null)
            footstepSource.volume = sfxVolume * masterVolume * 0.6f;
    }

    // ─────────────────────────────────────────
    //  SAVE AND LOAD
    // ─────────────────────────────────────────

    void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        UpdateVolumes();
    }

    // ─────────────────────────────────────────
    //  FADE
    // ─────────────────────────────────────────

    public void FadeOutMusic(float duration = 1f)
    {
        StartCoroutine(FadeRoutine(0f, duration));
    }

    public void FadeInMusic(float duration = 1f)
    {
        StartCoroutine(FadeRoutine(musicVolume * masterVolume, duration));
    }

    System.Collections.IEnumerator FadeRoutine(
        float targetVolume,
        float duration)
    {
        float startVolume = musicSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(
                startVolume,
                targetVolume,
                timer / duration
            );
            yield return null;
        }

        musicSource.volume = targetVolume;
    }
}