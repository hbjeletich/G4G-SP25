using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace RhythmKitchen
{
    public class RKCompletedDishScript : MonoBehaviour
    {
<<<<<<< HEAD
        [Header("UI elements")] // Set in Unity
        public GameObject[] stars;
        public GameObject[] greyStars;
=======
        public Image[] stars;
>>>>>>> 3430e29c05f0efb5e9595b287b669369c65a461a
        public TMP_Text dishNameText;
        public TMP_Text comboText;
        public TMP_Text perfectText;
        public TMP_Text goodText;
        public TMP_Text almostText;
        public RKSongData songData;

        void Start()
        {
            var am = RKAudioManager.Instance;

            if (am != null)
                am.PlayMusic("Ambience");
            else
                Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway.");

            dishNameText.text = songData.dishName;
        }

        public void setStars(int rating)
        {
<<<<<<< HEAD
            stars[0].SetActive(true);
            greyStars[0].SetActive(false);

            if (rating == 3)
            {
                stars[1].SetActive(true);
                stars[2].SetActive(true);
                greyStars[1].SetActive(false);
                greyStars[2].SetActive(false);
            }
            else if (rating == 2)
            {
                stars[1].SetActive(true);
                stars[2].SetActive(false);
                greyStars[1].SetActive(false);
                greyStars[2].SetActive(true);
            }
            else
            {
                stars[1].SetActive(false);
                stars[2].SetActive(false);
                greyStars[1].SetActive(true);
                greyStars[2].SetActive(true);
            }
=======
            if (rating < 3)
                stars[2].color = Color.black;
            else if (rating < 2)
                stars[1].color = Color.black;
>>>>>>> 3430e29c05f0efb5e9595b287b669369c65a461a
        }

        public void BackMainMenu()
        {
            Time.timeScale = 1f;

            var am = RKAudioManager.Instance;

            if (am != null)
                am.PlaySFX("ButtonPress");
            else
                Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway.");

            SceneManager.LoadScene("RKMainMenu");
        }

        public void GoToSongSelect()
        {
            Time.timeScale = 1f;

            var am = RKAudioManager.Instance;

            if (am != null)
                am.PlaySFX("ButtonPress");
            else
                Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway.");

            SceneManager.LoadScene("RKSongSelectMenu");
        }

    }

}