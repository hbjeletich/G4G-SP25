using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Worked on by: Jovanna Molina
// Commented by: Jovanna Molina
namespace RhythmKitchen
// this is the runtime spawner that instantiates notes and tells them when/where to land
{public class RKSpawner : MonoBehaviour
    {
        [SerializeField] private RKSongData songData;
        [Header("Song Chart")]
        [SerializeField] private RKNote.Type[] laneByBeat; // per-beat type (same length as songBeats)
        [SerializeField] private bool useRandom = false;
        private int nextIndex = 0;

        [Header("Refs")]
        [SerializeField] private RKConductor conductor; // this is beat/time source 
        [SerializeField] private Transform hitLine; // this is target Y position

        [Header("Travel")]
        [SerializeField] private float travelTime = 1.00f; // this is seconds it should take from spawn to hit

        [Header("Spawn Points (by lane)")]
        public Transform spawnCucumber;
        public Transform spawnTomato;
        public Transform spawnLettuce;
        public Transform spawnCrouton;

        [Header("Prefabs (by type)")]
        public RKNote prefabCucumber;
        public RKNote prefabTomato;
        public RKNote prefabLettuce;
        public RKNote prefabCrouton;

        void Start()
        {
            if (songData == null || songData.spawnTimes == null || songData.spawnTimes.Length == 0)
            {
                Debug.LogError("[Spawner] Missing SongData or SpawnTimes");
                enabled = false;
                return;
            }

            travelTime = songData.travelTime;

            if (!useRandom && (laneByBeat == null || laneByBeat.Length != songData.spawnTimes.Length))
            {
                Debug.LogWarning($"[Spawner] laneByBeat length {laneByBeat?.Length ?? 0} != beats {songData.spawnTimes.Length}");
            }
            nextIndex = 0;
        }
        private Transform GetSpawnPoint(RKNote.Type noteType)
        {
            switch (noteType)
            {
                case RKNote.Type.Cucumber:
                    return spawnCucumber;
                case RKNote.Type.Tomato:
                    return spawnTomato;
                case RKNote.Type.Lettuce:
                    return spawnLettuce;
                case RKNote.Type.Crouton:
                    return spawnCrouton;
                default:
                    Debug.LogWarning("[Spawner] NO spawn point found for " + noteType);
                    return null;
            }
        }
        private RKNote GetPrefab(RKNote.Type noteType)
        {
            switch (noteType)
            {
                case RKNote.Type.Cucumber:
                    return prefabCucumber;
                case RKNote.Type.Tomato:
                    return prefabTomato;
                case RKNote.Type.Lettuce:
                    return prefabLettuce;
                case RKNote.Type.Crouton:
                    return prefabCrouton;
                default:
                    Debug.LogWarning("[Spawner] NO prefab found for " + noteType);
                    return null;
            }
        }
        private RKNote.Type ChooseType(int i)
        {
            if (useRandom)
            {
                return (RKNote.Type)Random.Range(0, 4);
            }
            if (laneByBeat != null && i < laneByBeat.Length)
            {
                return laneByBeat[i];
            }
            return RKNote.Type.Cucumber; // just a fallback here
        }
        public void SpawnAt(RKNote.Type noteType, float targetTime)
        {
            Transform spawn = GetSpawnPoint(noteType); // gets which spawn point to use
            if (spawn == null)
            {
                return;
            }
            RKNote prefab = GetPrefab(noteType); // gets which prefab to use
            if (prefab == null)
            {
                return;
            }
            // instantiate note and parent under NotesRuntime 
            RKNote note = Instantiate(prefab, spawn.position, Quaternion.identity);
            note.transform.SetParent(GameObject.Find("NotesRuntime").transform);

            note.Init(conductor, spawn.position, hitLine.position.y, targetTime, travelTime); // initialize with target info
        }
        void Update()
        {
            if (conductor == null || songData == null)
            {
                return;
            }
            double dspNow = AudioSettings.dspTime;

            while (nextIndex < songData.spawnTimes.Length && dspNow >= songData.spawnTimes[nextIndex])
            {
                var type = ChooseType(nextIndex);

                float targetTime = (float)(songData.spawnTimes[nextIndex] + songData.travelTime);
                SpawnAt(type, targetTime);
                nextIndex++;
            }
        }
    }
}


