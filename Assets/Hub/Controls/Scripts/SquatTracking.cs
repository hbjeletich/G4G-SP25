using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Captury;
using UnityEngine.InputSystem.LowLevel;

public class SquatTracking : MonoBehaviour
{

    private Transform pelvis;
    [SerializeField] private string pelvisName = "Hips";

    [SerializeField] private float squatThresholdPercentage = 0.075f; // how much should pelvis drop
    [SerializeField] private float standingThreshold = 0.2f; // close to standing to be complete
    [SerializeField] private float calibrationDelay = 2.0f;

    private float standingPelvisHeight;
    private bool isCalibrated = false;

    private bool isInSquat = false;
    private bool wasInSquat = false;

    private CapturyInput capturyInput;

    private void Start()
    {
        capturyInput = InputSystem.GetDevice<CapturyInput>();

        if (capturyInput == null)
        {
            CapturyInput.Register();
            capturyInput = InputSystem.AddDevice<CapturyInput>();
        }

        CapturyNetworkPlugin networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            networkPlugin.SkeletonFound -= OnSkeletonFound;
            networkPlugin.SkeletonFound += OnSkeletonFound;
        }
        else
        {
            Debug.LogError("SquatTracking: Could not find CapturyNetworkPlugin!");
        }
    }

    private void OnDestroy()
    {
        // OnDestroy -- useful for scene changes

        if (capturyInput != null)
        {
            InputSystem.RemoveDevice(capturyInput);
        }

        CapturyNetworkPlugin networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            networkPlugin.SkeletonFound -= OnSkeletonFound;
        }
    }

    private void OnSkeletonFound(CapturySkeleton skeleton)
    {
        // link event to function
        skeleton.OnSkeletonSetupComplete += OnSkeletonSetupComplete;
    }

    private void OnSkeletonSetupComplete(CapturySkeleton skeleton)
    {
        // find pelvis bone
        pelvis = FindJointByExactName(skeleton, pelvisName);

        if (pelvis == null)
        {
            Debug.LogError("SquatTracking: Could not find pelvis bone! Check the name in the Inspector.");
        }
        else
        {
            Debug.Log("SquatTracking: Found pelvis joint.");
            StartCoroutine(CalibrateStandingPosition());
        }
    }

    IEnumerator CalibrateStandingPosition()
    {
        // callibrate to find standing position
        Debug.Log("Starting squat calibration. Please stand upright...");
        yield return new WaitForSeconds(calibrationDelay);

        if (pelvis != null)
        {
            standingPelvisHeight = pelvis.localPosition.y;
            isCalibrated = true;
            Debug.Log($"Squat calibration complete. Standing height: {standingPelvisHeight:F3}m");
        }
    }

    private Transform FindJointByExactName(CapturySkeleton skeleton, string jointName)
    {
        // finding the joint in skeleton by its exact name
        foreach (var joint in skeleton.joints)
        {
            if (joint.name == jointName)
            {
                Debug.Log("Match found: " + joint.name);
                return joint.transform;
            }
        }
        Debug.LogError($"Joint {jointName} not found in CapturySkeleton!");
        return null;
    }

    private void Update()
    {
        if (!isCalibrated || pelvis == null || capturyInput == null)
            return;

        // calculate how much the pelvis drops
        float currentPelvisHeight = pelvis.localPosition.y;
        float squatDepth = standingPelvisHeight - currentPelvisHeight;
        float squatThreshold = standingPelvisHeight * squatThresholdPercentage;

        wasInSquat = isInSquat;

        isInSquat = squatDepth > squatThreshold;

        CapturyInputState state = new CapturyInputState();

        // squat started
        if (isInSquat && !wasInSquat)
        {
            state.squatStarted = 1.0f;
            Debug.Log("Squat started!");
        }

        // squat complete
        if (!isInSquat && wasInSquat && squatDepth <= standingThreshold)
        {
            state.squatCompleted = 1.0f;
            Debug.Log("Squat completed!");
        }

        // send state data to input system
        InputSystem.QueueStateEvent(capturyInput, state);
    }

    public void Recalibrate()
    {
        isCalibrated = false;
        StartCoroutine(CalibrateStandingPosition());
    }
}