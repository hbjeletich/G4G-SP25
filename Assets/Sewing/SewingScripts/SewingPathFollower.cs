using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sewing;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace Sewing{
    

    public class SewingPathFollower : MonoBehaviour
    {
        [Header("Movement & Animation Settings")]
        [SerializeField] private InputActionAsset inputActions;
        public List<Transform> waypoints = new List<Transform>();          // Waypoints set in the Inspector
        public float moveSpeed = 2f;               // Speed of movement
        private int currentIndex = 0;
        private bool isMoving = false;
        private InputAction footRaiseAction, footLowerAction, leftFootHeightAction, rightFootHeightAction;
        private bool isFootRaised = false;
        public Animator machineAnimator;
        public bool keyboardControl = false;
        [Header("Path Completion Settings")]
            public float waitTime = 3f;

            public UnityEvent OnPathComplete;
            private bool pathComplete = false;

            private float totalFootHeight = 0f;
            private int footHeightSamples = 0;
            private float footTime = 0f;

        void Awake()
        {   
            var actionMap = inputActions.FindActionMap("Foot");
            footRaiseAction = actionMap.FindAction("FootRaised");
            footLowerAction = actionMap.FindAction("FootLowered");
            leftFootHeightAction = actionMap.FindAction("LeftFootPosition");
            rightFootHeightAction = actionMap.FindAction("RightFootPosition");
            footRaiseAction.performed += OnFootRaise;
            footLowerAction.performed += OnFootLower;
        }

        private void OnEnable()
        {
            footRaiseAction.Enable();
            footLowerAction.Enable();
            leftFootHeightAction.Enable();
            rightFootHeightAction.Enable();
        }

        private void OnDisable()
        {
            footRaiseAction.Disable();
            footLowerAction.Disable();
            leftFootHeightAction.Disable();
            rightFootHeightAction.Disable();
        }
        private void OnFootRaise(InputAction.CallbackContext ctx)
        {
            totalFootHeight = 0f;
            footHeightSamples = 0;
            footTime = 0f;

            float leftFootY = leftFootHeightAction.ReadValue<Vector3>().y;
            float rightFootY = rightFootHeightAction.ReadValue<Vector3>().y;
            string dataStr = $"Foot: {(leftFootY > rightFootY ? "Left" : "Right")}";
            DataLogger.Instance.LogData("FootRaise", dataStr);

            isFootRaised = true;
            machineAnimator.SetTrigger("Start");
            SoundManager.PlayLoopingSound(SoundType.SEWING);
        }

        private void OnFootLower(InputAction.CallbackContext ctx)
        {
            float averageFootHeight = footHeightSamples > 0 ? totalFootHeight / footHeightSamples : 0f;
            DataLogger.Instance.LogData("FootLower", $"AverageFootHeight: {averageFootHeight.ToString("F2")}; TimeRaised: {footTime.ToString("F2")}s");
            
            isFootRaised = false;
            machineAnimator.SetTrigger("Stop");
            SoundManager.StopLoopingSound();
        }

        private void NeedleMoving()
        {
                // added while loop -- only add a waypoint if not already moving
                while (!isMoving)
                {
                    if (waypoints.Count == 0) return;

                    int nextIndex = currentIndex + 1;
                    if (nextIndex >= waypoints.Count)
                    {
                        return; // Reached end, don't continue
                    }

                    UnityEngine.Debug.Log("Moving to waypoint " + nextIndex);
                    StartCoroutine(MoveToWaypoint(waypoints[nextIndex].position));
                    currentIndex = nextIndex;

                    // add foot height to average
                    float leftFootY = leftFootHeightAction.ReadValue<Vector3>().y;
                    float rightFootY = rightFootHeightAction.ReadValue<Vector3>().y;
                    float footHeight = Mathf.Max(leftFootY, rightFootY);
                    totalFootHeight += footHeight;
                    footHeightSamples++;

                    // add to foot time
                    footTime += Time.deltaTime;
                }
        }

        System.Collections.IEnumerator MoveToWaypoint(Vector3 targetPos)
        {
            isMoving = true;
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;
            isMoving = false;
            //if (currentIndex == waypoints.Count - 1 && pathComplete)
            if(currentIndex == waypoints.Count - 1)
            {
                pathComplete = true;
                UnityEngine.Debug.Log("path complete");
                OnPathComplete.Invoke();
                SoundManager.PlaySound(SoundType.REWARDONE);
                yield return new WaitForSeconds(waitTime);
                ChangeScene("5. Success");
            }
        }

        void Update()
        {
            if(isFootRaised)
            {
                NeedleMoving();
            }

            if(keyboardControl)
            {
                if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    OnFootRaise(new InputAction.CallbackContext());
                }

                if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                {
                    OnFootLower(new InputAction.CallbackContext());
                }
            }
        }
        public void ChangeScene(string sceneName) 
        {
            SceneManager.LoadScene(sceneName);
        }

        private void OnDestroy()
        {
            if (footRaiseAction != null)
            {
                footRaiseAction.performed -= OnFootRaise;
                footRaiseAction.Disable();
            }
            if (footLowerAction != null)
            {
                footLowerAction.performed -= OnFootLower;
                footLowerAction.Disable();
            }
        }
    }
}