using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Worked on by: Leia Phillips
// Commented by: Leia Phillips
public class RKSongSelectScript : MonoBehaviour
{
    // Loads the MainMenu scene, on Back button press
    public void BackMainMenu()
    {
        var am = RKAudioManager.Instance; // Current instance of the AudioManager

        if (am != null) // Checks if an AudioManager AudioManager instance exists
            am.PlaySFX("ButtonPress"); // Plays the ButtonPress sound
        else
            Debug.LogWarning("[RKSongSelectScript] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning

        SceneManager.LoadScene("RKMainMenu"); // Loads the MainMenu scene
    }

    // Loads the song scene, on song button press in Unity
    public void SelectSong(string sceneName)
    {
        if(SceneUtility.GetBuildIndexByScenePath(sceneName) >= 0) // Checks if sceneName is in build index
        {
            var am = RKAudioManager.Instance; // Current instance of the AudioManager

            if (am != null) // Checks if an AudioManager AudioManager instance exists
            {
                am.PlaySFX("SongStartSFX"); // Plays the ButtonPress sound
                am.StopMusic(); // Stops the background music
            }
            else
                Debug.LogWarning("[RKSongSelectScript] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning
        
            SceneManager.LoadScene(sceneName); // Loads sceneName
        }
        else
            Debug.LogWarning("[RKSongSelectScript] " + sceneName + " does not exist."); // If sceneName does not exist it logs a warning
    }
}
