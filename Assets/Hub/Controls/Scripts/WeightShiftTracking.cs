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
    [SerializeField] private float weightShiftThreshold = 0.15f; // adjust based on testing
    [SerializeField] private float neutralZoneWidth = 0.05f; // dead zone around center
    [SerializeField] private float calibrationDelay = 2.0f; // time to wait before calibrating
    [SerializeField] private int calibrationFrames = 30; // frames to average for calibration

    private float neutralPelvisPosition = 0f;
    private bool isShiftingLeft = false;
    private bool isShiftingRight = false;
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
        }
        else
        {
        }

        CapturyNetworkPlugin networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            networkPlugin.SkeletonFound -= OnSkeletonFound;
            networkPlugin.SkeletonFound += OnSkeletonFound;
        }
        else
        {
            Debug.LogError("WeightShiftTracking: Could not find CapturyNetworkPlugin!");
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
        Debug.Log("WeightShiftTracking received Skeleton: " + skeleton.name);

        // wait until SetTargetSkeleton() finishes
        skeleton.OnSkeletonSetupComplete += OnSkeletonSetupComplete;
    }

    private void OnSkeletonSetupComplete(CapturySkeleton skeleton)
    {
        Debug.Log("Skeleton setup complete for weight shift tracking!");

        pelvis = FindJointByExactName(skeleton, pelvisName);

        if (pelvis == null)
        {
            Debug.LogError("WeightShiftTracking: Could not find the pelvis bone! Check the name in the Inspector.");
        }
        else
        {
            Debug.Log("WeightShiftTracking: Found pelvis: " + pelvis.name);
            // start calibration coroutine
            StartCoroutine(CalibrateNeutralPosition());
        }
    }

    IEnumerator CalibrateNeutralPosition()
    {
        // calibrate to find neutral position
        Debug.Log("Starting weight shift calibration. Please stand in neutral position...");
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
            neutralPelvisPosition = initialPelvisPosition.x;
            isCalibrated = true;

            Debug.Log($"Weight shift calibration complete. Neutral position: {neutralPelvisPosition}");
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

        // calculate lateral shift from neutral position
        float currentPosition = pelvis.position.x;
        float shiftAmount = currentPosition - neutralPelvisPosition;

        // debug info
        if (debugMode && Time.frameCount % 30 == 0)
        {
            //Debug.Log($"Weight shift: {shiftAmount:F3}m from neutral");
        }

        CapturyInputState state = new CapturyInputState();

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

        // give info to input system
        InputSystem.QueueStateEvent(capturyInput, state);
    }

    // helper method to recalibrate the neutral position if needed
    public void Recalibrate()
    {
        isCalibrated = false;
        StartCoroutine(CalibrateNeutralPosition());
    }
}