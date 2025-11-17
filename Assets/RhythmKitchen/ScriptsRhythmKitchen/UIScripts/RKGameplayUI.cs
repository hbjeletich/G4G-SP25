using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

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
        public RKJudge judge;

        public Slider slider;
        public RKSongData songData;

        private float songLength;

        private float songTime;

        private float fillSpeed;
        private float targetProgress = 0;
        private float _nextLog;
        private double endTime;
        private double startTime;

        void Awake()
        {
            startTime = songData.songStartDspTime;
            songLength = songData.songLength;
            endTime = startTime + songLength;
            slider.minValue = (float) startTime;
            slider.maxValue = (float) endTime;
        }

        void Update()
        {
            if (endTime <= AudioSettings.dspTime)
            {
                Invoke("CompleteDish", .5f);
                Debug.Log("COMPLETETETETETE");
            }

            slider.value = (float)AudioSettings.dspTime;
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

            judge.starScore();

            conductor.musicSource.Stop();
        }

        public void Pause()
        {
            clickButton();

            AudioListener.pause = true;

            var am = RKAudioManager.Instance;
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