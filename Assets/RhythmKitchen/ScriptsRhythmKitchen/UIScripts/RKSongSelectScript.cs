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

        if (am != null)
        {
            am.PlaySFX("ButtonPress");
            am.StopMusic();
        }
        else
            Debug.LogWarning("[RKSongSelectScript] AudioManager missing: loading scene anyway.");

        SceneManager.LoadScene(sceneName);
    }
}
