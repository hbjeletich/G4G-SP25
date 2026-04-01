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
    [Tooltip("If the range of recent weightShiftX values is below this, weight shift is 'still'.")]
    [SerializeField] private float weightShiftRangeThreshold = 0.2f;

    [Tooltip("If the range of recent pelvisY values is below this, pelvis is 'still'.")]
    [SerializeField] private float pelvisRangeThreshold = 0.15f;

    [Tooltip("Minimum stillness value to be considered 'still'.")]
    [SerializeField] private float stillnesssThreshold = 0.7f;

    // Torso actions
    private InputAction weightShiftXAction;
    private InputAction pelvisPositionAction;

    // rolling history
    private float[] weightShiftHistory;
    private float[] pelvisYHistory;
    private int historyIndex = 0;
    private int sampleCount = 0;

    // state
    private float currentStillness = 0f;
    private float stillDuration = 0f;
    private bool isTracking = false;

    public float Stillness => currentStillness;
    public float StillDuration => stillDuration;
    public bool IsStill => currentStillness > stillnesssThreshold;

    public float WeightShift => weightShiftXAction != null ? weightShiftXAction.ReadValue<float>() : 0f;

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
            weightShiftXAction = torsoMap.FindAction("WeightShiftX");
            pelvisPositionAction = torsoMap.FindAction("PelvisPosition");
        }

        weightShiftHistory = new float[historySize];
        pelvisYHistory = new float[historySize];
    }

    private void OnEnable()
    {
        weightShiftXAction?.Enable();
        pelvisPositionAction?.Enable();
    }

    private void OnDisable()
    {
        weightShiftXAction?.Disable();
        pelvisPositionAction?.Disable();
    }

    private void Update()
    {
        if (!isTracking) return;

        // Sample current values
        float weightShiftX = weightShiftXAction != null ? weightShiftXAction.ReadValue<float>() : 0f;
        float pelvisY = pelvisPositionAction != null ? pelvisPositionAction.ReadValue<Vector3>().y : 0f;

        // Store in rolling buffer
        weightShiftHistory[historyIndex] = weightShiftX;
        pelvisYHistory[historyIndex] = pelvisY;
        historyIndex = (historyIndex + 1) % historySize;
        sampleCount = Mathf.Min(sampleCount + 1, historySize);

        // Need at least a few samples before we can judge
        if (sampleCount < 5)
        {
            currentStillness = 0f;
            return;
        }

        // Check how much values have varied over the window
        float weightRange = GetRange(weightShiftHistory, sampleCount);
        float pelvisRange = GetRange(pelvisYHistory, sampleCount);

        // Convert ranges to 0-1 stillness (low range = high stillness)
        float weightStillness = 1f - Mathf.Clamp01(weightRange / weightShiftRangeThreshold);
        float pelvisStillness = 1f - Mathf.Clamp01(pelvisRange / pelvisRangeThreshold);

        // Use the worse of the two
        currentStillness = Mathf.Min(weightStillness, pelvisStillness);

        // Track consecutive still duration
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