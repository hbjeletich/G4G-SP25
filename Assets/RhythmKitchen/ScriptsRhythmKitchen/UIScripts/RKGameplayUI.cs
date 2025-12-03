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
        public GameObject completedDish; // The completedDish panel, set in Unity
        public GameObject gameplayUI; // The UI for the gameplay, set in Unity
        public GameObject judgementLine; // The judgement line, set in Unity
        public GameObject notesRuntime; // The ingredients when they spawn, set in Unity
        public GameObject outlines; // A reference to the outlines of the ingredients, set in Unity

        [Header("Refs")]
        public RKConductor conductor; // A reference to the conductor class, set in Unity
        public RKJudge judge; // A reference to the judge class, set in Unity
        public Slider slider; // A reference to the slider class, set in Unity
        public RKSongData songData; // A reference to the class, set in Unity

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
                Invoke("CompleteDish", .25f); // calls CompleteDish after .25 seconds
                Debug.Log("[RKGameplayUI] Dish Complete");
            }

            slider.value = (float)AudioSettings.dspTime; // Sets the slider value to the dspTime
        }

        // Loads the CompletedDish panel
        public void CompleteDish()
        {
            var am = RKAudioManager.Instance; // Current instance of the AudioManager
            {
                if (am != null) // Checks if an AudioManager AudioManager instance exists
                {
                    am.PlaySFX("Shimmer"); // Plays Shimmer sfx
                }
                else
                {
                    Debug.LogWarning("[RKGameplayUI] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning
                }
            }

            // Sets gameplay objects to inactive
            gameplayUI.SetActive(false);
            judgementLine.SetActive(false);
            notesRuntime.SetActive(false);
            outlines.SetActive(false);

            // Sets CompletedDish assets to active
            completedDish.SetActive(true);

            judge.starScore(); // calculates the starScore for the playthrough

            conductor.musicSource.Stop(); // Stops the music, so ambient music can be played latee
        }

        // Pauses the game
        public void Pause()
        {
            var am = RKAudioManager.Instance; // Current instance of the AudioManager

            if (am != null) // Checks if an AudioManager AudioManager instance exists
                am.PlaySFX("ButtonPress"); // Plays the ButtonPress sound
            else
                Debug.LogWarning("[RKGameplayUI] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning

            AudioListener.pause = true;  // Pauses AudioListener, This is what actually pauses gameplay
        }
    }
}