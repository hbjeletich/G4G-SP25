using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RKCompletedDishScript : MonoBehaviour
{
    void Start()
    {
        var am = RKAudioManager.Instance;

        if (am != null)
            am.PlayMusic("Ambience");
        else
            Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway.");
    }
    
    public void BackMainMenu()
    {
        var am = RKAudioManager.Instance;

        if (am != null)
            am.PlaySFX("ButtonPress");
        else
            Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway.");
        
        SceneManager.LoadScene("RKMainMenu");
    }

    public void GoToSongSelect()
    {
        var am = RKAudioManager.Instance;

        if (am != null)
            am.PlaySFX("ButtonPress");
        else
            Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway.");

        SceneManager.LoadScene("RKSongSelectMenu");
    }

}

