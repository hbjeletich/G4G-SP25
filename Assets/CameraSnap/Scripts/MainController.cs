using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraSnap
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Motion Tracking Settings")]
        public MotionTrackingConfiguration motionConfig;
        [SerializeField] private InputActionAsset inputActions;

        [Header("Settings")]
        [SerializeField] private float panSpeed = 3f;           
        [SerializeField] private float shiftThreshold = 0.25f;  // Lean amount to trigger left/right

        private CartController cart;
        private CameraMode cameraMode;
        private CameraPan cameraPan;

        // Input Actions
        private InputAction weightShiftLeftAction;
private InputAction weightShiftRightAction;
        private InputAction pelvisPositionAction;
        private InputAction leftHipAbductedAction;
        private InputAction rightHipAbductedAction;
        private InputAction footRaisedAction;

        
       // private bool isSquatReady = true;
       // private float standingPelvisY = -999f; // Calibration baseline
       // private const float squatDropAmount = 0.15f; // How far down to count as squat

       
       // private enum ShiftState { Center, Left, Right }
       // private ShiftState currentShiftState = ShiftState.Center;

        private void Awake()
        {
            cart = FindObjectOfType<CartController>();
            cameraMode = FindObjectOfType<CameraMode>();
            cameraPan = FindObjectOfType<CameraPan>();

            if (inputActions == null)
                inputActions = Resources.Load<InputActionAsset>("CapturyInputActions");

            var torsoMap = inputActions.FindActionMap("Torso");
            var footMap = inputActions.FindActionMap("Foot");

            weightShiftLeftAction  = torsoMap?.FindAction("WeightShiftLeft");
    weightShiftRightAction = torsoMap?.FindAction("WeightShiftRight");
            pelvisPositionAction   = torsoMap?.FindAction("PelvisPosition");

            leftHipAbductedAction  = footMap?.FindAction("LeftHipAbducted");
            rightHipAbductedAction = footMap?.FindAction("RightHipAbducted");
            footRaisedAction       = footMap?.FindAction("FootRaised");
        }

        private void OnEnable()
        {
           weightShiftLeftAction?.Enable();
    weightShiftRightAction?.Enable();
            pelvisPositionAction?.Enable();
            leftHipAbductedAction?.Enable();
            rightHipAbductedAction?.Enable();
            footRaisedAction?.Enable();
        }

        private void OnDisable()
        {
            weightShiftLeftAction?.Disable();
    weightShiftRightAction?.Disable();
            pelvisPositionAction?.Disable();
            leftHipAbductedAction?.Disable();
            rightHipAbductedAction?.Disable();
            footRaisedAction?.Disable();
        }

        private void Update()
        {
            if (motionConfig == null) return;

            HandleWeightShift();
            HandleSquat();
            HandleHipAbduction();
            HandleFootRaise();
        }

       private void HandleWeightShift()
{
    if (!motionConfig.enableTorsoModule || cameraPan == null)
        return;

    if (weightShiftLeftAction != null && weightShiftLeftAction.WasPerformedThisFrame())
    {
        cameraPan.ManualPan(-panSpeed * Time.deltaTime);
        Debug.Log("[WeightShift] Left lean detected");
    }
    else if (weightShiftRightAction != null && weightShiftRightAction.WasPerformedThisFrame())
    {
        cameraPan.ManualPan(panSpeed * Time.deltaTime);
        Debug.Log("[WeightShift] Right lean detected");
    }
}

[SerializeField, Tooltip("How far below neutral pelvis Y counts as a squat (meters)")]
private float squatThreshold = 0.10f;

private float neutralPelvisY;
private bool squatTriggered;

private void Start()
{
    if (pelvisPositionAction != null)
    {
        neutralPelvisY = pelvisPositionAction.ReadValue<Vector3>().y;
        Debug.Log($"[Squat] Calibrated neutral pelvis Y = {neutralPelvisY:F3}");
    }
}

private void HandleSquat()
{
    if (!motionConfig.enableTorsoModule || cart == null || pelvisPositionAction == null)
        return;

    float pelvisY = pelvisPositionAction.ReadValue<Vector3>().y;

    // Detect squat (below neutral - threshold)
    if (!squatTriggered && pelvisY < neutralPelvisY - squatThreshold && cart.CanStop())
    {
        cart.StopCart();
        squatTriggered = true;
        Debug.Log($"[Squat Detected] PelvisY={pelvisY:F3} (< {neutralPelvisY - squatThreshold:F3})");
    }

    // Reset when returning above threshold
    if (squatTriggered && pelvisY >= neutralPelvisY - squatThreshold * 0.5f)
    {
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
        }

       
        private void HandleFootRaise()
        {
            if (!motionConfig.isFootRaiseTracked || footRaisedAction == null) return;

            if (!footRaisedAction.WasPerformedThisFrame()) return;

            Debug.Log("[Foot Raise] Detected");

           
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
