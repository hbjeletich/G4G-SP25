using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings_Script : MonoBehaviour
{
   public void playGame()
    {
        SceneManager.LoadScene("SelectionMenu");
        Debug.Log("Play Button Clicked");
    }


    public void backScreen()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Back Button Clicked");
    }


    public void settingsScreen()
    {
        SceneManager.LoadScene("SettingsMenu");
        Debug.Log("Settings Button Clicked");
    }


    public void galleryScreen()
    {
        SceneManager.LoadScene("GalleryMenu");
        Debug.Log("Galary Button Clicked");
    }

    
}
