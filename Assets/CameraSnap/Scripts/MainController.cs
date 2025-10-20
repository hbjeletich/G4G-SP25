using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace CameraSnap
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Motion Tracking Configuration")]
        public MotionTrackingConfiguration motionConfig;

        [Header("Captury Input Settings")]
        [Tooltip("If left empty, will auto-load CapturyInputActions from Resources.")]
        [SerializeField] private InputActionAsset inputActions;

        private CartController cart;
        private CameraMode cameraMode;
        private CameraPan cameraPan;

        // Captury input actions
        private InputAction weightShiftXAction;
        private InputAction pelvisPositionAction;
        private InputAction leftHipAbductedAction;
        private InputAction rightHipAbductedAction;
        private InputAction footRaisedAction;

        // Control flags
        private bool canToggleStop = true;
        private float lastSquatTime;

        void Awake()
        {
            // Auto-find core components in the scene
            cart = FindObjectOfType<CartController>();
            cameraPan = FindObjectOfType<CameraPan>();
            cameraMode = FindObjectOfType<CameraMode>();

            if (cart == null)
                Debug.LogWarning("[PlayerController] No CartController found in scene!");
            if (cameraMode == null)
                Debug.LogWarning("[PlayerController] No CameraMode found in scene!");
            if (cameraPan == null)
                Debug.LogWarning("[PlayerController] No CameraPan found in scene!");

            // Auto-load Captury input asset if not assigned
            if (inputActions == null)
            {
                inputActions = Resources.Load<InputActionAsset>("CapturyInputActions");
                if (inputActions == null)
                {
                    Debug.LogWarning("[PlayerController] Could not find CapturyInputActions asset in Resources folder!");
                    enabled = false;
                    return;
                }
            }

            var torsoMap = inputActions.FindActionMap("Torso");
            var footMap = inputActions.FindActionMap("Foot");

            if (torsoMap != null)
            {
                weightShiftXAction = torsoMap.FindAction("WeightShiftX");
                pelvisPositionAction = torsoMap.FindAction("PelvisPosition");
            }

            if (footMap != null)
            {
                leftHipAbductedAction = footMap.FindAction("LeftHipAbducted");
                rightHipAbductedAction = footMap.FindAction("RightHipAbducted");
                footRaisedAction = footMap.FindAction("FootRaised");
            }
        }

        void OnEnable()
        {
            EnableCapturyActions();
        }

        void OnDisable()
        {
            DisableCapturyActions();
        }

        private void EnableCapturyActions()
        {
            if (motionConfig == null)
            {
                Debug.LogWarning("[PlayerController] No MotionTrackingConfiguration assigned!");
                return;
            }

            if (motionConfig.enableTorsoModule && weightShiftXAction != null)
                weightShiftXAction.Enable();

            if (motionConfig.enableTorsoModule && pelvisPositionAction != null)
                pelvisPositionAction.Enable();

            if (motionConfig.isHipAbductionTracked && leftHipAbductedAction != null)
            {
                leftHipAbductedAction.Enable();
                leftHipAbductedAction.performed += OnHipAbducted;
            }

            if (motionConfig.isHipAbductionTracked && rightHipAbductedAction != null)
            {
                rightHipAbductedAction.Enable();
                rightHipAbductedAction.performed += OnHipAbducted;
            }

            if (motionConfig.isFootRaiseTracked && footRaisedAction != null)
            {
                footRaisedAction.Enable();
                footRaisedAction.performed += OnFootRaised;
            }
        }

        private void DisableCapturyActions()
        {
            if (leftHipAbductedAction != null)
                leftHipAbductedAction.performed -= OnHipAbducted;
            if (rightHipAbductedAction != null)
                rightHipAbductedAction.performed -= OnHipAbducted;
            if (footRaisedAction != null)
                footRaisedAction.performed -= OnFootRaised;

            if (weightShiftXAction != null) weightShiftXAction.Disable();
            if (pelvisPositionAction != null) pelvisPositionAction.Disable();
            if (leftHipAbductedAction != null) leftHipAbductedAction.Disable();
            if (rightHipAbductedAction != null) rightHipAbductedAction.Disable();
            if (footRaisedAction != null) footRaisedAction.Disable();
        }

        void Update()
        {
            HandleLookInput();
            HandleSquatInput();
        }

        private void HandleLookInput()
        {
            if (cameraPan == null || motionConfig == null) return;
            if (!motionConfig.enableTorsoModule || !motionConfig.isShiftTracked) return;

            float shiftValue = weightShiftXAction != null ? weightShiftXAction.ReadValue<float>() : 0f;
            float horizontalInput = 0f;

            if (Mathf.Abs(shiftValue) > motionConfig.weightShiftThreshold)
            {
                horizontalInput = Mathf.Clamp(
                    shiftValue * motionConfig.torsoSensitivity,
                    -1f,
                    1f
                );
            }

            cameraPan.ManualPan(horizontalInput);
        }

        private void HandleSquatInput()
        {
            if (cart == null || pelvisPositionAction == null || motionConfig == null) return;
            if (!motionConfig.enableTorsoModule) return;

            Vector3 pelvisPos = pelvisPositionAction.ReadValue<Vector3>();

            if (pelvisPos.y < motionConfig.footRaiseThreshold && canToggleStop)
            {
                if (cart.CanStop())
                {
                    if (cart.IsStopped()) cart.ResumeCart();
                    else cart.StopCart();

                    canToggleStop = false;
                    lastSquatTime = Time.time;
                }
            }

            if (!canToggleStop && Time.time - lastSquatTime > motionConfig.calibrationDelay)
                canToggleStop = true;
        }

        private void OnHipAbducted(InputAction.CallbackContext ctx)
        {
            if (motionConfig == null || !motionConfig.isHipAbductionTracked) return;
            if (cameraMode != null && cart != null && cart.IsStopped())
                cameraMode.TryToggleCameraMode();
        }

       private void OnFootRaised(InputAction.CallbackContext ctx)
{
    if (motionConfig == null || !motionConfig.isFootRaiseTracked) return;

    //  Check if the game is currently paused (end summary active)
    if (Time.timeScale == 0f)
    {
        Debug.Log("[PlayerController] Foot raise detected - restarting game.");
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        return;
    }

    //  Normal in-game foot raise (photo capture)
    if (cameraMode != null && cameraMode.IsInCameraMode())
        cameraMode.TryTakePhoto();
}

    }
}
