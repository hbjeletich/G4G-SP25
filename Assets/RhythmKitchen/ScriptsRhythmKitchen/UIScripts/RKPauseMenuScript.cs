using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Worked on by: Leia Phillips
// Commented by: Leia Phillips
public class RKPauseMenuScript : MonoBehaviour
{
    [Header("Sound")]
    public AudioMixer audioMixer; // The AudioMixer, set in Unity
    public Slider volumeSlider; // The volume Slider, set in Unity

    [Header("Pause Menu References")]
    public TMP_Text countdownText; // The TMP_Text object for the countdown, set in Unity
    public GameObject pausePanel; // The GameObject for the pausePanel, set in Unity
    public GameObject judgementLine; // The GameObject for the judgementLine, set in Unity
    public GameObject outlines; // The GameObject for the outlines, set in Unity
    public GameObject pauseMenu; // The GameObject for the pauseMenu, set in Unity
    public GameObject pauseButton; // The GameObject for the pauseButton, set in Unity
    public GameObject notesRuntime; // The GameObject for notesRuntime, set in Unity

    void Start()
    {
        float volume; // the volume

        audioMixer.GetFloat("RKVolume", out volume); // sets variable volume to the current volume value of the audioMixer
        volumeSlider.SetValueWithoutNotify(volume); // sets the slider value to volume, this makes the slider value always correct even after changing scenes
    }

    // Sets the game volume
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("RKVolume", volume); // sets audioMixer volume to variable volume
    }

    public void BackMainMenu()
    {
        clickButton(); // Plays ButtonPress sfx

        AudioListener.pause = false; // Unpauses AudioListener, AudioListener must be paused for game to pause

        SceneManager.LoadScene("RKMainMenu"); // Loads the MainMenu scene
    }

    public void ResumeGameplay()
    {
        clickButton(); // Plays ButtonPress sfx

        pausePanel.SetActive(false); // Makes the pausePanel inactive
        
        StartCoroutine(countdownStart()); // starts the countdown to resuming the game
    }

    // Countsdown to game resuming
    IEnumerator countdownStart()
    {
        int countdownLength = 3; // Length of the countdown.

        // loops every second for countdown length seconds, changing the countdownText accoridingly 
        while(countdownLength > 0)
        {
            countdownText.text = "" + countdownLength; // Sets countdownText to countdownLength
            countdownLength--; // Subtracts 1 from countdownLength
            yield return new WaitForSeconds(1); // waits 1 second before beginning next loop
        }

        countdownText.text = "Go!"; // Sets countdownText to "Go!"
        yield return new WaitForSeconds(1); // Waits 1 second before continuing code

        // Sets the gameplay objects to active
        judgementLine.SetActive(true);
        outlines.SetActive(true);
        pausePanel.SetActive(true);
        notesRuntime.SetActive(true); 
        pauseButton.SetActive(true);

        // Sets the pauseMenu objects to inactive
        pauseMenu.SetActive(false);

        AudioListener.pause = false; // unpauses the AudioListener, resumes gameplay
    }

    // Since button clicks happen more than once, this method was created
    // Plays the PressButton sfx if there is an instance of the AudioManager 
    private void clickButton()
    {
        var am = RKAudioManager.Instance; // Current instance of the AudioManager

        if (am != null) // Checks if an AudioManager AudioManager instance exists
            am.PlaySFX("ButtonPress"); // Plays the ButtonPress sound
        else
            Debug.LogWarning("[RKPauseMenuScript] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning
    
    }
}
