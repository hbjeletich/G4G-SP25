using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Captury;
using UnityEngine.InputSystem.LowLevel;

public class HipAbductionTracking : MonoBehaviour
{
    [Header("Skeleton Tracking")]
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    [Header("Joint Names")]
    [SerializeField] private string leftFootName = "LeftFoot";
    [SerializeField] private string rightFootName = "RightFoot";

    [Header("Hip Abduction Settings")]
    [SerializeField] private float minAbductionDistance = 0.2f; // min distance
    [SerializeField] private float minLiftHeight = 0.05f; // min lift height
    [SerializeField] private float calibrationDelay = 2.0f; // make this zero for captury replay input, 2 or more for live
    [SerializeField] private bool debugMode = true;

    private bool isLeftAbducted = false;
    private bool isRightAbducted = false;
    private float defaultFootDistance; // baseline distance between feet when standing normally
    private float groundHeight;
    private bool isCalibrated = false;

    private CapturyInput capturyInput;

    private void Start()
    {
        capturyInput = InputSystem.GetDevice<CapturyInput>();

        if (capturyInput == null)
        {
            CapturyInput.Register();

            capturyInput = InputSystem.AddDevice<CapturyInput>();
            Debug.Log($"HipAbductionTracking: Created new CapturyInput device with ID: {capturyInput.deviceId}");
        }
        else
        {
            Debug.Log($"HipAbductionTracking: Using existing CapturyInput device with ID: {capturyInput.deviceId}");
        }

        CapturyNetworkPlugin networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            Debug.Log("HipAbductionTracking: Subscribing to CapturyNetworkPlugin.SkeletonFound...");
            networkPlugin.SkeletonFound -= OnSkeletonFound;
            networkPlugin.SkeletonFound += OnSkeletonFound;
        }
        else
        {
            Debug.LogError("HipAbductionTracking: Could not find CapturyNetworkPlugin!");
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
        // wait until SetTargetSkeleton() finishes
        skeleton.OnSkeletonSetupComplete += OnSkeletonSetupComplete;
    }

    private void OnSkeletonSetupComplete(CapturySkeleton skeleton)
    {
        leftFoot = FindJointByExactName(skeleton, leftFootName);
        rightFoot = FindJointByExactName(skeleton, rightFootName);

        if (leftFoot == null || rightFoot == null)
        {
            Debug.LogError("HipAbductionTracking: Could not find the foot bones! Check the names in the Inspector.");
        }
        else
        {// Start calibration coroutine
            StartCoroutine(CalibrateStartPosition());
        }
    }

    IEnumerator CalibrateStartPosition()
    {
        yield return new WaitForSeconds(calibrationDelay); // Give time for user to get in position

        if (leftFoot != null && rightFoot != null)
        {
            // calc default distance
            Vector3 leftPos = leftFoot.position;
            Vector3 rightPos = rightFoot.position;

            // ignore height difference, just get horizontal
            Vector2 leftPos2D = new Vector2(leftPos.x, leftPos.z);
            Vector2 rightPos2D = new Vector2(rightPos.x, rightPos.z);

            defaultFootDistance = Vector2.Distance(leftPos2D, rightPos2D);

            // use average of foot heights as ground height
            groundHeight = (leftPos.y + rightPos.y) / 2.0f;

            isCalibrated = true;
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
        if (!isCalibrated || leftFoot == null || rightFoot == null || capturyInput == null)
            return;

        // get current foot positions
        Vector3 leftPos = leftFoot.position;
        Vector3 rightPos = rightFoot.position;

        // distance between feet (horizontal plane only)
        Vector2 leftPos2D = new Vector2(leftPos.x, leftPos.z);
        Vector2 rightPos2D = new Vector2(rightPos.x, rightPos.z);
        float currentDistance = Vector2.Distance(leftPos2D, rightPos2D);

        // relative distance compared to default stance
        float distanceRatio = currentDistance / defaultFootDistance;

        // foot heights relative to ground
        float leftHeight = leftPos.y - groundHeight;
        float rightHeight = rightPos.y - groundHeight;

        // check left hip abduction
        bool leftLiftedNow = leftHeight > minLiftHeight;
        bool leftAbductedNow = distanceRatio > (1.0f + minAbductionDistance / defaultFootDistance) && leftLiftedNow;

        // check right hip abduction
        bool rightLiftedNow = rightHeight > minLiftHeight;
        bool rightAbductedNow = distanceRatio > (1.0f + minAbductionDistance / defaultFootDistance) && rightLiftedNow;

        CapturyInputState state = new CapturyInputState();

        if (leftAbductedNow != isLeftAbducted)
        {
            isLeftAbducted = leftAbductedNow;

            if (isLeftAbducted)
            {
                state.leftHipAbducted = 1.0f;
                Debug.Log($"Left Hip Abduction detected! Distance: {currentDistance}m, Height: {leftHeight}m");
            }
            else
            {
                state.leftHipAbducted = 0.0f;
                Debug.Log("Left Hip returned to neutral");
            }
        }

        if (rightAbductedNow != isRightAbducted)
        {
            isRightAbducted = rightAbductedNow;

            if (isRightAbducted)
            {
                state.rightHipAbducted = 1.0f;
                Debug.Log($"Right Hip Abduction detected! Distance: {currentDistance}m, Height: {rightHeight}m");
            }
            else
            {
                state.rightHipAbducted = 0.0f;
                Debug.Log("Right Hip returned to neutral");
            }
        }

        state.abductionDistance = currentDistance - defaultFootDistance;
        state.leftFootHeight = leftHeight;
        state.rightFootHeight = rightHeight;

        InputSystem.QueueStateEvent(capturyInput, state);
    }

    // recalibrate if needed
    public void Recalibrate()
    {
        isCalibrated = false;
        StartCoroutine(CalibrateStartPosition());
    }
}