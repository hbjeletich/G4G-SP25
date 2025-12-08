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
<<<<<<< HEAD
            switch (rating)
            {
                case "PERFECT":
                    perfectCount++;
                    comboCount++;
                    if (comboCount > maxComboCount)
                    {
                        maxComboCount = comboCount;
                    }
                    StartCoroutine(judgementTextDisplay("Perfect!"));
                    break;
                case "GOOD":
                    goodCount++;
                    comboCount++;
                    if (comboCount > maxComboCount)
                    {
                        maxComboCount = comboCount;
                    }
                    StartCoroutine(judgementTextDisplay("Good"));
                    break;
                case "ALMOST":
                    almostCount++;
                    comboCount = 0; // this resets combo on almost. correct? ask Leia and Katie "want we ALMOST to NOT keep combo?"
                    StartCoroutine(judgementTextDisplay("Almost"));
                    break;
            }

            var am = RKAudioManager.Instance; // Current instance of the AudioManager

            if (am != null) // Checks if an AudioManager AudioManager instance exists
            {
                am.PlaySFX("VO" + rating); // Plays the voice over for rating sound
                am.PlaySFX("NoteDestroy"); // Plays the note destroy sound
            }
            else
                Debug.LogWarning("[RKJudge] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning

            UpdateUI();

            HideInstruction();

            // play SFX here:
            // RKAudioManager.Instance.PlaySFX(rating)...;

            Debug.Log($"[Judge] {rating} {note.noteType}");
            //Destroy(note.gameObject);
            note.ExplodeAndDestroy(); // particle effect + destroy
            if (debugOn)
            {
                Debug.Log($"[Judge] {rating} {note.noteType} (combo {comboCount})");
            }
        }

        public void RegisterMiss()
        {
            almostCount++;
            comboCount = 0;
            StartCoroutine(judgementTextDisplay("Almost"));

            HideInstruction();

            UpdateUI();
            if (debugOn)
            {
                Debug.Log("[Judge] MISS (pressed too far from target)");
            }

            var am = RKAudioManager.Instance; // Current instance of the AudioManager

            if (am != null) // Checks if an AudioManager AudioManager instance exists
            {
                am.PlaySFX("VOALMOST"); // Plays the voice over for rating sound
                am.PlaySFX("NoteDestroy"); // Plays the note destroy sound
            }
            else
                Debug.LogWarning("[RKJudge] AudioManager missing: loading scene anyway."); // If the AudioManager instance does not exist it logs a warning

        }

        public void starScore()
        {
            int totalBeats = perfectCount + goodCount + almostCount;

            int score = 1;

            if (almostCount < 1 && totalBeats/4 <= goodCount)
                score = 3;
            else if (totalBeats/2 > almostCount)
                score = 2;

            completedDish.setStars(score);
        }

        public IEnumerator judgementTextDisplay(string score)
        {
            float duration = 2f; // Length in seconds the text will stay on screen
            
            judgmentText.text = score; // Change the text to the string score

            yield return new WaitForSeconds(duration); // wait (duration) seconds

            judgmentText.text = " "; // Empty the text field
        }
    

        string GetInstructionForLane(RKNote.Type type)
    {
            switch (type) // assigned instructions based on lane type
        {
            case RKNote.Type.Lane1:
                return "Left Hip Abduct";
            case RKNote.Type.Lane2:
                return "Left Foot Raise";
            case RKNote.Type.Lane3:
                return "Right Foot Raise";
            case RKNote.Type.Lane4:
                return "Right Hip Abduct";
            default:
                return "";
        }
    }

        public void ShowInstruction(RKNote.Type type)
        {
            // string msg = GetInstructionForLane(type);
            // if (string.IsNullOrEmpty(msg))
            // {
            //     return;
            // }

            // if (instructionRoutine != null) // if previous is still running, stop it
            // {
            //     StopCoroutine(instructionRoutine);
            // }
            // instructionRoutine = StartCoroutine(InstructionRoutine(msg));
            instructionText.text = GetInstructionForLane(type);

        }

        public void HideInstruction()
        {
            instructionText.text = "";
        }

        // IEnumerator InstructionRoutine(string msg)
        // {
        //     float duration = 10.5f; 
        //     instructionText.text = msg;
        //     yield return new WaitForSeconds(duration);
        //     instructionText.text = "";
        // }
    }
}

 
=======
            Debug.Log($"[Judge] {rating} {note.noteType}");
            Destroy(note.gameObject);
        }
    }
}  
>>>>>>> 3430e29c05f0efb5e9595b287b669369c65a461a

