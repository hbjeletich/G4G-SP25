using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Worked on by: Jovanna Molina
// Commented by: Jovanna Molina
namespace RhythmKitchen
{ public class RKNote : MonoBehaviour
    {
        public enum Type { Lane1, Lane2, Lane3, Lane4 } // this defines the note type
        public Type noteType; // this is set in prefab

        [HideInInspector] public float targetTime; // when it should HIT the line
        [HideInInspector] public float targetY; // the Y position of HitLine
        [HideInInspector] public float travelTime; // how long the note should take to fall

        private Vector2 spawnPos; // stores the initial spawn position
        private Vector2 targetPos; // stores where are falling TO
        private RKConductor conductor; // this gives us songTime to track progress

        public void Init(RKConductor c, Vector2 spawn, float targetY, float targetTime, float travelTime)
        {
            conductor = c; // saves the Conductor ref
            spawnPos = spawn; // stores where food items are spawned from
            this.targetY = targetY; // stores the Y target of the HitLine
            this.targetTime = targetTime; // stores when food item should arrive at the HitLine
            this.travelTime = travelTime; // stores how long the fall should last
            targetPos = new Vector2(spawn.x, targetY); // defines where the note should land and keep straight-down fall
            transform.position = spawnPos; // places the note where it starts
            Debug.Log($"[Note] Spawned {noteType} -> land @ {targetTime:0.000}s"); // helpful debug
        }
        public void Update()
        {
            if (conductor == null) // if this Note doesn't have a ref to a Conductor
            {
                return; // returns nothing
            }
            float now = conductor.songTime; // current time of the song
            float t = Mathf.Clamp01(1f - ((targetTime - now) / travelTime)); // normalizes progress from 0 to 1 based on time to target
            // (targetTime - now) == remaining time
            transform.position = Vector2.Lerp(spawnPos, targetPos, t); // this moves the note smoothly toward the target position

            if (now > targetTime + 0.25f) // if current time of song is > 250ms after targetTime
            {
                Debug.Log($"[Note] MISS {noteType}");
                Destroy(gameObject);
            }
        }
    }
}

