using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Worked on by: Leia Phillips
// Commented by: Leia Phillips
public class RKMainMenuScript : MonoBehaviour
{
    void Start()
    {
        RKAudioManager.Instance.PlayMusic("Ambience"); // Begins the Ambience background music
    }
    // Loads the SongSelectMenu scene, on Play button press in Unity
    public void PlayGame()
    {
        RKAudioManager.Instance.PlaySFX("ButtonPress"); // Plays ButtonPress sfx
        SceneManager.LoadScene("RKSongSelectMenu"); // Loads the SongSelectMenu scene
    }

    // Opens settings menu, on Settings button press in Unity
    public void OpenSettings()
    {
        RKAudioManager.Instance.PlaySFX("ButtonPress"); // Plays ButtonPress sfx
        // Enabling the actual menu happens in Unity
    }

    // Quits the game, on Quit button press in Unity
    public void QuitGame()
    {
        RKAudioManager.Instance.PlaySFX("ButtonPress"); // Plays ButtonPress sfx
        Application.Quit(); // Quits the Application
    }

    // Opens CreditsScene, on Credits button press in Unity
    public void LoadCredits()
    {
        RKAudioManager.Instance.PlaySFX("ButtonPress"); // Plays ButtonPress sfx
        SceneManager.LoadScene("RKCredits"); // Loads the Credits scene
    }
}
