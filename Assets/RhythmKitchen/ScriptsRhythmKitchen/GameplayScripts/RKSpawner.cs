using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Worked on by: Jovanna Molina
// Commented by: Jovanna Molina
namespace RhythmKitchen
// this is the runtime spawner that instantiates notes and tells them when/where to land
{public class RKSpawner : MonoBehaviour
    {
        [Header("Song Chart")]
        [SerializeField] private RKNote.Type[] laneByBeat; // per-beat type (same length as songBeats)
        [SerializeField] private bool useRandom = false;

        [Header("Refs")]
        [SerializeField] private RKConductor conductor; // this is beat/time source 
        [SerializeField] private Transform hitLine; // this is target Y position
        [SerializeField] private RKSongData songData;
        [SerializeField] private RKJudge judge;

        [Header("Travel")] // comes from SongData... usually
        [SerializeField] private float travelTime = 1.00f; // this is seconds it should take from spawn to hit

        [Header("Spawn Points (by lane)")]
        public Transform spawnLane1;
        public Transform spawnLane2;
        public Transform spawnLane3;
        public Transform spawnLane4;

        [Header("Prefabs (by type)")] // comes from SongData
        private RKNote prefabLane1;
        private RKNote prefabLane2;
        private RKNote prefabLane3;
        private RKNote prefabLane4;


        [SerializeField] private Transform notesParent; // parent object for spawned notes
        int nextIndex;

        void Awake()
        {
            prefabLane1 = songData.prefabLane1;
            prefabLane2 = songData.prefabLane2;
            prefabLane3 = songData.prefabLane3;
            prefabLane4 = songData.prefabLane4;
        }

        void Start()
        {
            if (songData == null || songData.spawnTimes == null || songData.spawnTimes.Length == 0)
            {
                Debug.LogError("[Spawner] Missing SongData or SpawnTimes");
                enabled = false;
                return;
            }

            travelTime = songData.travelTime;
            nextIndex = 0;

            /* if (!useRandom && (laneByBeat == null || laneByBeat.Length != songData.spawnTimes.Length))
            {
                Debug.LogWarning($"[Spawner] laneByBeat length {laneByBeat?.Length ?? 0} != beats {songData.spawnTimes.Length}");
            }
            nextIndex = 0; */
        }
        void Update()
        {
            if (conductor == null || songData == null)
            {
                return;
            }
            double dspNow = AudioSettings.dspTime;

            // spawn any notes whose spawn DSP time has arrived
            while (nextIndex < songData.spawnTimes.Length && dspNow >= songData.spawnTimes[nextIndex])
            {
                var type = ChooseType(nextIndex);
                float targetTimeSongSec = songData.targetTimes[nextIndex];
                SpawnAt(type, targetTimeSongSec);
                nextIndex++;
            }

            /*// TEMPORARY HERE
            if (Input.GetKeyDown(KeyCode.T))
            {
                float now = conductor.songTime;
                // spawn a note that should hit exactly travelTime seconds from now
                SpawnAt(RKNote.Type.Lane1, now + songData.travelTime);
            } */
        }
        private RKNote.Type ChooseType(int i)
        {
            /*if (useRandom)
            {
                return (RKNote.Type)Random.Range(0, 4);
            }
            if (laneByBeat != null && i < laneByBeat.Length)
            {
                return laneByBeat[i];
            }
            return RKNote.Type.Lane1; // just a fallback here */

            var chart = songData.chartByBeat;
            if (chart != null && i < chart.Length)
            {
                return chart[i];
            }
            return (RKNote.Type)Random.Range(0, 4); // random fallback
        }
        private Transform GetSpawnPoint(RKNote.Type noteType)
        {
            switch (noteType)
            {
                case RKNote.Type.Lane1:
                    return spawnLane1;
                case RKNote.Type.Lane2:
                    return spawnLane2;
                case RKNote.Type.Lane3:
                    return spawnLane3;
                case RKNote.Type.Lane4:
                    return spawnLane4;
                default:
                    Debug.LogWarning("[Spawner] NO spawn point found for " + noteType);
                    return null;
            }
        }
        private RKNote GetPrefab(RKNote.Type noteType)
        {
            switch (noteType)
            {
                case RKNote.Type.Lane1:
                    return prefabLane1;
                case RKNote.Type.Lane2:
                    return prefabLane2;
                case RKNote.Type.Lane3:
                    return prefabLane3;
                case RKNote.Type.Lane4:
                    return prefabLane4;
                default:
                    Debug.LogWarning("[Spawner] NO prefab found for " + noteType);
                    return null;
            }
        }
        public void SpawnAt(RKNote.Type noteType, float targetTimeSongSec)
        {
            var spawn = GetSpawnPoint(noteType);
            var prefab = GetPrefab(noteType);
            if (!spawn || !prefab)
            {
                Debug.LogWarning("[Spawner] Missing spawn or prefab");
                return;
            }
            var note = Instantiate(prefab, spawn.position, Quaternion.identity);
            note.transform.SetParent(notesParent, true); // grouping

            note.Init(conductor, judge, spawn.position, hitLine.position.y, targetTimeSongSec, travelTime);

            if (judge != null)
            {
                judge.ShowInstruction(noteType);
            }
        }
    }
}


