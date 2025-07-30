using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Captury;
using Newtonsoft.Json;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem.XR.Haptics;

[Serializable]
public class MotionFrame
{
    public float timeStamp;
    public Dictionary<string, Vector3> jointPositions;
    public Dictionary<string, Quaternion> jointRotations;
    public CapturyInputState inputState;

    public MotionFrame()
    {
        jointPositions = new Dictionary<string, Vector3>();
        jointRotations = new Dictionary<string, Quaternion>();
    }
}

[System.Serializable]
public class MotionRecording
{
    public string sessionId;
    public DateTime startTime;
    public DateTime endTime;
    public float frameRate;
    public List<MotionFrame> frames;
    public Dictionary<string, string> metadata;

    public MotionRecording()
    {
        frames = new List<MotionFrame>();
        metadata = new Dictionary<string, string>();
        sessionId = System.Guid.NewGuid().ToString();
        startTime = DateTime.Now;
    }
}

public class CapturyMotionRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    [SerializeField] private bool autoStartRecording = false;
    [SerializeField] private float recordingFrameRate = 30f;
    [SerializeField] private string outputDirectory = "MotionRecordings";
    [SerializeField] private bool recordInputStates = true; // should we record what exercise?

    [Header("Joint Tracking")]
    [SerializeField]
    private string[] jointsToRecord =
    {
        "Hips", "LeftUpLeg", "LeftLeg", "LeftFoot", "LeftToeBase", "LeftToeBase_end", "RightUpLeg", "RightLeg", 
        "RightFoot", "RightToeBase", "RightToeBase_end", "Spine", "Spine1", "Spine2", "Spine3", "LeftShoulder", 
        "LeftArm", "LeftForeArm", "LeftHand", "RightShoulder", "RightArm", "RightForeArm", "RightHand", "Spine4",
        "Neck", "Head", "Head_end"
    };

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    // recording state
    private bool isRecording = false;
    private MotionRecording currentRecording;
    private float lastFrameTime;
    private float frameInterval;

    // captury references
    private CapturyNetworkPlugin networkPlugin;
    private CapturySkeleton trackedSkeleton;
    private Dictionary<string, Transform> jointTransforms;

    // input tracking
    private FootTracking footTracking;
    private WeightShiftTracking weightShiftTracking;
    private HipAbductionTracking hipAbductionTracking;
    // not using four square step or squat yet

    private void Start()
    {
        frameInterval = 1f / recordingFrameRate;
        jointTransforms = new Dictionary<string, Transform>();

        // find captury
        networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            networkPlugin.SkeletonFound += OnSkeletonFound;
        }
        else
        {
            Debug.LogError("CapturyMotionRecorder: Could not find Captury Network Plugin.");
        }

        // create output directory
        if(!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        if (autoStartRecording)
        {
            // wait a bit to allow things to set up, then start
            Invoke(nameof(autoStartRecording), 3f);
        }
    }

    private void OnDestroy()
    {
        if (networkPlugin != null)
        {
            networkPlugin.SkeletonFound -= OnSkeletonFound;
        }

        if(isRecording)
        {
            StopRecording();
        }
    }

    private void OnSkeletonFound(CapturySkeleton skeleton)
    {
        skeleton.OnSkeletonSetupComplete += OnSkeletonSetupComplete;
    }

    private void OnSkeletonSetupComplete(CapturySkeleton skeleton)
    {
        trackedSkeleton = skeleton;

        // map joints
        jointTransforms.Clear();

        foreach (var joint in skeleton.joints)
        {
            if (joint.transform != null)
            {
                jointTransforms[joint.name] = joint.transform;
                if (debugMode) Debug.Log($"CapturyMotionRecorder: Mapped joint {joint.name}");
            }
        }

        Debug.Log($"CapturyMotionRecorder: Skeleton setup complete. Tracking {jointTransforms.Count} joints.");
    }

    private void Update()
    {
        if (isRecording && trackedSkeleton != null)
        {
            // check if we need to record a new frame
            if (Time.time - lastFrameTime >= frameInterval)
            {
                RecordFrame();
                lastFrameTime = Time.time;
            }
        }
    }

    public void StartRecording()
    {
        if(isRecording)
        {
            Debug.LogWarning("CapturyMotionRecorder: Recording already in progress!");
            return;
        }

        if(trackedSkeleton == null)
        {
            Debug.LogWarning("CapturyMotionRecorder: No skeleton found! Failed to start recording.");
            return;
        }

        currentRecording = new MotionRecording();
        currentRecording.frameRate = recordingFrameRate;
        currentRecording.metadata["Unity Version"] = Application.unityVersion;
        currentRecording.metadata["Scene"] = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        currentRecording.metadata["Skeleton Name"] = trackedSkeleton.name;
        currentRecording.metadata["Joint Count"] = jointTransforms.Count.ToString();

        isRecording = true;
        lastFrameTime = Time.time;

        Debug.Log("CapturyMotionRecorder: Started recording.");
    }

    public void StopRecording()
    {
        if(!isRecording)
        {
            Debug.LogWarning("CapturyMotionRecorder: No recording to stop!");
            return;
        }

        isRecording = false;
        currentRecording.endTime = DateTime.Now;

        SaveRecording();

        Debug.Log($"CapturyMotionRecorder: Stopped recording. Recorded {currentRecording.frames.Count} frames");
    }


    private void RecordFrame()
    {
        var frame = new MotionFrame();
        frame.timeStamp = Time.time; 

        // record joint positions and rotations here
        foreach (var jointName in jointsToRecord)
        {
            if (jointTransforms.ContainsKey(jointName))
            {
                Transform joint = jointTransforms[jointName];
                frame.jointPositions[jointName] = joint.position;
                frame.jointRotations[jointName] = joint.rotation;
            }
        }

        // record input state if necessary 
        if(recordInputStates)
        {
            frame.inputState = GetCurrentInputState();
        }

        currentRecording.frames.Add(frame);

        if (debugMode && currentRecording.frames.Count % 30 == 0)
        {
            Debug.Log($"CapturyMotionRecorder: Recorded frame {currentRecording.frames.Count}, joints: {frame.jointPositions.Count}");
        }
    }

    private CapturyInputState GetCurrentInputState()
    {
       // will do this later!
        var inputState = new CapturyInputState();
        return inputState;
    }

    private void SaveRecording()
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filename = $"motion_recording_{timestamp}.json";
            string filepath = Path.Combine(outputDirectory, filename);

            string json = JsonConvert.SerializeObject(currentRecording, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            File.WriteAllText(filepath, json);

            Debug.Log($"CapturyMotionRecorder: Motion recording saved to: {filepath}");

            // and also do python stuff...
        }
        catch (Exception e)
        {
            Debug.LogError($"CapturyMotionRecorder: Failed to save recording: {e.Message}");
        }
    }
}
