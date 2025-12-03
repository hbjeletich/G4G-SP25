using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Worked on by: Leia Phillips
// Commented by: Leia Phillips
public class RKCreditsScript : MonoBehaviour
{
    //Script to go to the main menu
    public void BackMainMenu()
    {
        var am = RKAudioManager.Instance; // Current instance of the AudioManager

        if (am != null) //Checks if an AudioManager AudioManager instance exists
            am.PlaySFX("ButtonPress"); //Plays the ButtonPress sound
        else
            Debug.LogWarning("[RKSongSelectScript] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning

        SceneManager.LoadScene("RKMainMenu"); //Loads the MainMenu scene
    }
}
