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

    [SerializeField] private string leftAnkleName = "LeftFoot";
    [SerializeField] private string rightAnkleName = "RightFoot";

    private void Awake()
    {
        //CapturyInput.Register();
    }

    private void Start()
    {
        capturyInput = InputSystem.AddDevice<CapturyInput>();

        if (capturyInput == null)
        {
            Debug.LogError("Failed to create CapturyInput device!");
            return;
        }

        //Debug.Log($"CapturyInput device created with ID: {capturyInput.deviceId}");

        CapturyNetworkPlugin networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            //Debug.Log("FootTracking: Subscribing to CapturyNetworkPlugin.SkeletonFound...");
            networkPlugin.SkeletonFound -= OnSkeletonFound;
            networkPlugin.SkeletonFound += OnSkeletonFound;
        }
        else
        {
            Debug.LogError("FootTracking: Could not find CapturyNetworkPlugin!");
        }
    }

    private void OnDestroy()
    {
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
        //Debug.Log("FootTracking received Skeleton: " + skeleton.name);

        skeleton.OnSkeletonSetupComplete += OnSkeletonSetupComplete;
    }

    private void OnSkeletonSetupComplete(CapturySkeleton skeleton)
    {
        //Debug.Log("Skeleton setup complete. Now tracking ankles!");

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
        if (leftAnkle == null || rightAnkle == null || capturyInput == null) return;

        // height difference between ankles
        float footHeight = Mathf.Abs(leftAnkle.position.y - rightAnkle.position.y);

        var state = new CapturyInputState
        {
            footHeight = footHeight,
            footRaise = isFootRaised ? 0.0f : (footHeight > raiseThreshold ? 1.0f : 0.0f),
            footLower = isFootRaised ? (footHeight < raiseThreshold ? 1.0f : 0.0f) : 0.0f
        };

        // update foot raised state for the next frame
        bool wasFootRaised = isFootRaised;
        isFootRaised = footHeight > raiseThreshold;

        InputSystem.QueueStateEvent(capturyInput, state);
    }
}