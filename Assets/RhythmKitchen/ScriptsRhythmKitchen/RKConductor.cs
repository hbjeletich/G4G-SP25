using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Worked on by: Jovanna Molina
// Commented by: Jovanna Molina

namespace RhythmKitchen
{ public class RKConductor : MonoBehaviour
    {
        public RKSongData songData;

        [Header("Music Settings")]
        public AudioSource musicSource;
        private float BPM; // BPM of tutorial song specifically
        private float offsetMs; // this is calibration to nudge timing if it feels early/late (positive = judge later, negative = judge earlier)
        private double leadInSeconds; // this is a small delay so we can schedule precisely
        public float songLength; // this is length of the song
        private float secondsPerBeat; // this is 60/BPM
        private float[] songBeats; // this is the beat position
        public float curSongBeat => secondsPerBeat > 0f ? songTime / secondsPerBeat : 0f; // this is the beat position
        public float songTime { get; private set; } // this is seconds since scheduled start (+ offset)
        
        private float _nextLog; // this is throttle so logs print ~1/sec 
        private double songStartDspTime; // this is the DSP timestamp when the song will start

        void Start()
        {
            BPM = songData.bpm;
            offsetMs = songData.offsetMs;
            leadInSeconds = songData.leadInSeconds;
            songLength = songData.songLength;
            secondsPerBeat = songData.secondsPerBeat;
            songBeats = songData.songBeats;
            songStartDspTime = songData.songStartDspTime; 

            if (!musicSource || !musicSource.clip) // this is a safety check for missing references
            {
                Debug.LogError("[Conductor] Missing AudioSource or clip.");
                enabled = false;
                return;
            }

            musicSource.playOnAwake = false; // this is to avoid auto-playing before we're ready
            musicSource.loop = false; // this is off for the prototype

            songStartDspTime = AudioSettings.dspTime + leadInSeconds; // this is the exact start time on DSP clock
            // formatted like this bc this is saying "start the song X seconds from now, not immediately
            musicSource.PlayScheduled(songStartDspTime); // this is precise scheduled playback
            Debug.Log($"[Conductor] Scheduled '{musicSource.clip.name}' @ dsp={songStartDspTime:F3}"); // this shows which song was scheduled and at what DSP time
        }

        void Update()
        {
            double dspNow = AudioSettings.dspTime; // this is the current precise audio clock
            songTime = (float)(dspNow - songStartDspTime) + (offsetMs / 1000f); // this is elapsed + calibration

            if (songTime >= _nextLog) // logs once per sec to make sure time and beat calculations stay in sync
            {
                Debug.Log($"[Conductor] t={songTime:0.000}s beat={curSongBeat:0,00}"); // every sec, log prints the current song and beat count
                _nextLog += 1; // schedule next log 1 sec later
            }
        }
    }
}

