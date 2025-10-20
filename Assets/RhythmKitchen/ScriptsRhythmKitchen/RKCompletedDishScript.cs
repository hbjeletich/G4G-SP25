using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RKCompletedDishScript : MonoBehaviour
{
    void Start()
    {
        RKAudioManager.Instance.PlayMusic("Ambience");
    }
    
    public void BackMainMenu()
    {
        RKAudioManager.Instance.PlaySFX("ButtonPress");
        SceneManager.LoadScene("RKMainMenu");
    }

    public void GoToSongSelect()
    {
        RKAudioManager.Instance.PlaySFX("ButtonPress");
        SceneManager.LoadScene("RKSongSelectMenu");
    }

}

