using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RKMainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("RKSongSelectMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadCredits()
    {
        //SceneManager.LoadScene("RKCredits");
    }
}
