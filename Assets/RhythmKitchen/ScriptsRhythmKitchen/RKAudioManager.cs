using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;

// Worked on by: Leia Phillips
// Commented by: Leia Phillips
public class RKAudioManager : MonoBehaviour
{
    public static RKAudioManager Instance;

    public RKSound[] musicSounds, sfxSounds; // Arrays containing the name and AudioClip of all of the sounds
    public AudioSource musicSource, sfxSource; // The sources for the music and sfx

    private void Awake()
    {
        // Creates the Instance so we only need the audio manager on the main menu screen
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
        sfxSource.ignoreListenerPause = true; // Allows for SFX to play even when the game is paused
    }

/*

This section was added by Helena to work with the Hub & the rest of the games!

Destroys AudioManager & stops music when switching to NewGameSelect scene.

*/

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "NewGameSelect")
        {
            musicSource.Stop();
            sfxSource.Stop();
            Destroy(gameObject);
        }
    }

/*

End of Helena's work.

*/

    /*
     * Plays a sound based on it's name to musicSource.
     * 
     * @param name The name of the sound
     */
    public void PlayMusic(string name)
    {
        RKSound s = Array.Find(musicSounds, x => x.name == name); // Finds the RKSound based on the sound's name

        if (s == null) // Checking if the sound exists
        {
            Debug.Log(name + " is not valid music. Does it exist?");
        }
        else
        {
            // Sets the music clip to the sounds AudioClip and the plays the sound
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    
    /*
     * Stop the currently playing sound on musicSource
     */
    public void StopMusic()
    {
        musicSource.Stop();
    }

    /*
     * Plays a one shot sound based on it's name to sfxSource.
     * 
     * @param name The name of the sound
     */
    public void PlaySFX(string name)
    {
        RKSound s = Array.Find(sfxSounds, x => x.name == name); // Finds the RKSound based on the sound's name

        if (s == null) // Checking if the sound exists
        {
            Debug.Log(name + " is not a valid SFX. Does it exist?");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip); //Plays the AudioClip as a one shot
        }
    }
}