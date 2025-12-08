using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class RKPauseMenuScript : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    void Start()
    {
        float volume;

        audioMixer.GetFloat("RKVolume", out volume);
        volumeSlider.SetValueWithoutNotify(volume);
    }

    public void SetVolume(float volume)
    {
        Debug.Log("volume: " + volume);
        audioMixer.SetFloat("RKVolume", volume);
    }

    public void BackMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        clickButton();

        SceneManager.LoadScene("RKMainMenu");
    }

    public void ResumeGameplay()
    {
<<<<<<< HEAD
        pausePanel.SetActive(false); // Makes the pausePanel inactive
=======
        Time.timeScale = 1f;
        AudioListener.pause = false;
>>>>>>> 3430e29c05f0efb5e9595b287b669369c65a461a
        
        clickButton();
    }

<<<<<<< HEAD
    // Countsdown to game resuming
    IEnumerator countdownStart()
    {
        int countdownLength = 3; // Length of the countdown.

        var am = RKAudioManager.Instance; // Current instance of the AudioManager             

        // loops every second for countdown length seconds, changing the countdownText accoridingly 
        while(countdownLength > 0)
        {
            if (am != null) // Checks if an AudioManager AudioManager instance exists
            {
                am.PlaySFX("CountDown"+countdownLength); // Plays CountDown Audio for countdownLength beat
                am.PlaySFX("CountVO"+countdownLength); // Plays CountVO Audio for countdownLength beat
            }
            else
                Debug.LogWarning("[RKPauseMenuScript] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning
            
            countdownText.text = "" + countdownLength; // Sets countdownText to countdownLength
            countdownLength--; // Subtracts 1 from countdownLength
            yield return new WaitForSeconds(1); // waits 1 second before beginning next loop
        }

        if (am != null) // Checks if an AudioManager AudioManager instance exists
        {
            am.PlaySFX("CountDownGo"); // Plays CountDown Audio
            am.PlaySFX("CountVOGo"); // Plays CountDown Audio
        }
        else
            Debug.LogWarning("[RKPauseMenuScript] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning
            
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
=======
>>>>>>> 3430e29c05f0efb5e9595b287b669369c65a461a
    private void clickButton()
    {
        var am = RKAudioManager.Instance;

        if (am != null)
            am.PlaySFX("ButtonPress");
        else
            Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway.");
    
    }
}
