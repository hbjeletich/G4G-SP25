using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

// Worked on by: Jovanna Molina and Leia Phillips
// Commented by: Jovanna Molina and Leia Phillips

// Script is ONLY attached to Input object under CAPTURY

namespace RhythmKitchen
{
    public class RKJudge : MonoBehaviour
    {
        [SerializeField] private bool debugOn;

        [Header("Refs")]
        [SerializeField] private RKConductor conductor;
        [SerializeField] private RKCompletedDishScript completedDish;
        [SerializeField] private Transform notesRuntime; // parent object for spawned notes


        [Header("Keys (A/W/S/D by lane)")]
        public KeyCode keyLane1 = KeyCode.A;
        public KeyCode keyLane2 = KeyCode.W;
        public KeyCode keyLane3 = KeyCode.S;
        public KeyCode keyLane4 = KeyCode.D;

        [Header("Captury Inputs")]
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private float footThreshold;

        private InputAction leftHipAction;
        private InputAction leftFootPositionAction;
        private InputAction rightFootPositionAction;
        private InputAction rightHipAction;
        // private InputAction footLoweredAction;

        private bool isLeftHipAbduct = false;
        private bool isRightHipAbduct = false;
        private bool isLeftFootRaised = false;
        private bool isRightFootRaised = false;
        // private bool isFootLowered = false;

        private float initialLeftFootZPos = 0f;
        private float initialRightFootZPos = 0f;

        [Header("Windows (seconds)")]
        public float missWindow;
        public float goodWindow;
        public float perfectWindow;

        [Header("Score UI (assign TMP objects)")]
        [SerializeField] private TMP_Text judgmentText;
        [SerializeField] private TMP_Text perfectNum;
        [SerializeField] private TMP_Text goodNum;
        [SerializeField] private TMP_Text almostNum;
        [SerializeField] private TMP_Text comboNum;
        [SerializeField] private TMP_Text maxComboNum;

        int perfectCount, goodCount, almostCount;
        int comboCount, maxComboCount;

        void UpdateUI()
    {
        if (perfectNum)
        {
            perfectNum.text = perfectCount.ToString();
        }
        if (goodNum)
        {
            goodNum.text = goodCount.ToString();
        }
        if (almostNum)
        {
            almostNum.text = almostCount.ToString();
        }
        if (comboNum)
        {
            comboNum.text = comboCount.ToString();
        }
        if (maxComboNum)
        {
            maxComboNum.text = maxComboCount.ToString();
        }
    }

        void Awake()
        {
            var actionMap = inputActions.FindActionMap("Foot");
            leftHipAction = actionMap.FindAction("LeftHipAbducted");
            leftFootPositionAction = actionMap.FindAction("LeftFootPosition");
            rightFootPositionAction = actionMap.FindAction("RightFootPosition");
            rightHipAction = actionMap.FindAction("RightHipAbducted");
            // footLoweredAction = actionMap.FindAction("FootLowered");
        }

        private void OnEnable()
        {

            leftHipAction.Enable();
            leftFootPositionAction.Enable();
            rightFootPositionAction.Enable();
            rightHipAction.Enable();
            // footLoweredAction.Enable();

            leftHipAction.performed += OnLeftHipAbduction;
            rightHipAction.performed += OnRightHipAbduction;
            // footLoweredAction.performed += OnFootLowered;
        }

        private void OnDisable()
        {
            leftHipAction.Disable();
            leftFootPositionAction.Disable();
            rightFootPositionAction.Disable();
            rightHipAction.Disable();
            // footLoweredAction.Disable();

            leftHipAction.performed -= OnLeftHipAbduction;
            rightHipAction.performed -= OnRightHipAbduction;
            // footLoweredAction.performed -= OnFootLowered;
        }

        private void OnLeftHipAbduction(InputAction.CallbackContext contex)
        {
            isLeftHipAbduct = true;
            Debug.Log("[Captury] OnLeftHipAbduction Called");
        }

        private void OnLeftFootRaised()
        {
            isLeftFootRaised = true;
            Debug.Log("[Captury] OnLeftFootRaised Called");
        }

        private void OnRightFootRaised()
        {
            isRightFootRaised = true;
            Debug.Log("[Captury] OnRightFootRaised Called");
        }

        private void OnRightHipAbduction(InputAction.CallbackContext contex)
        {
            isRightHipAbduct = true;
            Debug.Log("[Captury] OnRightHipAbduction Called");
        }

        // private void OnFootLowered(InputAction.CallbackContext contex)
        // {
        //     isFootLowered = true;
        //     Debug.Log("[Captury] OnFootLowered Called");
        // }

        public void initialFootPositionCallibration()
        {
            // Sets the initial foot positions to the current value
            // The player should be prompted to stand still!
            initialLeftFootZPos = leftFootPositionAction.ReadValue<Vector3>().z;
            initialRightFootZPos = rightFootPositionAction.ReadValue<Vector3>().z;
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
                // check for position input
                float rightFootYPos = rightFootPositionAction.ReadValue<Vector3>().y;
                float leftFootYPos = leftFootPositionAction.ReadValue<Vector3>().y;
                float rightFootZPos = rightFootPositionAction.ReadValue<Vector3>().z;
                float leftFootZPos = leftFootPositionAction.ReadValue<Vector3>().z;
                // Debug.Log($"[Captury Foot] Left Foot Y: {leftFootYPos}, Right Foot Y: {rightFootYPos}");
                Debug.Log($"[Captury Foot] Left Foot Z: {leftFootZPos}, Right Foot Z: {rightFootZPos}/ Initial Left : {initialLeftFootZPos}, Initial Right: {initialRightFootZPos} ");

                // if (isFootLowered)
                // {
                //     isFootLowered = false;

                //     initialLeftFootZPos = leftFootPositionAction.ReadValue<Vector3>().z;
                //     initialRightFootZPos = rightFootPositionAction.ReadValue<Vector3>().z;
                // }

                if(rightFootZPos >= footThreshold + initialRightFootZPos)
                {
                    OnRightFootRaised();
                }
                else if(leftFootZPos >= footThreshold + initialLeftFootZPos)
                {
                    OnLeftFootRaised();
                }

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
                if (isLeftFootRaised)
                {
                    TryHit(RKNote.Type.Lane2);
                    isLeftFootRaised = false;
                    Debug.Log("[Captury] Left foot lift attempt");
                }
                if (isRightFootRaised)
                {
                    TryHit(RKNote.Type.Lane3);
                    isRightFootRaised = false;
                    Debug.Log("[Captury] Right foot lift attempt");
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
            else
            {
                OnHit(target, "ALMOST");
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

            UpdateUI();

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

        void RegisterMiss()
        {
            comboCount = 0;
            UpdateUI();
            if (debugOn)
            {
                Debug.Log("[Judge] MISS (pressed too far from target)");
            }
            //play SFX here:
            // RKAudioManager.Instance.PlaySFX("miss")...;
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
    }
}  

