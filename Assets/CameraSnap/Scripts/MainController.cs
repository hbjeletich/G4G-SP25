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
    private InputAction headPositionAction;
    private InputAction weightShiftXAction;
    // Neutral sampled offset for WeightShiftX to remove steady bias
    private float neutralWeightShiftOffset = 0f;
    private InputAction leftHipAbductedAction;
    private InputAction rightHipAbductedAction;
    private InputAction footRaisedAction;


       
   


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


            headPositionAction     = headMap?.FindAction("HeadPosition");
            pelvisPositionAction   = torsoMap?.FindAction("PelvisPosition");


            // torso weight shift action (continuous)
            weightShiftXAction     = torsoMap?.FindAction("WeightShiftX");


            leftHipAbductedAction  = footMap?.FindAction("LeftHipAbducted");
            rightHipAbductedAction = footMap?.FindAction("RightHipAbducted");
            footRaisedAction       = footMap?.FindAction("FootRaised");
        }


        private void OnEnable()
        {
            pelvisPositionAction?.Enable();
            headPositionAction?.Enable();
            weightShiftXAction?.Enable();
            // react immediately to weight-shift value changes when supported by the action
            if (weightShiftXAction != null)
                weightShiftXAction.performed += OnWeightShiftPerformed;
            
            leftHipAbductedAction?.Enable();
            rightHipAbductedAction?.Enable();
            footRaisedAction?.Enable();
        }


        private void OnDisable()
        {
            pelvisPositionAction?.Disable();
            headPositionAction?.Disable();
            if (weightShiftXAction != null)
            {
                weightShiftXAction.performed -= OnWeightShiftPerformed;
                weightShiftXAction.Disable();
            }
           
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


       




[SerializeField, Tooltip("How far below neutral pelvis Y counts as a squat")]
private float squatThreshold = 0.10f;


private float neutralPelvisY;
private bool squatTriggered;


private void Start()
{
    //calibrate pelvis position for the squatting to work better. Before, it would think the player was naturally squatting.
    if (pelvisPositionAction != null)
    {
        neutralPelvisY = pelvisPositionAction.ReadValue<Vector3>().y;
        Debug.Log($"[Squat] Calibrated neutral pelvis Y = {neutralPelvisY:F3}");
    }
   
    // Calibrate neutral WeightShiftX at startup if available. To know what the player is like standing still.. 
    //This helps make the camera stay still when the player is, but if it is calibrated wrong, it makes playing the game
    //hard because the player is fighting with the camera. 
    if (weightShiftXAction != null && motionConfig != null && motionConfig.enableTorsoModule && motionConfig.isShiftTracked)
    {
        neutralWeightShiftOffset = weightShiftXAction.ReadValue<float>();
        Debug.Log($"[Calibration] WeightShift neutral offset = {neutralWeightShiftOffset:F3}");
    }


}


// Public method to recalibrate neutral WeightShiftX at runtime. Recalibration is to adjust to the players position standing still, this helps
//make it move around less. Players naturally move around as they play, so the calibration at the start would not work throughout.
public void RecalibrateWeightShiftNeutral()
{
    if (weightShiftXAction == null)
    {
        Debug.LogWarning("[Calibration] Cannot recalibrate WeightShift: action missing.");
        return;
    }


    neutralWeightShiftOffset = weightShiftXAction.ReadValue<float>();
    Debug.Log($"[Calibration] Recalibrated WeightShift neutral offset = {neutralWeightShiftOffset:F3}");
}


        private void HandleWeightShift()
        {
            // Use torso module weight-shift to drive lateral camera panning
            if (motionConfig == null) return;
            if (!motionConfig.enableTorsoModule || !motionConfig.isShiftTracked) return;

            // Prefer continuous WeightShiftX if available
            if (weightShiftXAction != null)
            {
                // Debug.Log($"[CartWeightShift] Continuous input value={weightShiftXAction.ReadValue<float>():F3}");
                ApplyWeightShift(weightShiftXAction.ReadValue<float>());
                return;
            }

            // If we reach here there is no continuous WeightShiftX
            cameraPan?.ManualPan(0f);
        }

        // Immediate-performed callback for the weight-shift action so panning reacts
        // as soon as the action produces a value (helps responsiveness).
        private void OnWeightShiftPerformed(InputAction.CallbackContext ctx)
        {
            //ApplyWeightShift(ctx.ReadValue<float>());
        }

        
        private void ApplyWeightShift(float rawWs)
        {
            if (motionConfig == null || cameraPan == null) return;
            if (!motionConfig.enableTorsoModule || !motionConfig.isShiftTracked) return;

            // float ws = rawWs;// - neutralWeightShiftOffset;
            // float normalizedDeadzone = motionConfig.neutralZoneWidth;

            // // Debug: report normalized weight-shift and current lean state
            // string leanState = Mathf.Abs(ws) <= normalizedDeadzone ? "Neutral" : (ws < 0f ? "Left" : "Right");
            // Debug.Log($"[WeightShift] value={ws:F3}, state={leanState}, deadzone={normalizedDeadzone:F3}");

            // if (Mathf.Abs(ws) <= normalizedDeadzone)
            // {
            //     cameraPan.ManualPan(0f);
            //     return;
            // }

            // float input = Mathf.Clamp(ws, -1f, 1f);
            // cameraPan.ManualPan(input);
            cameraPan?.ManualPan(rawWs);
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



