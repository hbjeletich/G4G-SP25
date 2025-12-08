using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RKSongSelectScript : MonoBehaviour
{
    public void BackMainMenu()
    {
        var am = RKAudioManager.Instance;

        if (am != null)
            am.PlaySFX("ButtonPress");
        else
            Debug.LogWarning("[RKSongSelectScript] AudioManager missing: loading scene anyway.");
        SceneManager.LoadScene("RKMainMenu");
    }

    public void SelectSong(string sceneName)
    {
        var am = RKAudioManager.Instance;

<<<<<<< HEAD
            if (am != null) // Checks if an AudioManager AudioManager instance exists
            {
                am.PlaySFX("SongStartSFX"); // Plays the ButtonPress sound
                am.StopMusic(); // Stops the background music
            }
            else
                Debug.LogWarning("[RKSongSelectScript] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning
        
            SceneManager.LoadScene(sceneName); // Loads sceneName
=======
        if (am != null)
        {
            RKAudioManager.Instance.PlaySFX("RecordScratch");
            RKAudioManager.Instance.StopMusic();
>>>>>>> 3430e29c05f0efb5e9595b287b669369c65a461a
        }
        else
            Debug.LogWarning("[RKSongSelectScript] AudioManager missing: loading scene anyway.");

        SceneManager.LoadScene(sceneName);
    }
}
