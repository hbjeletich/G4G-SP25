using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RKMainMenuScript : MonoBehaviour
{
<<<<<<< HEAD
    void Start()
    {
        RKAudioManager.Instance.PlayMusic("Ambience"); // Begins the Ambience background music
    }
    // Loads the SongSelectMenu scene, on Play button press in Unity
=======
>>>>>>> 3430e29c05f0efb5e9595b287b669369c65a461a
    public void PlayGame()
    {
        RKAudioManager.Instance.PlaySFX("ButtonPress");
        SceneManager.LoadScene("RKSongSelectMenu");
    }

    public void OpenSettings()
    {
        RKAudioManager.Instance.PlaySFX("ButtonPress");
    }

    public void QuitGame()
    {
<<<<<<< HEAD
        RKAudioManager.Instance.PlaySFX("ButtonPress"); // Plays ButtonPress sfx
        // Application.Quit(); // Quits the Application
=======
        RKAudioManager.Instance.PlaySFX("ButtonPress");
        Application.Quit();
>>>>>>> 3430e29c05f0efb5e9595b287b669369c65a461a
    }

    public void LoadCredits()
    {
        RKAudioManager.Instance.PlaySFX("ButtonPress");
        //SceneManager.LoadScene("RKCredits");
    }
}
