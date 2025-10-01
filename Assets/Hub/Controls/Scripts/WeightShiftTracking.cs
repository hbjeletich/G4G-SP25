using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Captury;
using UnityEngine.InputSystem.LowLevel;

public class WeightShiftTracking : MonoBehaviour
{
    [Header("Skeleton Tracking")]
    [SerializeField] private Transform pelvis;

    [Header("Joint Names")]
    [SerializeField] private string pelvisName = "Hips";

    [Header("Weight Shift Settings")]
    [SerializeField] private bool enableWeightShiftTracking = true;
    [SerializeField] private float weightShiftThreshold = 0.15f;
    [SerializeField] private float neutralZoneWidth = 0.05f;

    [Header("Squat Tracking Settings")]
    [SerializeField] private bool enableSquatTracking = false;
    [SerializeField] private float squatThresholdPercentage = 0.075f; // how much should pelvis drop
    [SerializeField] private float standingThreshold = 0.2f;

    [Header("Calibration")]
    [SerializeField] private float calibrationDelay = 2.0f; // time to wait before calibrating
    [SerializeField] private int calibrationFrames = 30;

    // weight shift variables
    private float neutralPelvisPosition = 0f;
    private bool isShiftingLeft = false;
    private bool isShiftingRight = false;

    // squat tracking variables
    private float standingPelvisHeight = 0f;
    private bool isInSquat = false;
    private bool wasInSquat = false;

    // general variables
    private bool isCalibrated = false;
    private CapturyInput capturyInput;
    private Vector3 initialPelvisPosition; // for calibration

    [SerializeField] private bool debugMode = true;

    private void Start()
    {
        capturyInput = InputSystem.GetDevice<CapturyInput>();

        if (capturyInput == null)
        {
            CapturyInput.Register();
            capturyInput = InputSystem.AddDevice<CapturyInput>();
            Debug.Log("MotionTracking: Created new CapturyInput device");
        }
        else
        {
            Debug.Log("MotionTracking: Found existing CapturyInput device");
        }

        CapturyNetworkPlugin networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            networkPlugin.SkeletonFound -= OnSkeletonFound;
            networkPlugin.SkeletonFound += OnSkeletonFound;
        }
        else
        {
            Debug.LogError("MotionTracking: Could not find CapturyNetworkPlugin!");
        }
    }

    private void OnDestroy()
    {
        CapturyNetworkPlugin networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            networkPlugin.SkeletonFound -= OnSkeletonFound;
        }
    }

    private void OnSkeletonFound(CapturySkeleton skeleton)
    {
        Debug.Log("MotionTracking received Skeleton: " + skeleton.name);

        // wait until SetTargetSkeleton() finishes
        skeleton.OnSkeletonSetupComplete += OnSkeletonSetupComplete;
    }

    private void OnSkeletonSetupComplete(CapturySkeleton skeleton)
    {
        Debug.Log("Skeleton setup complete for motion tracking!");

        pelvis = FindJointByExactName(skeleton, pelvisName);

        if (pelvis == null)
        {
            Debug.LogError("MotionTracking: Could not find the pelvis bone! Check the name in the Inspector.");
        }
        else
        {
            Debug.Log("MotionTracking: Found pelvis: " + pelvis.name);
            // start calibration coroutine
            StartCoroutine(CalibrateNeutralPosition());
        }
    }

    IEnumerator CalibrateNeutralPosition()
    {
        // calibrate to find neutral position
        Debug.Log("Starting motion tracking calibration. Please stand in neutral position...");
        yield return new WaitForSeconds(calibrationDelay); // give time for user to get in position

        if (pelvis != null)
        {
            // take average of several frames for more stable calibration
            Vector3 sum = Vector3.zero;

            for (int i = 0; i < calibrationFrames; i++)
            {
                if (pelvis != null)
                {
                    sum += pelvis.position;
                    yield return null; // wait a frame
                }
            }

            initialPelvisPosition = sum / calibrationFrames;

            // set up weight shift calibration
            if (enableWeightShiftTracking)
            {
                neutralPelvisPosition = initialPelvisPosition.x;
            }

            // set up squat calibration
            if (enableSquatTracking)
            {
                standingPelvisHeight = pelvis.localPosition.y;
            }

            isCalibrated = true;

            Debug.Log($"Motion tracking calibration complete.");
            if (enableWeightShiftTracking)
                Debug.Log($"Weight shift neutral position: {neutralPelvisPosition}");
            if (enableSquatTracking)
                Debug.Log($"Standing height: {standingPelvisHeight:F3}m");
        }
    }

    private Transform FindJointByExactName(CapturySkeleton skeleton, string jointName)
    {
        foreach (var joint in skeleton.joints)
        {
            if (joint.name == jointName)
            {
                Debug.Log("Found joint: " + joint.name);
                return joint.transform;
            }
        }
        Debug.LogError($"Joint {jointName} not found in CapturySkeleton!");
        return null;
    }

    private void Update()
    {
        if (!isCalibrated || pelvis == null || capturyInput == null) return;

        CapturyInputState state = new CapturyInputState();

        if (enableWeightShiftTracking)
        {
            UpdateWeightShiftTracking(ref state);
        }

        if (enableSquatTracking)
        {
            UpdateSquatTracking(ref state);
        }

        // send state to input system
        InputSystem.QueueStateEvent(capturyInput, state);
    }

    private void UpdateWeightShiftTracking(ref CapturyInputState state)
    {
        // calculate lateral shift from neutral position
        float currentPosition = pelvis.position.x;
        float shiftAmount = currentPosition - neutralPelvisPosition;

        // normalize shift amount to [-1, 1] range for the axis
        state.weightShiftX = Mathf.Clamp(shiftAmount / weightShiftThreshold, -1f, 1f);

        // detect left/right weight shifts outside the neutral zone
        bool isInNeutralZone = Mathf.Abs(shiftAmount) < neutralZoneWidth;

        // check for left shift
        if (shiftAmount < -neutralZoneWidth && !isShiftingLeft)
        {
            isShiftingLeft = true;
            isShiftingRight = false;
            state.weightShiftLeft = 1.0f;
            state.weightShiftRight = 0.0f;

            if (debugMode)
            {
                Debug.Log("Weight shifted LEFT");
            }
        }
        // check for right shift
        else if (shiftAmount > neutralZoneWidth && !isShiftingRight)
        {
            isShiftingRight = true;
            isShiftingLeft = false;
            state.weightShiftRight = 1.0f;
            state.weightShiftLeft = 0.0f;

            if (debugMode)
            {
                Debug.Log("Weight shifted RIGHT");
            }
        }
        // check for return to neutral
        else if (isInNeutralZone && (isShiftingLeft || isShiftingRight))
        {
            isShiftingLeft = false;
            isShiftingRight = false;
            state.weightShiftLeft = 0.0f;
            state.weightShiftRight = 0.0f;

            if (debugMode)
            {
                Debug.Log("Weight returned to NEUTRAL");
            }
        }
        // maintain current button states when not changing
        else
        {
            state.weightShiftLeft = isShiftingLeft ? 1.0f : 0.0f;
            state.weightShiftRight = isShiftingRight ? 1.0f : 0.0f;
        }
    }

    private void UpdateSquatTracking(ref CapturyInputState state)
    {
        // calculate how much the pelvis drops
        float currentPelvisHeight = pelvis.localPosition.y;
        float squatDepth = standingPelvisHeight - currentPelvisHeight;
        float squatThreshold = standingPelvisHeight * squatThresholdPercentage;

        wasInSquat = isInSquat;
        isInSquat = squatDepth > squatThreshold;

        // y position tracking for continuous squat depth
        state.squatTrackingY = Mathf.Clamp01(squatDepth / (standingPelvisHeight * 0.3f)); // Normalize to 0-1 range

        // squat started
        if (isInSquat && !wasInSquat)
        {
            state.squatStarted = 1.0f;
            if (debugMode)
            {
                Debug.Log("Squat started!");
            }
        }

        // squat complete
        if (!isInSquat && wasInSquat && squatDepth <= standingThreshold)
        {
            state.squatCompleted = 1.0f;
            if (debugMode)
            {
                Debug.Log("Squat completed!");
            }
        }
    }

    // helper method to recalibrate the neutral position if needed
    public void Recalibrate()
    {
        isCalibrated = false;
        StartCoroutine(CalibrateNeutralPosition());
    }
}