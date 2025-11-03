using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Worked on by: Jovanna Molina and Leia Phillips 
// Commented by: Jovanna Molina

namespace RhythmKitchen
{
    public class RKSongData : MonoBehaviour
    {
        [Header("Authoring")]
        public float bpm; // BPM of song
        public float travelTime = 1f; // how long it takes the beat to travel from spawn point to hit lane
        public float leadInSeconds = 1f; // this is a small delay so we can schedule precisely
        public float offsetMs = 0f; // this is calibration to nudge timing if it feels early/late (positive = judge later, negative = judge earlier)
        public float songLength; // the length on seconds of the song
        [TextArea] public string songBeatString;
        [TextArea] public string chartString; // comma-separated list of note types (1,2,3,4)

        // computed and consumed by other scripts
        public float[] songBeats { get; private set; } // the beats of the input
        public float[] targetTimes { get; private set; } // SONG-RELATIVE seconds when notes hit the line
        public double[] spawnTimes { get; private set; } // dsp times when to instantiate each note
        public float secondsPerBeat { get; private set; } // this is 60/BPM
        public double songStartDspTime { get; private set; } // this is the DSP timestamp when the song will start

        public RKNote.Type[] chartByBeat { get; private set; } // the note type per beat parsed from chartString

        //public double timeAdjustment { get; private set; } // the time adjustment from when scene starts when the note should spawn to the hit lane
        // songBeats[index]*secondsPerBeat+timeAdjustment = time in seconds when to spawn songBeats[index]

        // leia's awake
        /*void Awake()
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
        } */

        void Awake()
        {
            // this is the base timing
            secondsPerBeat = 60f / Mathf.Max(1f, bpm); // this just avoids divide by zero
            songStartDspTime = AudioSettings.dspTime + leadInSeconds; // when the AudioSource will start

            // this parses the beat list 
            var parts = songBeatString.Replace(" ", "").Split(',');
            songBeats = new float[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                songBeats[i] = float.Parse(parts[i])-1; //subtracting one so the first beat would be at 0
            }

            // build per note timing
            targetTimes = new float[songBeats.Length]; // song-relative seconds, which is used by RKNote/Conductor
            spawnTimes = new double[songBeats.Length]; // absolute dsp timestamps, which is used by RKSpawner

            float offsetSeconds = offsetMs / 1000f; // converts ms to secs

            for (int i = 0; i < songBeats.Length; i++)
            {
                // ok, this is when the note SHOULD hit the line, in song seconds since song start
                // beat_time + calibration offset
                float hitTimeSongSeconds = (songBeats[i] * secondsPerBeat) + offsetSeconds;
                targetTimes[i] = hitTimeSongSeconds;

                // this is when it should spawn the note, in absolute dsp time
                // start_time + (hit_time - travel_time)
                double spawnDspTime = songStartDspTime + (hitTimeSongSeconds - travelTime);
                spawnTimes[i] = spawnDspTime;
            }
            Debug.Log($"[SongData] bpm={bpm} spb{secondsPerBeat:F3} beats={songBeats.Length}");


            // at the end of your Awake(), after you built songBeats/targetTimes/spawnTimes
            if (!string.IsNullOrWhiteSpace(chartString))
            {
                parts = chartString.Replace(" ", "").Split(',');
                chartByBeat = new RKNote.Type[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                    chartByBeat[i] = ParseType(parts[i]);
            }
            else
            {
                chartByBeat = null; // fall back to random or single-lane if you want
            }
        }

            // helper inside RKSongData
            private RKNote.Type ParseType(string s)
            {
                s = s.ToLowerInvariant();
                // map names or keys to lanes/types
                // adjust if you’re using food names instead of Lane1..4
                switch (s)
                {
                    case "lane1": case "a": case "cucumber": return RKNote.Type.Lane1;
                    case "lane2": case "w": case "tomato": return RKNote.Type.Lane2;
                    case "lane3": case "s": case "lettuce": return RKNote.Type.Lane3;
                    case "lane4": case "d": case "crouton": return RKNote.Type.Lane4;
                    default:
                        Debug.LogWarning($"[SongData] Unknown chart token '{s}', defaulting Lane1");
                        return RKNote.Type.Lane1;
                }
            }
    } 
}