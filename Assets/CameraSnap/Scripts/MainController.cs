using UnityEngine;
using UnityEngine.InputSystem;




//THE ORDER OF MOVEMENT!: In game, player enters a slowdown zone where a camera ui appears. They need to squat to stop the cart
//Once squat is performed, they must do a hip abduction to enter camera mode. When in camera mode, they cannot exit the camera
//mode unless they squat to resume cart movement, then they can do a hip abduction to re-enter camera mode. In camera mode,
//It is likely that any lifting of the foot will count as a foot raised, which is for taking photographs. The player must
//be looking at the animal, and the camera overlay turns green, then they raise foot to capture photo and text will appear
// to state what animal is captured and a camera click sound will happen.. Then, the cart automatically resumes and they will
//auto-exit camera mode. 


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
        private InputAction weightShiftXAction;
        private InputAction pelvisPositionAction;
        private InputAction leftHipAbductedAction;
        private InputAction rightHipAbductedAction;
        private InputAction footRaisedAction;

        // Squat cooldown
        private bool canSquatToggle = true;
        private float lastSquatTime = 0f;

        void Awake()
        {
            cart = FindObjectOfType<CartController>();
            cameraMode = FindObjectOfType<CameraMode>();
            cameraPan = FindObjectOfType<CameraPan>();

            if (inputActions == null)
                inputActions = Resources.Load<InputActionAsset>("CapturyInputActions");

            var torsoMap = inputActions.FindActionMap("Torso");
            var footMap = inputActions.FindActionMap("Foot");

            weightShiftXAction     = torsoMap?.FindAction("WeightShiftX");
            pelvisPositionAction   = torsoMap?.FindAction("PelvisPosition");

            leftHipAbductedAction  = footMap?.FindAction("LeftHipAbducted");
            rightHipAbductedAction = footMap?.FindAction("RightHipAbducted");
            footRaisedAction       = footMap?.FindAction("FootRaised");
        }

        void OnEnable()
        {
            weightShiftXAction?.Enable();
            pelvisPositionAction?.Enable();
            leftHipAbductedAction?.Enable();
            rightHipAbductedAction?.Enable();
            footRaisedAction?.Enable();
        }

        void OnDisable()
        {
            weightShiftXAction?.Disable();
            pelvisPositionAction?.Disable();
            leftHipAbductedAction?.Disable();
            rightHipAbductedAction?.Disable();
            footRaisedAction?.Disable();
        }

        void Update()
        {
            if (!motionConfig) return;

            HandleWeightShift();
            HandleSquat();
            HandleHipAbduction();
            HandleFootRaise();
        }

        //  Weight Shift - Pan camera
        private void HandleWeightShift()
        {
            if (!motionConfig.enableTorsoModule || !motionConfig.isShiftTracked) return;

            float shift = weightShiftXAction.ReadValue<float>();
            if (Mathf.Abs(shift) > motionConfig.weightShiftThreshold)
            {
                float input = Mathf.Clamp(shift * motionConfig.torsoSensitivity, -1f, 1f);
                cameraPan?.ManualPan(input);
            }
        }

// Here, the code tracks whether the position of the player's pelvis has gone down. If it went down, it marks that as one squat
//The player is then required to stand back up before the game is ready to track another squat which is why I have this bool.

        // Track whether player returned to a standing position
private bool isSquatReady = true;

private void HandleSquat()
{
    if (!motionConfig.enableTorsoModule || cart == null) return;

    float pelvisY = pelvisPositionAction.ReadValue<Vector3>().y;
    float squatThreshold = motionConfig.footRaiseThreshold;  // In the config
    float standUpThreshold = squatThreshold + 0.05f;         // Small buffer to confirm standing

    //  Player must be standing above threshold before squat is allowed again
    if (!isSquatReady && pelvisY > standUpThreshold)
    {
        isSquatReady = true;
        // Debug Player returned to standing. Squat is ready again;
    }

    //  If pelvis goes low AND squat is allowed - Trigger once
    if (isSquatReady && pelvisY < squatThreshold && cart.CanStop())
    {
        if (cart.IsStopped())
            cart.ResumeCart();
        else
            cart.StopCart();

        Debug.Log($"Squat Triggered Once → PelvisY={pelvisY}, CartStopped={cart.IsStopped()}");

        isSquatReady = false; // Block until standing happens again
    }
}

        // Hip Abduction - Toggle camera mode
       private void HandleHipAbduction()
{
    if (!motionConfig.isHipAbductionTracked || cameraMode == null || cart == null) return;

    // Hip abduction should only toggle camera mode when cart is not moving
    if (!cart.IsStopped()) return;

    // we ignore this if Camera Mode is active (to avoid conflict with taking pictures)
    if (cameraMode.IsInCameraMode()) return;

    // Detect left or right hip abduction once
    if (leftHipAbductedAction.WasPerformedThisFrame() ||
        rightHipAbductedAction.WasPerformedThisFrame())
    {
        cameraMode.TryToggleCameraMode();
        Debug.Log("Hip Abduction → Camera Mode Toggled");
    }
}


        //  Foot Raise - Take photo or restart at end
        private void HandleFootRaise()
        {
            if (!motionConfig.isFootRaiseTracked) return;
            if (!footRaisedAction.WasPerformedThisFrame()) return;

            Debug.Log("Foot Raise Detected");

            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                Debug.Log("Game restarted with foot raise");
                return;
            }

            if (cameraMode != null && cameraMode.IsInCameraMode())
            {
                cameraMode.TryTakePhoto();
                Debug.Log("Foot raise → photo taken");
            }
        }
    }
}
