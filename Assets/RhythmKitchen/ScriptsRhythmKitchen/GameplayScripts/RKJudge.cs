using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Worked on by: Jovanna Molina
// Commented by: Jovanna Molina

namespace RhythmKitchen
{public class RKJudge : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private RKConductor conductor;
        [SerializeField] private Transform notesRuntime; // parent object for spawned notes

        [Header("Keys (A/W/S/D by lane)")]
        public KeyCode keyLane1 = KeyCode.A;
        public KeyCode keyLane2 = KeyCode.W;
        public KeyCode keyLane3 = KeyCode.S;
        public KeyCode keyLane4 = KeyCode.D;

        [Header("Windows (seconds)")]
        public float missWindow = 0.250f;
        public float goodWindow = 0.220f;
        public float perfectWindow = 0.120f;

        void Update()
        {
            if (conductor == null || notesRuntime == null)
            {
                return;
            }
            if (Input.GetKeyDown(keyLane1))
            {
                TryHit(RKNote.Type.Lane1);
            }
            if (Input.GetKeyDown(keyLane2))
            {
                TryHit(RKNote.Type.Lane2);
            }
            if (Input.GetKeyDown(keyLane3))
            {
                TryHit(RKNote.Type.Lane3);
            }
            if (Input.GetKeyDown(keyLane4))
            {
                TryHit(RKNote.Type.Lane4);
            }
        }

        private void TryHit(RKNote.Type type)
        {
            RKNote target = FindClosestNoteInLane(type);
            if (target == null)
            {
                return;
            }
            float now = conductor.songTime;
            float delta = Mathf.Abs(now - target.targetTime);

            if (delta <= perfectWindow)
            {
                OnHit(target, "PERFECT");
            }
            else if (delta <= goodWindow)
            {
                OnHit(target, "GOOD");
            }
            else if (delta <= missWindow)
            {
                OnHit(target, "MISS");
            }
            else
            {
                // too far away
            }
        }

        private RKNote FindClosestNoteInLane(RKNote.Type type)
        {
            RKNote closest = null;
            float closestDelta = float.MaxValue;

            foreach (Transform child in notesRuntime)
            {
                RKNote note = child.GetComponent<RKNote>();
                if (note != null && note.noteType == type)
                {
                    float now = conductor.songTime;
                    float delta = Mathf.Abs(now - note.targetTime);
                    if (delta < closestDelta)
                    {
                        closestDelta = delta;
                        closest = note;
                    }
                }
            }
            return closest;
        }
        private void OnHit(RKNote note, string rating)
        {
            // NOTE: expand this to add score, UI, SFX
            Debug.Log($"[Judge] {rating} {note.noteType}");
            Destroy(note.gameObject);
        }
    }
}

