using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Captury;
using UnityEngine.InputSystem.LowLevel;

public class FootTracking : MonoBehaviour
{
    [SerializeField] private Transform leftAnkle;
    [SerializeField] private Transform rightAnkle;
    [SerializeField] private float raiseThreshold = 0.1f;

    private CapturyInput capturyInput;
    private bool isFootRaised = false;

    [SerializeField] private string leftAnkleName = "Dan:LeftFoot";
    [SerializeField] private string rightAnkleName = "Dan:RightFoot";

    private void Start()
    {
        CapturyNetworkPlugin networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            Debug.Log("FootTracking: Subscribing to CapturyNetworkPlugin.SkeletonFound...");
            networkPlugin.SkeletonFound -= OnSkeletonFound;
            networkPlugin.SkeletonFound += OnSkeletonFound;
        }
        else
        {
            Debug.LogError("FootTracking: Could not find CapturyNetworkPlugin!");
        }

        if (capturyInput == null)
        {
            capturyInput = InputSystem.AddDevice<CapturyInput>();
            Debug.Log("CapturyInput device created!");
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
        Debug.Log("FootTracking received Skeleton: " + skeleton.name);

        // Wait until SetTargetSkeleton() finishes
        skeleton.OnSkeletonSetupComplete += OnSkeletonSetupComplete;
    }

    private void OnSkeletonSetupComplete(CapturySkeleton skeleton)
    {
        Debug.Log("Skeleton setup complete. Now tracking ankles!");

        leftAnkle = FindJointByExactName(skeleton, leftAnkleName);
        rightAnkle = FindJointByExactName(skeleton, rightAnkleName);

        if (leftAnkle == null || rightAnkle == null)
        {
            Debug.LogError("FootTracking: Could not find the ankle bones! Check the names in the Inspector.");
        }
        else
        {
            Debug.Log("FootTracking: Found ankle bones! Left: " + leftAnkle.name + " | Right: " + rightAnkle.name);
        }
    }


    private Transform FindJointByExactName(CapturySkeleton skeleton, string jointName)
    {
        foreach (var joint in skeleton.joints)
        {
            //Debug.Log("Joint Name: " + joint.name + " looking for " + jointName);
            if (joint.name == jointName)
            {
                Debug.Log("Match found!");
                Debug.Log(joint.transform);
                return joint.transform;
            }
        }
        Debug.LogError($"Joint {jointName} not found in CapturySkeleton!");
        return null;
    }

    private void Update()
    {
        if (leftAnkle == null || rightAnkle == null || capturyInput == null) return;

        if (capturyInput.footHeight == null || capturyInput.footRaise == null || capturyInput.footLower == null)
        {
            Debug.LogError("CapturyInput controls are not assigned. Ensure `FinishSetup()` runs correctly.");
            return;
        }

        float footHeight = Mathf.Abs(leftAnkle.position.y - rightAnkle.position.y);
        InputSystem.QueueDeltaStateEvent(capturyInput.footHeight, footHeight);

        Debug.Log($"Foot Height: {footHeight}");

        using (var eventPtr = StateEvent.From(capturyInput, out InputEventPtr eventRef))
        {
            if (footHeight > raiseThreshold && !isFootRaised)
            {
                isFootRaised = true;
                capturyInput.footRaise.WriteValueIntoEvent(1.0f, eventRef);
                capturyInput.footLower.WriteValueIntoEvent(0.0f, eventRef);
                Debug.Log("Foot Raised");
            }

            if (footHeight < raiseThreshold && isFootRaised)
            {
                isFootRaised = false;
                capturyInput.footLower.WriteValueIntoEvent(1.0f, eventRef);
                capturyInput.footRaise.WriteValueIntoEvent(0.0f, eventRef);
                Debug.Log("Foot Lowered");
            }

            InputSystem.QueueEvent(eventRef); // Manually queue the event
        }
    }
}