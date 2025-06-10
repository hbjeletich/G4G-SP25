using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Captury;
using UnityEngine.InputSystem.LowLevel;

public class FourSquareStepTracking : MonoBehaviour
{
    [Header("Skeleton Tracking")]
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private Transform pelvis;

    [Header("Joint Names")]
    [SerializeField] private string leftFootName = "LeftFoot";
    [SerializeField] private string rightFootName = "RightFoot";
    [SerializeField] private string pelvisName = "Hips";

    [Header("Four Square Settings")]
    [SerializeField] private float squareSize = 0.5f; // size of each quadrant in meters
    [SerializeField] private float stepHeightThreshold = 0.1f; // min height for a step
    [SerializeField] private float calibrationDelay = 2.0f; // make this zero for captury replay input, 2 or more for live

    // offset from center to put player in quadrant 1
    [SerializeField] private Vector2 quadrantOneOffset = new Vector2(0.2f, -0.2f);

    // quadrant states
    private Vector2 centerPoint;
    private bool isCalibrated = false;

    // current quadrant
    private int currentQuadrant = 0; // 0 = not assigned yet, 1-4 = quadrants
    private int lastQuadrant = 0;

    // stepping states
    private bool isLeftFootRaised = false;
    private bool isRightFootRaised = false;

    // input
    private CapturyInput capturyInput;

    // quadrant enter events
    private Dictionary<int, bool> quadrantEntered = new Dictionary<int, bool>() {
        {1, true}, // start with quad 1 entered
        {2, false}, 
        {3, false}, 
        {4, false}
    };

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
            Debug.LogError("FourSquareStepTracking: Could not find CapturyNetworkPlugin!");
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
        // wait for skeleton to be found
        skeleton.OnSkeletonSetupComplete += OnSkeletonSetupComplete;
    }

    private void OnSkeletonSetupComplete(CapturySkeleton skeleton)
    {
        leftFoot = FindJointByExactName(skeleton, leftFootName);
        rightFoot = FindJointByExactName(skeleton, rightFootName);
        pelvis = FindJointByExactName(skeleton, pelvisName);

        if (leftFoot == null || rightFoot == null || pelvis == null)
        {
            Debug.LogError("FourSquareStepTracking: Could not find bones! Check the names.");
        }
        else
        {
            Debug.Log("FourSquareStepTracking: Found joints.");
            // start calibration
            StartCoroutine(CalibrateStartPosition());
        }
    }

    IEnumerator CalibrateStartPosition()
    {
        // calibration from quadrant one
        Debug.Log("Starting four square calibration. Please stand in quadrant 1 position...");
        yield return new WaitForSeconds(calibrationDelay); // give time for user to get in position

        if (pelvis != null)
        {
            Vector3 pelvisPos = pelvis.position;

            // calculate point by applying the offset from quadrant 1 (where the user is standing)
            centerPoint = new Vector2(pelvisPos.x - quadrantOneOffset.x, pelvisPos.z - quadrantOneOffset.y);

            currentQuadrant = 1;
            lastQuadrant = 1;

            isCalibrated = true;

            CapturyInputState state = new CapturyInputState();
            state.quadrantOne = 1.0f;

            // give info to input system
            InputSystem.QueueStateEvent(capturyInput, state);
        }
    }

    private void ResetQuadrantStates()
    {
        foreach (int quadrant in quadrantEntered.Keys)
        {
            quadrantEntered[quadrant] = false;
        }

        quadrantEntered[1] = true;
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
        if (!isCalibrated || leftFoot == null || rightFoot == null || pelvis == null || capturyInput == null)
            return;

        // for step detection
        float leftFootHeight = leftFoot.position.y;
        float rightFootHeight = rightFoot.position.y;

        // foot is raised based on height threshold
        bool leftRaised = leftFootHeight > stepHeightThreshold;
        bool rightRaised = rightFootHeight > stepHeightThreshold;

        bool leftFootStateChanged = leftRaised != isLeftFootRaised;
        bool rightFootStateChanged = rightRaised != isRightFootRaised;

        isLeftFootRaised = leftRaised;
        isRightFootRaised = rightRaised;

        // get positions for quadrant detection using XZ plane (floor plane)
        Vector2 bodyCenter = new Vector2(pelvis.position.x, pelvis.position.z);
        Vector2 leftFootPos = new Vector2(leftFoot.position.x, leftFoot.position.z);
        Vector2 rightFootPos = new Vector2(rightFoot.position.x, rightFoot.position.z);

        // get quadrants
        int leftFootQuadrant = GetQuadrant(leftFootPos);
        int rightFootQuadrant = GetQuadrant(rightFootPos);
        int bodyQuadrant = GetQuadrant(bodyCenter);

        // see if quadrants have changed
        if (bodyQuadrant != 0 && bodyQuadrant != currentQuadrant)
        {
            lastQuadrant = currentQuadrant;
            currentQuadrant = bodyQuadrant;

            // mark as entered
            if (!quadrantEntered[currentQuadrant])
            {
                quadrantEntered[currentQuadrant] = true;

                Debug.Log($"Entered quadrant {currentQuadrant}");

                CapturyInputState state = new CapturyInputState();

                // set correct button
                switch (currentQuadrant)
                {
                    case 1:
                        state.quadrantOne = 1.0f;
                        break;
                    case 2:
                        state.quadrantTwo = 1.0f;
                        break;
                    case 3:
                        state.quadrantThree = 1.0f;
                        break;
                    case 4:
                        state.quadrantFour = 1.0f;
                        break;
                }

                InputSystem.QueueStateEvent(capturyInput, state);

                // check if all quadrants have been entered
                CheckAllQuadrantsEntered();
            }
        }
    }

    // detemines which quadrant a position is in, relative to the center point
    private int GetQuadrant(Vector2 position)
    {
        // relative from center
        Vector2 relativePos = position - centerPoint;

        // no quadrant if within center area
        if (Mathf.Abs(relativePos.x) < 0.1f || Mathf.Abs(relativePos.y) < 0.1f)
            return 0;

        // Quadrant 1: bottom-right (positive X, negative Z)
        if (relativePos.x > 0 && relativePos.y < 0)
            return 1;

        // Quadrant 2: top-right (positive X, positive Z) - now in front of quadrant 1
        if (relativePos.x > 0 && relativePos.y > 0)
            return 2;

        // Quadrant 3: top-left (negative X, positive Z)
        if (relativePos.x < 0 && relativePos.y > 0)
            return 3;

        // Quadrant 4: bottom-left (negative X, negative Z)
        if (relativePos.x < 0 && relativePos.y < 0)
            return 4;

        // should not happen hopefully!
        return 0;
    }

    private void CheckAllQuadrantsEntered()
    {
        bool allEntered = true; // set true
        foreach (bool entered in quadrantEntered.Values)
        {
            if (!entered)
            {
                // if any unentered, set false
                allEntered = false;
                break;
            }
        }

        if (allEntered)
        {
            Debug.Log("All four quadrants have been entered!");

            // state for all quadrants complete
            CapturyInputState state = new CapturyInputState();
            state.allQuadrantsComplete = 1.0f;

            InputSystem.QueueStateEvent(capturyInput, state);

            // reset for the next round
            ResetQuadrantStates();
        }
    }

    // recalibrate if needed
    public void Recalibrate()
    {
        isCalibrated = false;
        StartCoroutine(CalibrateStartPosition());
    }
}