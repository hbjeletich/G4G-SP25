using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenController : MonoBehaviour
{
    public Image image;
    public TMP_Text text;

    // Arrays of images and text objects to cycle through
    public Sprite[] images;
    public string[] texts;

    public Button nextButton; 
    public Button playGameButton;

    private int currentScreen = 0;

    // Function to go to the next screen
    public void NextScreen()
    {
        // Only move forward if it is not already at the last screen
        if (currentScreen < texts.Length - 1)
        {
            currentScreen++;
            UpdateScreen();
        }
    }

    // Function to go to the previous screen
    public void PreviousScreen()
    {
        // Only move backward if it is not already at the first screen
        if (currentScreen > 0)
        {
            currentScreen--;
            UpdateScreen();
        }
    }

    // Updates the UI elements (image, text, buttons) according to currentScreen
    void UpdateScreen()
    {
        image.sprite = images[currentScreen];
        text.text = texts[currentScreen];

        // Display the button that enters game play only if it is at the last screen
        if (currentScreen == texts.Length - 1) 
        {
            nextButton.gameObject.SetActive(false);
            playGameButton.gameObject.SetActive(true); 
        }
        else
        {
            nextButton.gameObject.SetActive(true);
            playGameButton.gameObject.SetActive(false);
        }
    }
}
