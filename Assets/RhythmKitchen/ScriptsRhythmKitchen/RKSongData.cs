using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace RhythmKitchen
{
    public class RKSongData : MonoBehaviour
    {
        public float bpm; // BPM of song
        public float travelTime; // how long it takes the beat to travel from spawn point to hit lane
        public float leadInSeconds; // this is a small delay so we can schedule precisely
        public float offsetMs; // this is calibration to nudge timing if it feels early/late (positive = judge later, negative = judge earlier)
        public float songLength; // the length on seconds of the song
        public string songBeatString;


        public float[] songBeats { get; private set; } // the beats of the inputs
        public double songStartDspTime{ get; private set; } // this is the DSP timestamp when the song will start
        public float secondsPerBeat { get; private set; } // this is 60/BPM
        public double timeAdjustment { get; private set; } // the time adjustment from when scene starts when the note should spawn to the hit lane
        public double[] spawnTimes { get; private set; } // the spawn timings for each song beat
                                                         // songBeats[index]*secondsPerBeat+timeAdjustment = time in seconds when to spawn songBeats[index]

        void Awake()
        {
            string[] tempSongBeats = songBeatString.Replace(" ", "").Split(","); //Removes spaces from the songBeatString and splits at commas to create a string array
            songBeats = new float[tempSongBeats.Length]; // Setting songBeats to an empty array
            spawnTimes = new double[tempSongBeats.Length]; // Setting spawnTimes to an empty array

            // populates songBeats with float values of tempSongBeats
            for (int i = 0; i < tempSongBeats.Length; i++)
            {
                songBeats[i] = float.Parse(tempSongBeats[i]);
                Debug.Log($"[SongData] Song Beat [{i}] = {songBeats[i]}");
            }
            
            secondsPerBeat = 60f / Mathf.Max(1f, bpm); // this is clamp to avoid divided-by-zero
            Debug.Log($"[SongData] BPM={bpm} spb={secondsPerBeat:F3}"); // current song is BPM and each beat lasts X secs

            songStartDspTime = AudioSettings.dspTime + leadInSeconds;

            timeAdjustment = offsetMs - travelTime + songStartDspTime; // Might not need songStartDspTime
            Debug.Log($"[SongData] Time Adjustment = {timeAdjustment}");

            for (int i = 0; i < songBeats.Length; i++)
            {
                spawnTimes[i] = songBeats[i] * secondsPerBeat + timeAdjustment; // populates spawn times with the spawn timing of each beat
                Debug.Log($"[SongData] Spawn Time [{i}] = {spawnTimes[i]}");
            }
        }
    }
}