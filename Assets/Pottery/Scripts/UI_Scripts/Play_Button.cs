using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings_Script : MonoBehaviour
{

    int progress = 0;

    public Slider slider;

    bool nextStage = false;


    public void playGame()
    {
        SceneManager.LoadScene("SelectionMenu");
        Debug.Log("Play Button Clicked");
    }

    public void startGame()
    {
        SceneManager.LoadScene("Gamestage_1");
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

    public void ButtonCheck()
    {
        Debug.Log("Button Clicked");
    }

    public void updateGamestate()
    {
       
            SceneManager.LoadScene("Gamestage_2");
            Debug.Log("Next Stage Loaded");
      
    }




    void update()
    {
        UpdateSlider();
        Debug.Log("progress: " + progress);
    }


    public void UpdateSlider()
    {
       if (Input.GetKey(KeyCode.Space))
        {
            progress++;
            slider.value = progress;

            Debug.Log("Space Key Pressed");
        }

       if (slider.value == 10)
        {
            Debug.Log("Slider Full, nextStage = true");
            nextStage = true;
        }
    }

   

}


    
  

