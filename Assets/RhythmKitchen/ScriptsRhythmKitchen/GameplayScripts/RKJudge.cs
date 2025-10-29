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
        [SerializeField] private RKSpawner spawner;

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
            if (conductor == null)
            {
                return;
            }

        }

        private void TryHit(RKNote.Type type)
        {
            
        }
    }
}

