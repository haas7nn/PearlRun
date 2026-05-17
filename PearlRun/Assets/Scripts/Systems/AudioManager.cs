using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // ─────────────────────────────────────
    //  Singleton
    // ─────────────────────────────────────
    public static AudioManager instance;

    // ─────────────────────────────────────
    //  Audio Sources
    // ─────────────────────────────────────
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    // ─────────────────────────────────────
    //  Volume Settings
    // ─────────────────────────────────────
    [Header("Volume")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    // ─────────────────────────────────────
    //  Music Clips (assign in Inspector)
    // ─────────────────────────────────────
    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip level1Music;   // m7rg.mp3
    public AudioClip level2Music;   // manama.mp3
    public AudioClip level3Music;   // Qerqi3an.mp3
    public AudioClip level4Music;   // desert.mp3
    public AudioClip level5Music;   // amwaj
    public AudioClip level6Music;   // circuit.mp3
    public AudioClip victoryMusic;  // victory.mp3

    // ─────────────────────────────────────
    //  SFX Clips (assign in Inspector)
    // ─────────────────────────────────────
    [Header("Player SFX")]
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip slideSound;
    public AudioClip punchSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip footstepSound;

    [Header("Collectible SFX")]
    public AudioClip pearlCollectSound;
    public AudioClip lifePickupSound;
    public AudioClip powerUpSound;

    [Header("UI SFX")]
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    public AudioClip gameOverSound;
    public AudioClip levelCompleteSound;
    public AudioClip checkpointSound;

    // ─────────────────────────────────────
    //  Unity Lifecycle
    // ─────────────────────────────────────
    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetupAudioSources();
        LoadSavedVolumes();
    }

    // ─────────────────────────────────────
    //  Setup
    // ─────────────────────────────────────
    void SetupAudioSources()
    {
        // Create music source if not assigned
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        // Create SFX source if not assigned
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    void LoadSavedVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        UpdateVolumes();
    }

    // ─────────────────────────────────────
    //  Music Control
    // ─────────────────────────────────────
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: PlayMusic called with null clip!");
            return;
        }

        // Don't restart if already playing this clip
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume * masterVolume;
        musicSource.Play();
    }

    // Call this from each level scene
    // Example: AudioManager.instance.PlayLevelMusic(1);
    public void PlayLevelMusic(int levelIndex)
    {
        AudioClip clip = levelIndex switch
        {
            0 => menuMusic,
            1 => level1Music,
            2 => level2Music,
            3 => level3Music,
            4 => level4Music,
            5 => level5Music,
            6 => level6Music,
            7 => victoryMusic,
            _ => menuMusic
        };

        if (clip == null)
        {
            Debug.LogWarning($"AudioManager: No music clip assigned for level {levelIndex}");
            return;
        }

        PlayMusic(clip);
    }

    public void PlayVictoryMusic()
    {
        PlayMusic(victoryMusic);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    // ─────────────────────────────────────
    //  SFX Control
    // ─────────────────────────────────────

    // Main method - pass any AudioClip directly
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: PlaySFX called with null clip!");
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    // ─────────────────────────────────────
    //  Named SFX Shortcuts
    //  Call these from any script easily
    // ─────────────────────────────────────
    public void PlayJump() => PlaySFX(jumpSound);
    public void PlayLand() => PlaySFX(landSound);
    public void PlaySlide() => PlaySFX(slideSound);
    public void PlayPunch() => PlaySFX(punchSound);
    public void PlayHurt() => PlaySFX(hurtSound);
    public void PlayDeath() => PlaySFX(deathSound);
    public void PlayFootstep() => PlaySFX(footstepSound);

    public void PlayPearlCollect() => PlaySFX(pearlCollectSound);
    public void PlayLifePickup() => PlaySFX(lifePickupSound);
    public void PlayPowerUp() => PlaySFX(powerUpSound);

    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlayButtonHover() => PlaySFX(buttonHoverSound);
    public void PlayGameOver() => PlaySFX(gameOverSound);
    public void PlayLevelComplete() => PlaySFX(levelCompleteSound);
    public void PlayCheckpoint() => PlaySFX(checkpointSound);

    // ─────────────────────────────────────
    //  Volume Control
    // ─────────────────────────────────────
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        UpdateVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        UpdateVolumes();
    }

    void UpdateVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume * masterVolume;

        PlayerPrefs.Save();
    }

    // ─────────────────────────────────────
    //  Getters (for Settings UI sliders)
    // ─────────────────────────────────────
    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
}