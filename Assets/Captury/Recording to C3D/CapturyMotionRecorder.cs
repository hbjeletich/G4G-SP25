using Captury;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class JointData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class MotionFrame
{
    public float timestamp;
    public JointData[] joints;
    public CapturyInputState inputState;

    public MotionFrame()
    {
        joints = new JointData[0];
    }
}

[System.Serializable]
public class MotionRecording
{
    public string sessionId;
    public string startTime;
    public string endTime;   
    public float frameRate;
    public MotionFrame[] frames; 
    public string[] metadataKeys;  
    public string[] metadataValues;

    public MotionRecording()
    {
        frames = new MotionFrame[0];
        sessionId = System.Guid.NewGuid().ToString();
        startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

public class CapturyMotionRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    [SerializeField] private bool autoStartRecording = false;
    [SerializeField] private float recordingFrameRate = 30f;
    [SerializeField] private string outputDirectory = "MotionRecordings";
    [SerializeField] private bool recordInputStates = true;

    [Header("Joint Tracking")]
    [SerializeField]
    private string[] jointsToRecord = {
        "Hips", "Spine", "Spine1", "Spine2", "Spine3", "Spine4", "Neck", "Head",
        "LeftShoulder", "LeftUpperArm", "LeftLowerArm", "LeftHand",
        "RightShoulder", "RightUpperArm", "RightLowerArm", "RightHand",
        "LeftUpperLeg", "LeftLowerLeg", "LeftFoot", "LeftToes",
        "RightUpperLeg", "RightLowerLeg", "RightFoot", "RightToes"
    };

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    // recording state
    private bool isRecording = false;
    private List<MotionFrame> recordedFrames;
    private Dictionary<string, string> recordingMetadata;
    private float lastFrameTime;
    private float frameInterval;

    // captury references
    private CapturyNetworkPlugin networkPlugin;
    private CapturySkeleton trackedSkeleton;
    private Dictionary<string, Transform> jointTransforms;

    private void Start()
    {
        frameInterval = 1f / recordingFrameRate;
        jointTransforms = new Dictionary<string, Transform>();

        networkPlugin = FindObjectOfType<CapturyNetworkPlugin>();
        if (networkPlugin != null)
        {
            networkPlugin.SkeletonFound += OnSkeletonFound;
        }
        else
        {
            Debug.LogError("CapturyMotionRecorder: Could not find CapturyNetworkPlugin!");
        }

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        if (autoStartRecording)
        {
            Invoke(nameof(StartRecording), 3f);
        }
    }

    private void OnDestroy()
    {
        if (networkPlugin != null)
        {
            networkPlugin.SkeletonFound -= OnSkeletonFound;
        }

        if (isRecording)
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
        Debug.Log("=== SKELETON SETUP STARTING ===");
        trackedSkeleton = skeleton;

        // map joints
        jointTransforms.Clear();

        Debug.Log($"Skeleton has {skeleton.joints.Length} joints");

        foreach (var joint in skeleton.joints)
        {
            Debug.Log($"Available joint: '{joint.name}', transform is null: {joint.transform == null}");
            if (joint.transform != null)
            {
                jointTransforms[joint.name] = joint.transform;
                if (debugMode) Debug.Log($"CapturyMotionRecorder: Mapped joint {joint.name}");
            }
        }

        Debug.Log($"=== FINAL JOINT MAPPING ===");
        foreach (var kvp in jointTransforms)
        {
            Debug.Log($"Mapped: '{kvp.Key}' -> {kvp.Value.name}");
        }

        Debug.Log($"CapturyMotionRecorder: Skeleton setup complete. Tracking {jointTransforms.Count} joints.");
    }

    private void Update()
    {
        if (isRecording && trackedSkeleton != null)
        {
            if (Time.time - lastFrameTime >= frameInterval)
            {
                RecordFrame();
                lastFrameTime = Time.time;
            }
        }
    }

    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("Recording is already in progress!");
            return;
        }

        if (trackedSkeleton == null)
        {
            Debug.LogWarning("No skeleton found! Cannot start recording.");
            return;
        }

        recordedFrames = new List<MotionFrame>();
        recordingMetadata = new Dictionary<string, string>();

        recordingMetadata["Unity Version"] = Application.unityVersion;
        recordingMetadata["Scene"] = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        recordingMetadata["Skeleton Name"] = trackedSkeleton.name;
        recordingMetadata["Joint Count"] = jointTransforms.Count.ToString();

        isRecording = true;
        lastFrameTime = Time.time;

        Debug.Log("Started motion recording!");
    }

    public void StopRecording()
    {
        if (!isRecording)
        {
            Debug.LogWarning("No recording in progress!");
            return;
        }

        isRecording = false;

        SaveRecording();

        Debug.Log($"Stopped motion recording. Recorded {recordedFrames.Count} frames.");
    }

    private void RecordFrame()
    {
        var frame = new MotionFrame();
        frame.timestamp = Time.time;

        Debug.Log($"=== RECORDING FRAME ===");
        Debug.Log($"trackedSkeleton is null: {trackedSkeleton == null}");
        Debug.Log($"jointTransforms count: {jointTransforms.Count}");
        Debug.Log($"jointsToRecord length: {jointsToRecord.Length}");

        var jointDataList = new List<JointData>();

        // record joint positions and rotations here
        int foundCount = 0;
        foreach (var jointName in jointsToRecord)
        {
            Debug.Log($"Looking for joint: '{jointName}'");
            if (jointTransforms.ContainsKey(jointName))
            {
                Transform joint = jointTransforms[jointName];
                Debug.Log($"Found joint '{jointName}' at position: {joint.position}");

                var jointData = new JointData();
                jointData.name = jointName;
                jointData.position = joint.position;
                jointData.rotation = joint.rotation;
                jointDataList.Add(jointData);
                foundCount++;
            }
            else
            {
                Debug.Log($"Joint '{jointName}' NOT FOUND in jointTransforms");
            }
        }

        frame.joints = jointDataList.ToArray();
        Debug.Log($"Created frame with {frame.joints.Length} joints out of {jointsToRecord.Length} requested");

        // record input state if necessary 
        if (recordInputStates)
        {
            frame.inputState = GetCurrentInputState();
        }

        recordedFrames.Add(frame);

        if (debugMode && recordedFrames.Count % 30 == 0)
        {
            Debug.Log($"CapturyMotionRecorder: Recorded frame {recordedFrames.Count}, joints: {frame.joints.Length}");
        }
    }

    private CapturyInputState GetCurrentInputState()
    {
        // TO BE REPLACED
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

            var recording = new MotionRecording();
            recording.frameRate = recordingFrameRate;
            recording.frames = recordedFrames.ToArray();
            recording.endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            recording.metadataKeys = new string[recordingMetadata.Count];
            recording.metadataValues = new string[recordingMetadata.Count];
            int i = 0;
            foreach (var kvp in recordingMetadata)
            {
                recording.metadataKeys[i] = kvp.Key;
                recording.metadataValues[i] = kvp.Value;
                i++;
            }

            string json = JsonUtility.ToJson(recording, true);

            File.WriteAllText(filepath, json);

            Debug.Log($"Motion recording saved to: {filepath}");

            CreatePythonConversionScript(filepath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save recording: {e.Message}");
        }
    }

    private void CreatePythonConversionScript(string jsonFilePath)
    {
        string pythonScript = GeneratePythonConversionScript(jsonFilePath);
        string scriptPath = Path.ChangeExtension(jsonFilePath, ".py");

        try
        {
            File.WriteAllText(scriptPath, pythonScript);
            Debug.Log($"Python conversion script created: {scriptPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create Python script: {e.Message}");
        }
    }

    private string GeneratePythonConversionScript(string jsonFilePath)
    {
        string jsonFileName = Path.GetFileName(jsonFilePath);
        string c3dFileName = Path.ChangeExtension(jsonFileName, ".c3d");

        return $@"#!/usr/bin/env python3
import json
import numpy as np
import sys
import os
import warnings

def convert_to_c3d(json_path):
    # convert motion capture JSON to C3D
    
    try:
        import c3d
        print('Creating production C3D file...')
    except ImportError:
        print('Error: c3d library not installed. Run: pip install c3d')
        return None
    
    # suppress the no analog data warning since we expect this
    warnings.filterwarnings('ignore', message='No analog data found in file.')
    
    # Load motion data
    with open(json_path, 'r') as f:
        data = json.load(f)
    
    frames = data['frames']
    joint_names = [joint['name'] for joint in frames[0]['joints']]
    frame_rate = data['frameRate']
    
    print(f'Processing: {{len(frames)}} frames, {{len(joint_names)}} joints, {{frame_rate}} Hz')
    
    # Prepare motion data in correct format
    print('Converting motion data to clinical format...')
    
    n_frames = len(frames)
    n_points = len(joint_names)
    all_frames_data = []
    
    for frame_idx, frame in enumerate(frames):
        if frame_idx % 100 == 0:
            print(f'Frame {{frame_idx}}/{{n_frames}}')
        
        frame_joints = {{joint['name']: joint for joint in frame['joints']}}
        point_data = np.zeros((n_points, 4), dtype=np.float32)
        
        for point_idx, joint_name in enumerate(joint_names):
            if joint_name in frame_joints:
                pos = frame_joints[joint_name]['position']
                # convert to clinical coordinates (mm)
                point_data[point_idx, 0] = pos['x'] * 1000    # X (lateral)
                point_data[point_idx, 1] = pos['z'] * 1000    # Y (anterior) - swapped
                point_data[point_idx, 2] = -pos['y'] * 1000   # Z (superior) - flipped
                point_data[point_idx, 3] = 0.0                # Residual: 0 = good data
            else:
                # Missing joint data
                point_data[point_idx, 0:3] = 0.0
                point_data[point_idx, 3] = -1.0               # Residual: -1 = missing
        
        # Analog data: empty but properly shaped to avoid library bugs
        # Shape: (n_analog_channels, n_analog_samples_per_frame)
        # For no analog data: (0, 1) - this prevents index errors
        analog_data = np.array([], dtype=np.float32).reshape(0, 1)
        
        # Add frame as required tuple format
        all_frames_data.append((point_data, analog_data))
    
    # Create output file
    output_file = json_path.replace('.json', '_PRODUCTION.c3d')
    
    print(f'Creating C3D file: {{output_file}}')
    
    try:
        with open(output_file, 'wb') as handle:
            # Create writer with minimal, stable parameters
            writer = c3d.Writer(
                point_rate=float(frame_rate),  # Ensure float
                analog_rate=0                  # No analog data
            )
            
            # Set point labels safely
            try:
                writer.set_point_labels(joint_names)
                print('Joint labels set successfully')
            except Exception as e:
                print(f'Could not set labels: {{e}}')
                print('File will work but without joint names')
            
            # Add all motion frames
            print('Adding motion frames...')
            try:
                writer.add_frames(all_frames_data)
                print('All frames added successfully')
            except Exception as e:
                print(f'Error adding frames: {{e}}')
                return None
            
            # Write file with error handling
            print('Writing C3D file...')
            try:
                writer.write(handle)
                print(' C3D file written successfully')
            except Exception as e:
                print(f'Writer cleanup warning: {{e}}')
                print('Checking if file was created anyway...')
        
        # Verify the file was created and is valid
        print('Verifying C3D file...')
        
        if not os.path.exists(output_file):
            print('File was not created')
            return None
        
        file_size = os.path.getsize(output_file)
        if file_size == 0:
            print('File is empty')
            return None
        
        print(f'File created: {{file_size}} bytes')
        
        # Test if file can be read
        try:
            with open(output_file, 'rb') as handle:
                reader = c3d.Reader(handle)
                
                # Get file info
                points_count = getattr(reader, 'point_used', 0)
                frames_count = getattr(reader, 'last_frame', 0) - getattr(reader, 'first_frame', 0) + 1
                rate = getattr(reader, 'point_rate', 0)
                
                print(f'File verification successful:')
                print(f'  Points: {{points_count}}')
                print(f'  Frames: {{frames_count}}')
                print(f'  Frame rate: {{rate}} Hz')
                print(f'  Duration: {{frames_count/rate:.1f}} seconds')
                
                # Check if labels are present
                if hasattr(reader, 'point_labels') and reader.point_labels:
                    print(f'  Labels: {{len(reader.point_labels)}} joints')
                    print(f'  First few: {{reader.point_labels[:3]}}')
                else:
                    print('  Labels: Not included')
                
        except Exception as e:
            print(f'File created but verification failed: {{e}}')
        
        return output_file
        
    except Exception as e:
        print(f'Unexpected error during conversion: {{e}}')
        import traceback
        traceback.print_exc()
        return None
    
    finally:
        # Reset warning filters
        warnings.resetwarnings()

def main():
    if len(sys.argv) != 2:
        print('Usage: python production_c3d_converter.py your_file.json')
        print('Example: python production_c3d_converter.py motion_recording_2025-07-31_15-21-39.json')
        return
    
    json_file = sys.argv[1]
    
    if not os.path.exists(json_file):
        print(f'Error: File not found: {{json_file}}')
        return
    
    print('=== C3D Converter ===')
    print(f'Input: {{json_file}}')
    
    result = convert_to_c3d(json_file)
    
    if result:
        print(f'Success! Output: {{result}}')
        
        # Provide joint reference
        print(f'Joint reference (in order):')
        joint_names = [
            'Hips', 'LeftUpLeg', 'LeftLeg', 'LeftFoot', 'LeftToeBase',
            'RightUpLeg', 'RightLeg', 'RightFoot', 'RightToeBase', 
            'Spine', 'Spine1', 'Spine2', 'Spine3',
            'LeftShoulder', 'LeftArm', 'LeftForeArm', 'LeftHand',
            'RightShoulder', 'RightArm', 'RightForeArm', 'RightHand',
            'Spine4', 'Neck', 'Head'
        ]
        for i, name in enumerate(joint_names[:8], 1):
            print(f'  {{i:2d}}. {{name}}')
        print(f'  ... and {{len(joint_names)-8}} more joints')
        
    else:
        print(f'Conversion failed!')

if __name__ == '__main__':
    main()
";
    }

    public bool IsRecording => isRecording;
    public int FrameCount => recordedFrames?.Count ?? 0;
    public float RecordingDuration => isRecording ? Time.time - lastFrameTime : 0f;
}