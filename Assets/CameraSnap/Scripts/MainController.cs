using UnityEngine;
using UnityEngine.InputSystem;


namespace CameraSnap
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Motion Tracking Settings")]
        public MotionTrackingConfiguration motionConfig;
        [SerializeField] private InputActionAsset inputActions;
        
        private CartController cart;
        private CameraMode cameraMode;
        private CameraPan cameraPan;

    // Input Actions
     
        private InputAction pelvisPositionAction;
        private InputAction weightShiftXAction;
        private InputAction leftHipAbductedAction;
        private InputAction rightHipAbductedAction;
        private InputAction footRaisedAction;
        private InputAction leftFootHeightAction;
        private InputAction rightFootHeightAction;

        private float defaultFootDistance = 0f;

        private void Awake()
        {
            cart = FindObjectOfType<CartController>();
            cameraMode = FindObjectOfType<CameraMode>();
            cameraPan = FindObjectOfType<CameraPan>();


            if (inputActions == null)
                inputActions = Resources.Load<InputActionAsset>("CapturyInputActions");


            var headMap = inputActions.FindActionMap("Head");
            var torsoMap = inputActions.FindActionMap("Torso");
            var footMap = inputActions.FindActionMap("Foot");

            pelvisPositionAction   = torsoMap?.FindAction("PelvisPosition");
            // torso weight shift action (continuous)
            weightShiftXAction     = torsoMap?.FindAction("WeightShiftX");

            leftHipAbductedAction  = footMap?.FindAction("LeftHipAbducted");
            rightHipAbductedAction = footMap?.FindAction("RightHipAbducted");
            footRaisedAction       = footMap?.FindAction("FootRaised");
            leftFootHeightAction = footMap.FindAction("LeftFootPosition");
            rightFootHeightAction = footMap.FindAction("RightFootPosition");
        }

        void Start()
        {
            Vector3 leftPos = leftFootHeightAction.ReadValue<Vector3>();
            Vector3 rightPos = rightFootHeightAction.ReadValue<Vector3>();

            Vector2 leftPos2D = new Vector2(leftPos.x, leftPos.z);
            Vector2 rightPos2D = new Vector2(rightPos.x, rightPos.z);
            defaultFootDistance = Vector2.Distance(leftPos2D, rightPos2D);
        }


        private void OnEnable()
        {
            pelvisPositionAction?.Enable();
            weightShiftXAction?.Enable();
            
            leftHipAbductedAction?.Enable();
            rightHipAbductedAction?.Enable();
            footRaisedAction?.Enable();
            leftFootHeightAction.Enable();
            rightFootHeightAction.Enable();
        }


        private void OnDisable()
        {
            pelvisPositionAction?.Disable();
            weightShiftXAction.Disable();
           
            leftHipAbductedAction?.Disable();
            rightHipAbductedAction?.Disable();
            footRaisedAction?.Disable();
            leftFootHeightAction.Disable();
            rightFootHeightAction.Disable();
        }


        private void Update()
        {
            if (motionConfig == null) return;

            HandleWeightShift();
            HandleSquat();
            HandleHipAbduction();
            HandleFootRaise();
           
        }

        //how far below neutral pelvis Y counts as a squat
        private float squatThreshold = 0.05f;
        private bool squatTriggered;

        private void HandleWeightShift()
        {
            float weightShiftX = weightShiftXAction.ReadValue<float>();

            if(Mathf.Abs(weightShiftX) > 0.1f)
            {
                string dataStr = $"WeightShiftX: {weightShiftX:F2}; Shifting: {((weightShiftX > 0) ? "Right" : "Left")}";
                DataLogger.Instance.LogInput("WeightShiftX", weightShiftX.ToString("F2"));
            }

            cameraPan?.ManualPan(weightShiftX);
        }


        private void HandleSquat()
        {
            float pelvisY = pelvisPositionAction.ReadValue<Vector3>().y;

            // Detect squat (below threshold)
            if (!squatTriggered && -pelvisY > squatThreshold && cart.CanStop())
            {
                cart.StopCart();
                squatTriggered = true;
                Debug.Log($"[Squat Detected] PelvisY=-{pelvisY:F3} (> {squatThreshold:F3})");
            }

            // Reset when returning above threshold
            if (squatTriggered && -pelvisY <= squatThreshold)
            {
                DataLogger.Instance.LogInput("PelvisSquat", pelvisY.ToString("F2"));
                squatTriggered = false;
                Debug.Log("[Squat Reset] Standing again");
            }
        }




        private void HandleHipAbduction()
        {
            if (!motionConfig.isHipAbductionTracked || cameraMode == null || cart == null) return;


            if (!cart.IsStopped()) return;        
            if (cameraMode.IsInCameraMode()) return;


            if (leftHipAbductedAction.WasPerformedThisFrame() ||
                rightHipAbductedAction.WasPerformedThisFrame())
            {
                cameraMode.TryToggleCameraMode();
                Debug.Log("[Hip Abduction] - Camera Mode toggled");
                 
            }

            // logger
            Vector3 leftPos = leftFootHeightAction.ReadValue<Vector3>();
            Vector3 rightPos = rightFootHeightAction.ReadValue<Vector3>();

            Vector2 leftPos2D = new Vector2(leftPos.x, leftPos.z);
            Vector2 rightPos2D = new Vector2(rightPos.x, rightPos.z);
            float currentDistance = Vector2.Distance(leftPos2D, rightPos2D);
            float abductionDistance = currentDistance - defaultFootDistance;

            string hipSide;
            if(leftPos2D.magnitude > rightPos2D.magnitude)
            {
                hipSide = "Left";
            } 
            else
            {
                hipSide = "Right";
            }

            string dataStr = $"Distance: {abductionDistance.ToString("F2")}; Side: {hipSide}";
            DataLogger.Instance.LogInput($"HipAbduction", dataStr);
        }


       
        private void HandleFootRaise()
        {
            if (!motionConfig.isFootRaiseTracked || footRaisedAction == null) return;


            if (!footRaisedAction.WasPerformedThisFrame()) return;


            Debug.Log("[Foot Raise] Detected");

            float leftFootY = leftFootHeightAction.ReadValue<Vector3>().y;
            float rightFootY = rightFootHeightAction.ReadValue<Vector3>().y;
            float footHeight = Mathf.Max(leftFootY, rightFootY);

            string dataSrt = $"FootHeight: {footHeight:F2}; Foot: {(leftFootY > rightFootY ? "Left" : "Right")}";
            DataLogger.Instance.LogInput("FootHeight", footHeight.ToString("F2"));

            // If we're currently in the main menu scene, use the SceneTransitionManager
            //  to start the game. 
            var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (sceneName == "MainMenu")
            {
                var stm = FindObjectOfType<SceneTransitionManager>();
                if (stm != null)
                {
                    stm.OnFootRaisedExternal();
                    Debug.Log("FootRaiseDetected-MovingtoNextScene");
                }
                else
                {
                    
                     Debug.Log("Error scene transition");
                }
                return;
            }


           
            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                Debug.Log("[Foot Raise] - Restarted Scene");
                return;
            }


           
            if (cameraMode != null && cameraMode.IsInCameraMode())
            {
                cameraMode.TryTakePhoto();
                Debug.Log("[Foot Raise] - Photo Taken");
            }
        }


       
    }
}



