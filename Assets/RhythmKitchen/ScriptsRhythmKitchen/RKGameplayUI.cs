using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RhythmKitchen
{
    public class RKGameplayUI : MonoBehaviour
    {
        public Slider slider;
        public RKSongData songData;

        private float songLength;

        private float songTime;

        private float fillSpeed;
        private float targetProgress = 0;
        private float _nextLog;
        private float startTime;

        void Awake()
        {
            songLength = songData.songLength;
            slider.maxValue = songLength;
            startTime = Time.time;
        }

        void Update()
        {
            float time = Time.time - startTime;

            if (time >= songLength)
            {
                Invoke("CompleteDish", .5f);
            }

            slider.value = time;
        }

        public void CompleteDish()
        {
            var am = RKAudioManager.Instance;
            {
                if (am != null) // is not empty
                {
                    am.PlaySFX("Shimmer");
                }
                else
                {
                    Debug.LogWarning("[RKGameplayUI] AudioManager missing: loading scene anyway.");
                }
                SceneManager.LoadScene("RKCompletedDish");
            }
        }
    }
}