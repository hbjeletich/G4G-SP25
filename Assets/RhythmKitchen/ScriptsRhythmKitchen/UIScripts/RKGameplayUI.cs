using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

// Worked on by: Leia Phillips
// Commented by: Leia Phillips
namespace RhythmKitchen
{
    public class RKGameplayUI : MonoBehaviour
    {
        [Header("Game Objects")]
        public GameObject completedDish; // The completedDish panel
        public GameObject gameplayUI; // The UI for the gameplay
        public GameObject judgementLine; // The judgement line
        public GameObject notesRuntime; // The ingredients when they spawn
        public GameObject outlines; // A reference to the outlines of the ingredients

        [Header("Refs")]
        public RKConductor conductor; // A reference to the conductor class
        public RKJudge judge; // A reference to the judge class
        public Slider slider; // A reference to the slider class
        public RKSongData songData; // A reference to the class

        [Header("Song Info")] // Comes from SongData
        private float songLength; // the length of the song
        private double startTime; // The dspTime for when the song starts
        private double endTime; // The dspTime for when the song ends

        void Awake()
        {
            startTime = songData.songStartDspTime; // gets the songStartDspTime from songData
            songLength = songData.songLength; // gets the songLength from songData
            endTime = startTime + songLength; // calculates the endTime based on dspTime
            slider.minValue = (float) startTime; // sets the minValue of the progess bar to the startTime
            slider.maxValue = (float) endTime; // sets the maxValue of the progess bar to the endTime
        }

        void Update()
        {
            // If the dspTime is at the endTime complete the dish
            if (endTime <= AudioSettings.dspTime)
            {
                Invoke("CompleteDish", .5f);
                Debug.Log("[RKGameplayUI] Dish Complete");
            }

            slider.value = (float)AudioSettings.dspTime; // Sets the slider value to the dspTime
        }

        /*
         * Loads the CompletedDishScene
         */
        public void CompleteDish()
        {
            // Checks if the AudioManager has been loaded, plays a sound if it has been.
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