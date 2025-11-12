using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Worked on by: Jovanna Molina and Leia Phillips 
// Commented by: Jovanna Molina

namespace RhythmKitchen
{ public class RKConductor : MonoBehaviour
    {
        public RKSongData songData;
        public AudioSource musicSource;

        public float songTime { get; private set; } // seconds since scheduled song start w/ no offset
        public float curSongBeat => (songData != null && songData.secondsPerBeat > 0f) ? songTime / songData.secondsPerBeat : 0f;

        public double songStartDspTime;
        float nextLog;

        void Start()
        {
            if (!musicSource || !musicSource.clip)
            {
                Debug.LogError("[Conductor] Missing AudioSource or clip");
                enabled = false;
                return;
            }

            musicSource.playOnAwake = false;
            musicSource.loop = false;

            songStartDspTime = songData.songStartDspTime;
            musicSource.PlayScheduled(songStartDspTime);
            Debug.Log($"[Conductor] Scheduled '{musicSource.clip.name}' @ dsp={songStartDspTime:F3}");
        }

        void Update()
        {
            // songTime is jus DSP-now minus scheduled start (offset already baked into targetTimes)
            songTime = (float)(AudioSettings.dspTime - songStartDspTime);

            if (songTime >= nextLog)
            {
                Debug.Log($"[Conductor] t={songTime:0.000}s beat{curSongBeat:0.00}");
                nextLog += 1f;
            }
        }

    }

}

