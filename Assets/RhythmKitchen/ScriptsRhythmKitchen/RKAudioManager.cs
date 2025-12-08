using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class RKAudioManager : MonoBehaviour
{
    public static RKAudioManager Instance;

    public RKSound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

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
        }
    }

    private void Start()
    {
<<<<<<< HEAD
        sfxSource.ignoreListenerPause = true; // Allows for SFX to play even when the game is paused
=======
        PlayMusic("Ambience");
>>>>>>> 3430e29c05f0efb5e9595b287b669369c65a461a
    }

    public void PlayMusic(string name)
    {
        RKSound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log(name + " is not valid music. Does it exist?");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    
    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(string name)
    {
        RKSound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log(name + " is not a valid SFX. Does it exist?");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
}