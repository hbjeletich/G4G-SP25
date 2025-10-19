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
        public KeyCode keyCucumber = KeyCode.A;
        public KeyCode keyTomato = KeyCode.W;
        public KeyCode keyLettuce = KeyCode.S;
        public KeyCode keyCrouton = KeyCode.D;

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

