using UnityEngine;
using UnityEngine.InputSystem;

public class StillnessTracker : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("History")]
    [Tooltip("How many samples to keep in the rolling window.")]
    [SerializeField] private int historySize = 60;

    [Header("Thresholds")]
    [Tooltip("If the range of recent pelvis position on any axis is below this, considered still.")]
    [SerializeField] private float positionRangeThreshold = 0.2f;

    [Tooltip("Minimum stillness value to be considered 'still'.")]
    [SerializeField] private float stillnesssThreshold = 0.7f;

    // Raw pelvis position action
    private InputAction pelvisPositionAction;

    // Rolling history for each axis of raw position
    private float[] pelvisXHistory;
    private float[] pelvisYHistory;
    private float[] pelvisZHistory;
    private int historyIndex = 0;
    private int sampleCount = 0;

    // state
    private float currentStillness = 0f;
    private float stillDuration = 0f;
    private bool isTracking = false;

    public float Stillness => currentStillness;
    public float StillDuration => stillDuration;
    public bool IsStill => currentStillness > stillnesssThreshold;

    // Expose current raw pelvis X for UI compatibility
    public float WeightShift => pelvisPositionAction != null ? pelvisPositionAction.ReadValue<Vector3>().x : 0f;

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("StillnessTracker: No InputActionAsset assigned!");
            return;
        }

        var torsoMap = inputActions.FindActionMap("Torso");
        if (torsoMap != null)
        {
            pelvisPositionAction = torsoMap.FindAction("PelvisPosition");
        }

        pelvisXHistory = new float[historySize];
        pelvisYHistory = new float[historySize];
        pelvisZHistory = new float[historySize];
    }

    private void OnEnable()
    {
        pelvisPositionAction?.Enable();
    }

    private void OnDisable()
    {
        pelvisPositionAction?.Disable();
    }

    private void Update()
    {
        if (!isTracking) return;

        // Sample raw pelvis position — not calibration-relative
        Vector3 pelvisPos = pelvisPositionAction != null
            ? pelvisPositionAction.ReadValue<Vector3>()
            : Vector3.zero;

        pelvisXHistory[historyIndex] = pelvisPos.x;
        pelvisYHistory[historyIndex] = pelvisPos.y;
        pelvisZHistory[historyIndex] = pelvisPos.z;
        historyIndex = (historyIndex + 1) % historySize;
        sampleCount = Mathf.Min(sampleCount + 1, historySize);

        if (sampleCount < 5)
        {
            currentStillness = 0f;
            return;
        }

        // Check how much raw position varied over the rolling window
        float rangeX = GetRange(pelvisXHistory, sampleCount);
        float rangeY = GetRange(pelvisYHistory, sampleCount);
        float rangeZ = GetRange(pelvisZHistory, sampleCount);

        // Use the worst axis
        float worstRange = Mathf.Max(rangeX, Mathf.Max(rangeY, rangeZ));

        // Low range = high stillness
        currentStillness = 1f - Mathf.Clamp01(worstRange / positionRangeThreshold);

        if (IsStill)
        {
            stillDuration += Time.deltaTime;
        }
        else
        {
            stillDuration = 0f;
        }
    }

    public void StartTracking()
    {
        currentStillness = 0f;
        stillDuration = 0f;
        historyIndex = 0;
        sampleCount = 0;
        isTracking = true;
    }

    public void StopTracking()
    {
        isTracking = false;
    }

    public bool HasBeenStillFor(float seconds)
    {
        return stillDuration >= seconds;
    }

    private float GetRange(float[] buffer, int count)
    {
        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < count; i++)
        {
            if (buffer[i] < min) min = buffer[i];
            if (buffer[i] > max) max = buffer[i];
        }

        return max - min;
    }
}