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
        public Image[] stars;
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
            if (rating < 3)
                stars[2].color = Color.black;
            else if (rating < 2)
                stars[1].color = Color.black;
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