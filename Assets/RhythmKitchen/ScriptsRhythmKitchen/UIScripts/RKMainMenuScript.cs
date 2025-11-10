using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RKMainMenuScript : MonoBehaviour
{
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
        RKAudioManager.Instance.PlaySFX("ButtonPress");
        Application.Quit();
    }

    public void LoadCredits()
    {
        RKAudioManager.Instance.PlaySFX("ButtonPress");
        //SceneManager.LoadScene("RKCredits");
    }
}
