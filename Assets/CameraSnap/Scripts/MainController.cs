using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace CameraSnap
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Captury Input Settings")]
        [Tooltip("If left empty, will auto-load CapturyInputActions from Resources.")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("Sensitivity Settings")]
        public float squatThreshold = 0.2f;
        public float weightShiftSensitivity = 30f;
        public float squatCooldown = 1.0f;

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
                    Debug.LogWarning("[PlayerController] Could not find CapturyInputActions asset in Resources folder!");
            }
            // If using keyboard, don't enable Captury input
if (inputActions == null)
{
    enabled = false;
    return;
}

            if (inputActions != null)
            {
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
            if (weightShiftXAction != null) weightShiftXAction.Enable();
            if (pelvisPositionAction != null) pelvisPositionAction.Enable();

            if (leftHipAbductedAction != null)
            {
                leftHipAbductedAction.Enable();
                leftHipAbductedAction.performed += OnHipAbducted;
            }

            if (rightHipAbductedAction != null)
            {
                rightHipAbductedAction.Enable();
                rightHipAbductedAction.performed += OnHipAbducted;
            }

            if (footRaisedAction != null)
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
            if (cameraPan == null) return;

            float shiftValue = weightShiftXAction != null ? weightShiftXAction.ReadValue<float>() : 0f;
            float horizontalInput = 0f;

            if (Mathf.Abs(shiftValue) > 0.05f) // avoid jitter
                horizontalInput = Mathf.Clamp(shiftValue * weightShiftSensitivity, -1f, 1f);

            cameraPan.ManualPan(horizontalInput);
        }

        private void HandleSquatInput()
        {
            if (cart == null || pelvisPositionAction == null) return;

            Vector3 pelvisPos = pelvisPositionAction.ReadValue<Vector3>();

            if (pelvisPos.y < squatThreshold && canToggleStop)
            {
                if (cart.CanStop())
                {
                    if (cart.IsStopped()) cart.ResumeCart();
                    else cart.StopCart();

                    canToggleStop = false;
                    lastSquatTime = Time.time;
                }
            }

            if (!canToggleStop && Time.time - lastSquatTime > squatCooldown)
                canToggleStop = true;
        }

        private void OnHipAbducted(InputAction.CallbackContext ctx)
        {
            if (cameraMode != null && cart != null && cart.IsStopped())
            {
                cameraMode.TryToggleCameraMode();
            }
        }

        private void OnFootRaised(InputAction.CallbackContext ctx)
        {
            if (cameraMode != null && cameraMode.IsInCameraMode())
            {
                cameraMode.TryTakePhoto();
            }
        }
    }
}
