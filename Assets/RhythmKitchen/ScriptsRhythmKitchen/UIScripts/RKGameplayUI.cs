using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RhythmKitchen
{
    public class RKGameplayUI : MonoBehaviour
    {
        public GameObject completedDish;
        public GameObject gameplayUI;
        public GameObject judgementLine;
        public GameObject notesRuntime;
        public GameObject outlines;

        public RKConductor conductor;

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
            }

            gameplayUI.SetActive(false);
            judgementLine.SetActive(false);
            notesRuntime.SetActive(false);
            outlines.SetActive(false);
            completedDish.SetActive(true);

            Time.timeScale = 0f;
            conductor.musicSource.Stop();
        }

        public void Pause()
        {
            clickButton();

            judgementLine.SetActive(false);

            AudioListener.pause = true;
            Time.timeScale = 0f;
        }

        private void clickButton()
    {
        var am = RKAudioManager.Instance;

        if (am != null)
            am.PlaySFX("ButtonPress");
        else
            Debug.LogWarning("[RKCompletedDishScript] AudioManager missing: loading scene anyway.");
    
    }
    }
}