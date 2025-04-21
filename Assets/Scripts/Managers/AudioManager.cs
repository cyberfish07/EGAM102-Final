using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX")]
    public Sound[] sounds;

    [Header("BGM")]
    public Sound backgroundMusic;
    public float musicFadeTime = 1f;

    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();

    void Awake()
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

        foreach (Sound s in sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            s.source = source;

            source.clip = s.clip;
            source.volume = s.volume;
            source.pitch = s.pitch;
            source.loop = s.loop;

            soundDictionary[s.name] = s;
        }

        if (backgroundMusic.clip != null)
        {
            AudioSource musicSource = gameObject.AddComponent<AudioSource>();
            backgroundMusic.source = musicSource;

            musicSource.clip = backgroundMusic.clip;
            musicSource.volume = backgroundMusic.volume;
            musicSource.pitch = backgroundMusic.pitch;
            musicSource.loop = true;

            StartBackgroundMusic();
        }
    }

    public void StartBackgroundMusic()
    {
        if (backgroundMusic.source != null && !backgroundMusic.source.isPlaying)
        {
            backgroundMusic.source.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (backgroundMusic.source != null && backgroundMusic.source.isPlaying)
        {
            backgroundMusic.source.Stop();
        }
    }

    public void PlaySound(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }

    public void StopSound(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            s.source.Stop();
        }
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        if (backgroundMusic.source != null)
        {
            backgroundMusic.source.volume = volume * backgroundMusic.volume;
        }
    }

    public void SetSoundVolume(float volume)
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null)
            {
                s.source.volume = volume * s.volume;
            }
        }
    }
    public void PauseBackgroundMusic()
    {
        if (backgroundMusic.source != null && backgroundMusic.source.isPlaying)
        {
            backgroundMusic.source.Pause();
        }
    }

    public void ResumeBackgroundMusic()
    {
        if (backgroundMusic.source != null && !backgroundMusic.source.isPlaying)
        {
            backgroundMusic.source.UnPause();
        }
    }
}