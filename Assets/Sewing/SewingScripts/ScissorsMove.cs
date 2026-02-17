using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Sewing;

namespace Sewing {
    public class ScissorsMove : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        // List to hold waypoints (Empty GameObjects you set up in the scene)
        public List<Transform> waypoints = new List<Transform>();
        private int currentWaypointIndex = 0; // Tracks the current waypoint
        public int RepOne = 0;
        public int RepTwo = 0;
        private Animator animator;
        private bool movementAvailable = true;
        private InputAction leftHipAction;
        private InputAction rightHipAction;
        private InputAction leftFootHeightAction;
        private InputAction rightFootHeightAction;
        private float defaultFootDistance;
        //private InputAction footRaiseAction;

        public float moveSpeed = 3f; // Speed of movement
        public float rotationSpeed = 5f; // Speed of rotation

        public UnityEvent OnLastWaypoint;

        void Awake()
        {
            animator = GetComponent<Animator>();
            var actionMap = inputActions.FindActionMap("Foot");
            leftHipAction = actionMap.FindAction("LeftHipAbducted");
            rightHipAction = actionMap.FindAction("RightHipAbducted");
            leftFootHeightAction = actionMap.FindAction("LeftFootPosition");
            rightFootHeightAction = actionMap.FindAction("RightFootPosition");
            //footRaiseAction = actionMap.FindAction("FootRaise");

            /*footRaiseAction.performed += OnFootRaise;
            leftHipAction.performed += OnLeftHip;
            rightHipAction.performed += OnRightHip;*/
        }

        void Start()
        {
            Vector3 leftPos = leftFootHeightAction.ReadValue<Vector3>();
            Vector3 rightPos = rightFootHeightAction.ReadValue<Vector3>();

            Vector2 leftPos2D = new Vector2(leftPos.x, leftPos.z);
            Vector2 rightPos2D = new Vector2(rightPos.x, rightPos.z);
            defaultFootDistance = Vector2.Distance(leftPos2D, rightPos2D);
        }
    // Update is called once per frame
        public void ChangeScene(string sceneName)   
        {
            SceneManager.LoadScene(sceneName);
        }
    
        void Update()
        {
            if (currentWaypointIndex == waypoints.Count) 
            {
                SoundManager.PlaySound(SoundType.REWARDONE);
                StartCoroutine(SceneSwitch());
            }
        }

        private IEnumerator SceneSwitch()
        {
            yield return new WaitForSeconds(3f);
            ChangeScene("4. Sewing");
        }

        private void OnEnable()
        {
            leftHipAction.Enable();
            rightHipAction.Enable();
            leftFootHeightAction.Enable();
            rightFootHeightAction.Enable();
            //footRaiseAction.Enable();

            //footRaiseAction.performed += OnFootRaise;
            leftHipAction.performed += OnLeftHip;
            rightHipAction.performed += OnRightHip;
        }

        private void OnDisable()
        {
            leftHipAction.Disable();
            rightHipAction.Disable();
            leftFootHeightAction.Disable();
            rightFootHeightAction.Disable();
            //footRaiseAction.Disable();


            //footRaiseAction.performed -= OnFootRaise;
            leftHipAction.performed -= OnLeftHip;
            rightHipAction.performed -= OnRightHip;
        }
        /*private void OnFootRaise(InputAction.CallbackContext ctx)
        {
            if (currentWaypointIndex == waypoints.Count) {
                ChangeScene("4. Sewing");
        }
        }*/
        private void OnLeftHip(InputAction.CallbackContext ctx)
        {
            bool isLeft = true;
            CalculateAbductionDistance(isLeft);
            //DataLogger.Instance.LogInput("LeftHipAbducted", ctx.ReadValue<float>().ToString("F2"));
            if (movementAvailable == true && currentWaypointIndex < waypoints.Count)
            {
                movementAvailable = false;
                    RepOne++;
                    if(animator!= null) animator.SetTrigger("PlayAnimation");
                    StartCoroutine(MoveToWaypoint(waypoints[currentWaypointIndex])); // Start movement to waypoint
            }
        }
        private void OnRightHip(InputAction.CallbackContext ctx)
        {
            bool isLeft = false;
            CalculateAbductionDistance(isLeft);

            if (movementAvailable == true && currentWaypointIndex < waypoints.Count)
            {
                movementAvailable = false;
                    RepTwo++;
                    if (animator != null) animator.SetTrigger("PlayAnimation");
                    StartCoroutine(MoveToWaypoint(waypoints[currentWaypointIndex])); // Start movement to waypoint
            }
        }

        private IEnumerator MoveToWaypoint(Transform targetWaypoint)
        {
            SoundManager.PlaySound(SoundType.SCISSORS);
            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;
            Vector3 targetPosition = targetWaypoint.position;
            Quaternion targetRotation = targetWaypoint.rotation;

            float journeyLength = Vector3.Distance(startPosition, targetPosition);
            float startTime = Time.time;

            while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                // Smooth movement
                float distanceCovered = (Time.time - startTime) * moveSpeed;
                float fractionOfJourney = distanceCovered / journeyLength;

                transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

                // Smooth rotation
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, fractionOfJourney);

                yield return null; // Wait for the next frame
            }

            transform.position = targetPosition; // Ensure it exactly reaches the target position
            transform.rotation = targetRotation; // Ensure it exactly reaches the target rotation

            currentWaypointIndex++;
            movementAvailable = true;
            if (currentWaypointIndex == waypoints.Count) 
            {
                OnLastWaypoint.Invoke();
            }
        }

        void OnDestroy()
        {
            if (leftHipAction != null)
            {
                leftHipAction.performed -= OnLeftHip;
                leftHipAction.Disable();
            }

            if (rightHipAction != null)
            {
                rightHipAction.performed -= OnRightHip;
                rightHipAction.Disable();
            }
        }

        void CalculateAbductionDistance(bool isLeft)
        {
            Vector3 leftPos = leftFootHeightAction.ReadValue<Vector3>();
            Vector3 rightPos = rightFootHeightAction.ReadValue<Vector3>();

            Vector2 leftPos2D = new Vector2(leftPos.x, leftPos.z);
            Vector2 rightPos2D = new Vector2(rightPos.x, rightPos.z);
            float currentDistance = Vector2.Distance(leftPos2D, rightPos2D);
            float abductionDistance = currentDistance - defaultFootDistance;

            // Log the abduction distance for analysis
            string hipSide = isLeft ? "Left" : "Right";

            string dataStr = $"Distance: {abductionDistance.ToString("F2")}; Side: {hipSide}";
            DataLogger.Instance.LogInput($"HipAbduction", dataStr);
        }
    }
}