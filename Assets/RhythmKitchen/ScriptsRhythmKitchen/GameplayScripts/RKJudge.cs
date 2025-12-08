using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

// Worked on by: Jovanna Molina and Leia Phillips
// Commented by: Jovanna Molina

namespace RhythmKitchen
{ public class RKJudge : MonoBehaviour
    {
        [SerializeField] private bool debugOn;

        [Header("Refs")]
        [SerializeField] private RKConductor conductor;
        [SerializeField] private Transform notesRuntime; // parent object for spawned notes

        [Header("Keys (A/W/S/D by lane)")]
        public KeyCode keyLane1 = KeyCode.A;
        public KeyCode keyLane2 = KeyCode.W;
        public KeyCode keyLane3 = KeyCode.S;
        public KeyCode keyLane4 = KeyCode.D;

        [Header("Captury Inputs")]
        [SerializeField] private InputActionAsset inputActions;
        
        private InputAction leftHipAction;
        private InputAction leftFootRaised;
        private InputAction rightFootRaised;
        private InputAction rightHipAction;

        private bool isLeftHipAbduct = false;
        private bool isLeftLegLift = false;
        private bool isRightLegLift = false;
        private bool isRightHipAbduct = false;

        [Header("Windows (seconds)")]
        public float missWindow;
        public float goodWindow;
        public float perfectWindow;

        void Awake()
        {
            var actionMap = inputActions.FindActionMap("Foot");
            leftHipAction = actionMap.FindAction("LeftHipAbducted");
            leftFootRaised = actionMap.FindAction("LeftStep");
            rightFootRaised = actionMap.FindAction("RightStep");
            rightHipAction = actionMap.FindAction("RightHipAbducted");
        }

        private void OnEnable()
        {
            
            leftHipAction.Enable();
            leftFootRaised.Enable();
            rightFootRaised.Enable();
            rightHipAction.Enable();

            leftHipAction.performed += OnLeftHipAbduction;
            leftFootRaised.performed += OnLeftFootRaised;
            rightFootRaised.performed += OnRightFootRaised;
            rightHipAction.performed += OnRightHipAbduction;
        }

        private void OnDisable()
        {
            leftHipAction.Disable();
            leftFootRaised.Disable();
            rightFootRaised.Disable();
            rightHipAction.Disable();

            leftHipAction.performed -= OnLeftHipAbduction;
            leftFootRaised.performed -= OnLeftFootRaised;
            rightFootRaised.performed -= OnRightFootRaised;
            rightHipAction.performed -= OnRightHipAbduction;
        }

        private void OnLeftHipAbduction(InputAction.CallbackContext contex)
        {
            isLeftHipAbduct = true;
            Debug.Log("[Captury] OnLeftHipAbduction Called");
        }

        private void OnLeftFootRaised(InputAction.CallbackContext contex)
        {
            isLeftLegLift = true;
            Debug.Log("[Captury] OnLeftFootRaised Called");
        }
        
        private void OnRightFootRaised(InputAction.CallbackContext contex)
        {
            isRightLegLift = true;
            Debug.Log("[Captury] OnRightFootRaised Called");
        }

        private void OnRightHipAbduction(InputAction.CallbackContext contex)
        {
            isRightHipAbduct = true;
            Debug.Log("[Captury] OnRightHipAbduction Called");
        }

        void Update()
        {
            if (debugOn)
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
            else
            {
                if (conductor == null || notesRuntime == null)
                {
                    return;
                }
                if (isLeftHipAbduct)
                {
                    TryHit(RKNote.Type.Lane1);
                    isLeftHipAbduct = false;
                    Debug.Log("[Captury] Left hip abduction attempt");
                }
                if (isLeftLegLift)
                {
                    TryHit(RKNote.Type.Lane2);
                    isLeftLegLift = false;
                    Debug.Log("[Captury] Left leg abduct attempt");
                }
                if (isRightLegLift)
                {
                    TryHit(RKNote.Type.Lane3);
                    isRightLegLift = false;
                    Debug.Log("[Captury] Right leg abduct attempt");
                }
                if (isRightHipAbduct)
                {
                    TryHit(RKNote.Type.Lane4);
                    isRightHipAbduct = false;
                    Debug.Log("[Captury] Right hip abduction attempt");
                }
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
                OnHit(target, "ALMOST");
            }
            else
            {
                Debug.Log($"[Judge] Too Far Away");
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

