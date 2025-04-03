using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer audioMixer;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    public AudioClip defaultMusic; // Музыка по умолчанию для первой сцены

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadVolumeSettings();

        if (defaultMusic != null)
        {
            PlayMusic(defaultMusic);
        }

        // Подписываемся на смену сцены
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeMusicForScene(scene.name);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.isPlaying && musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }


    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayUI(AudioClip clip)
    {
        uiSource.PlayOneShot(clip);
    }

    public void SetVolume(string parameter, float volume)
    {
        if (volume == 0f)
        {
            audioMixer.SetFloat(parameter, -80f);
        }
        else
        {
            audioMixer.SetFloat(parameter, Mathf.Log10(volume) * 20);
        }
    }

    private void LoadVolumeSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float uiVolume = PlayerPrefs.GetFloat("UIVolume", 1f);

        SetVolume("Volume_Music", musicVolume);
        SetVolume("Volume_SFX", sfxVolume);
        SetVolume("Volume_UI", uiVolume);
    }

    private void ChangeMusicForScene(string sceneName)
    {
        AudioClip newMusic = null;

        switch (sceneName)
        {
            case "MainMenu":
                newMusic = Resources.Load<AudioClip>("Music/MainMenuMusic");
                break;
            case "MainRoom":
                newMusic = Resources.Load<AudioClip>("Music/MainRoomMusic");
                break;
            case "Forge":
                newMusic = Resources.Load<AudioClip>("Music/ForgeMusic");
                break;
            case "Storage":
                newMusic = Resources.Load<AudioClip>("Music/StorageMusic");
                break;
        }

        if (newMusic != null)
        {
            PlayMusic(newMusic);
        }
    }
}
