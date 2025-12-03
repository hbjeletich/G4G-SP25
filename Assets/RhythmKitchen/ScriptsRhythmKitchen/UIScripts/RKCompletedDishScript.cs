using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

// Worked on by: Leia Phillips
// Commented by: Leia Phillips
namespace RhythmKitchen
{
    public class RKCompletedDishScript : MonoBehaviour
    {
        [Header("UI elements")] // Set in Unity
        public GameObject[] stars;
        public TMP_Text dishNameText;
        public TMP_Text comboText;
        public TMP_Text perfectText;
        public TMP_Text goodText;
        public TMP_Text almostText;

        [Header("Refs")] // Set in Unity
        public RKSongData songData;

        void Start()
        {
            var am = RKAudioManager.Instance; // Current instance of the AudioManager

            if (am != null) // Checks if an AudioManager AudioManager instance exists
                am.PlayMusic("Ambience"); // Begins the Ambience background music
            else
                Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning

            dishNameText.text = songData.dishName; // Sets the dishNameText object to the dishName that was set in songData
        }

        // Sets the comboText object to num
        public void setComboText(int num)
        {
            comboText.text = "" + num;
        }

        // Sets the perfectText object to num
        public void setPerfectText(int num)
        {
            perfectText.text = "" + num;
        }

        // Sets the goodText object to num
        public void setGoodText(int num)
        {
            goodText.text = "" + num;
        }
        
        // Sets the almostText object to num
        public void setAlmostText (int num)
        {
            almostText.text = "" + num;
        }

        // changes the star images to black based on rating
        public void setStars(int rating)
        {
            if (rating < 3)
                stars[2].SetActive(false);
            if (rating < 2)
                stars[1].SetActive(false);
        }

        // Loads the MainMenu scene, on MainMenu button press
        public void BackMainMenu()
        {
            var am = RKAudioManager.Instance; // Current instance of the AudioManager

            if (am != null) // Checks if an AudioManager AudioManager instance exists
                am.PlaySFX("ButtonPress"); // Plays the ButtonPress sound
            else
                Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning

            SceneManager.LoadScene("RKMainMenu"); // Loads the MainMenu scene
        }

        // Loads the SongSelect scene, on MainMenu button press
        public void GoToSongSelect()
        {
            var am = RKAudioManager.Instance; // Current instance of the AudioManager

            if (am != null) // Checks if an AudioManager AudioManager instance exists
                am.PlaySFX("ButtonPress"); // Plays the ButtonPress sound
            else
                Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning

            SceneManager.LoadScene("RKSongSelectMenu"); // Loads the SongSelect scene
        }

    }

}