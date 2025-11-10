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
    if (pelvisPositionAction != null)
    {
        neutralPelvisY = pelvisPositionAction.ReadValue<Vector3>().y;
        Debug.Log($"[Squat] Calibrated neutral pelvis Y = {neutralPelvisY:F3}");
    }
   
    // Calibrate neutral WeightShiftX at startup if available
    if (weightShiftXAction != null && motionConfig != null && motionConfig.enableTorsoModule && motionConfig.isShiftTracked)
    {
        neutralWeightShiftOffset = weightShiftXAction.ReadValue<float>();
        Debug.Log($"[Calibration] WeightShift neutral offset = {neutralWeightShiftOffset:F3}");
    }


}


// Public method to recalibrate neutral WeightShiftX at runtime
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
                float ws = weightShiftXAction.ReadValue<float>() - neutralWeightShiftOffset;
                if (Mathf.Abs(ws) <= motionConfig.neutralZoneWidth)
                {
                    cameraPan?.ManualPan(0f);
                    return;
                }


                // Proportional pan based on continuous WeightShiftX
                float input = Mathf.Clamp(ws * motionConfig.torsoSensitivity, -1f, 1f);
                cameraPan?.ManualPan(input);
                return;
            }
            // If we reach here there is no continuous WeightShiftX
            cameraPan?.ManualPan(0f);
        }

        // Immediate-performed callback for the weight-shift action so panning reacts
        // as soon as the action produces a value (helps responsiveness).
        private void OnWeightShiftPerformed(InputAction.CallbackContext ctx)
        {
            if (motionConfig == null) return;
            if (!motionConfig.enableTorsoModule || !motionConfig.isShiftTracked) return;
            if (cameraPan == null) return;
 float ws = weightShiftXAction.ReadValue<float>() - neutralWeightShiftOffset;
            float ws = ctx.ReadValue<float>() - neutralWeightShiftOffset;
            if (Mathf.Abs(ws) <= motionConfig.neutralZoneWidth)
            {
                cameraPan.ManualPan(0f);
                return;
            }

            float input = Mathf.Clamp(ws * motionConfig.torsoSensitivity, -1f, 1f);
            cameraPan.ManualPan(input);
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



