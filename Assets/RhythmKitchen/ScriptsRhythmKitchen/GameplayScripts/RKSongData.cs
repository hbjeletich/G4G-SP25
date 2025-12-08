using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

// Worked on by: Jovanna Molina and Leia Phillips 
// Commented by: Jovanna Molina and Leia Phillips

namespace RhythmKitchen
{
    public class RKSongData : MonoBehaviour
    {
        [Header("Authoring")]
        public string dishName; // Name of the dish, set in Unity
        public AudioClip audioClip; // the AudioClip for the dish, set in Unity
        public float songLength; // the length on seconds of the song, set in Unity
        public float bpm; // BPM of song, set in Unity
        public float travelTime = 1f; // how long it takes the beat to travel from spawn point to hit lane, set in Unity
        public float leadInSeconds = 1f; // this is a small delay so we can schedule precisely, set in Unity
        public float offsetMs = 0f; // this is calibration to nudge timing if it feels early/late (positive = judge later, negative = judge earlier), set in Unity
        [TextArea] public string songBeatString; // comma-separated list of the songBeats for with the ingredients should hit the judgement line, set in Unity
        [TextArea] public string chartString; // comma-separated list of note types (1,2,3,4)

        [Header("Prefabs (by type)")] // Prefabs for the Lanes, set in Unity
        public RKNote prefabLane1;
        public RKNote prefabLane2;
        public RKNote prefabLane3;
        public RKNote prefabLane4;

        [Header("Outlines")] // Sprites of the outlines for the ingredients, set in Unity
        public Sprite outlineSprite1;
        public Sprite outlineSprite2;
        public Sprite outlineSprite3;
        public Sprite outlineSprite4;

        [Header("Don't Touch!")] // Note for those changing values in Unity
        public AudioSource audioSource; // The AudioSource for the scene, set in Unity
        public SpriteRenderer outlineHolder1; // Where the outline for lane1 should be, set in Unity
        public SpriteRenderer outlineHolder2; // Where the outline for lane2 should be, set in Unity
        public SpriteRenderer outlineHolder3; // Where the outline for lane3 should be, set in Unity
        public SpriteRenderer outlineHolder4; // Where the outline for lane4 should be, set in Unity
        

        // computed and consumed by other scripts
        public float[] songBeats { get; private set; } // the beats of the input
        public float[] targetTimes { get; private set; } // SONG-RELATIVE seconds when notes hit the line
        public double[] spawnTimes { get; private set; } // dsp times when to instantiate each note
        public float secondsPerBeat { get; private set; } // this is 60/BPM
        public double songStartDspTime { get; private set; } // this is the DSP timestamp when the song will start

        public RKNote.Type[] chartByBeat { get; private set; } // the note type per beat parsed from chartString

        void Awake()
        {
            audioSource.clip = audioClip;

            outlineHolder1.sprite = outlineSprite1;
            outlineHolder2.sprite = outlineSprite2;
            outlineHolder3.sprite = outlineSprite3;
            outlineHolder4.sprite = outlineSprite4;

            // this is the base timing
            secondsPerBeat = 60f / Mathf.Max(1f, bpm); // this just avoids divide by zero
            songStartDspTime = AudioSettings.dspTime + leadInSeconds; // when the AudioSource will start

            // this parses the beat list 
            var parts = songBeatString.Replace(" ", "").Split(',');
            songBeats = new float[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                songBeats[i] = float.Parse(parts[i]) - 1; //subtracting one so the first beat would be at 0
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