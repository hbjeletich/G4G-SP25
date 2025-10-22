using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings_Script : MonoBehaviour
{

    public float progress = 0;

    public Slider slider;

    public float fillspeed;

    bool nextStage = false;

    public Camera captureCamera; // assign your capture cam in the Inspector

    public int resolution = 512; // 512x512 is fine for thumbnails

    //public void takeScreenshot(string fileName)
    //{
    //    RenderTexture rt = new RenderTexture(resolution, resolution, 24);
    //    captureCamera.targetTexture = rt;

    //    Texture2D screenshot = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);
    //    captureCamera.Render();

    //    RenderTexture.active = rt;
    //    screenshot.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
    //    screenshot.Apply();

    //    captureCamera.targetTexture = null;
    //    RenderTexture.active = null;
    //    Destroy(rt);

    //    byte[] bytes = screenshot.EncodeToPNG();
    //    string path = Path.Combine(Application.persistentDataPath, fileName + ".png");
    //    File.WriteAllBytes(path, bytes);

    //    Debug.Log($"Saved screenshot: {path}");
    //}

    //public void gamefinish()
    //{
    //    //FindObjectOfType<PotteryScreenshot>().CaptureAndSave("pottery_" + System.DateTime.Now.ToFileTime());
    //}

    public void playGame()
    {
        SceneManager.LoadScene("SelectionMenu");
        Debug.Log("Play Button Clicked");
    }

    public void confirmscreen()
    {
        SceneManager.LoadScene("ConfirmationScreen");
        Debug.Log("Confirm Screen loaded");
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




    void Update()
    {
        UpdateSlider();
        //Debug.Log("progress: " + progress);
        //gamefinish();

    }


    public void UpdateSlider()
    {
       if (Input.GetKeyDown(KeyCode.Space))
        {
            progress ++;
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


    
  

