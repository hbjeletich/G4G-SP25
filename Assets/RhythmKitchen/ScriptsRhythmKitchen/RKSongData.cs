using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmKitchen
{
    public class RKSongData : MonoBehaviour
    {
        [SerializeField] public float bpm; // BPM of song
        [SerializeField] public float travelTime; // how long it takes the beat to travel from spawn point to hit lane
        [SerializeField] public float leadInSeconds; // this is a small delay so we can schedule precisely
        [SerializeField] public float offsetMs; // this is calibration to nudge timing if it feels early/late (positive = judge later, negative = judge earlier)
        [SerializeField] public float songTime; // the length on seconds of the song
        [SerializeField] public double songStartDspTime; // this is the DSP timestamp when the song will start
        [SerializeField] public float[] songBeats; // the beats of the inputs

        public float secondsPerBeat { get; private set; } // this is 60/BPM
        public float timeAdjustment { get; private set; } // the time adjustment from when scene starts when the note should spawn to the hit lane
        public float[] spawnTimes { get; private set; } // the spawn timings for each song beat
                                                        // songBeats[index]*secondsPerBeat+timeAdjustment = time in seconds when to spawn songBeats[index]

        void Awake() // might need to be start, or Init or some other way of setting the variables
        {
            secondsPerBeat = 60f / Mathf.Max(1f, BPM); // this is clamp to avoid divided-by-zero
            Debug.Log($"[Conductor] BPM={BPM} spb={secondsPerBeat:F3}"); // current song is BPM and each beat lasts X secs

            timeAdjustment = offsetMs - leadInSeconds - travelTime + songStartDspTime; // Might not need songStartDspTime

            for (int i = 0; i <= songBeats.Length; i++)
            {
                spawnTimes[i] = songBeats[i] * secondsPerBeat + timeAdjustment; // populates spawn times with the spawn timing of each beat
            }
        }
    }
}