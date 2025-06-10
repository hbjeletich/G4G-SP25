using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Captury;
using UnityEngine.InputSystem.LowLevel;

public class FootTracking : MonoBehaviour
{
    // transforms for ankles
    private Transform leftAnkle;
    private Transform rightAnkle;

    [SerializeField] private float raiseThreshold = 0.1f; // what counts as foot being raised

    private CapturyInput capturyInput;
    private bool isFootRaised = false; // tracking state

    // names of the game object that tracks the ankle
    [SerializeField] private string leftAnkleName = "LeftFoot";
    [SerializeField] private string rightAnkleName = "RightFoot";

    private void Awake()
    {
        //CapturyInput.Register();
    }

    private void Start()
    {
        // add captury input device
        capturyInput = InputSystem.AddDevice<CapturyInput>();

        if (capturyInput == null)
        { 
            Debug.LogError("Failed to create CapturyInput device!");
            return;
        }

        // find captury network plugin (part of captury's scripts)
        // this is how we connect to skeleton
        CapturyNetworkPlugin networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            // OnSkeletonFound is event we added to captury's script so we can
            // get the skeleton at the correct time
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
        // looking for ankle parts

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
        if (leftAnkle == null || rightAnkle == null || capturyInput == null) return;

        // height difference between ankles
        float footHeight = Mathf.Abs(leftAnkle.position.y - rightAnkle.position.y);

        // create input state
        var state = new CapturyInputState
        {
            footHeight = footHeight,
            footRaise = isFootRaised ? 0.0f : (footHeight > raiseThreshold ? 1.0f : 0.0f),
            footLower = isFootRaised ? (footHeight < raiseThreshold ? 1.0f : 0.0f) : 0.0f
        };

        // update foot raised state for the next frame
        bool wasFootRaised = isFootRaised;
        isFootRaised = footHeight > raiseThreshold;

        // send state data to input system
        InputSystem.QueueStateEvent(capturyInput, state);
    }
}