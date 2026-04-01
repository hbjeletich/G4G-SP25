using UnityEngine;
using UnityEngine.InputSystem;

public class StillnessTracker : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Thresholds")]
    [Tooltip("Maximum weight shift X value to count as 'still'.")]
    [SerializeField] private float weightShiftStillThreshold = 0.15f;

    [Tooltip("Maximum pelvis Y movement from baseline to count as 'still'.")]
    [SerializeField] private float pelvisStillThreshold = 0.1f;

    [Header("Smoothing")]
    [Tooltip("How quickly the stillness value responds to changes.")]
    [SerializeField] private float smoothSpeed = 3.0f;

    // Torso actions
    private InputAction weightShiftXAction;
    private InputAction pelvisPositionAction;

    // state
    private float currentStillness = 0f;
    private float stillDuration = 0f;
    private bool isTracking = false;

    public float Stillness => currentStillness;
    public float StillDuration => stillDuration;

    public bool IsStill => currentStillness > 0.8f;

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

        // Read current values using ReadValue — same pattern as game select
        float weightShiftX = weightShiftXAction != null ? Mathf.Abs(weightShiftXAction.ReadValue<float>()) : 0f;
        float pelvisY = pelvisPositionAction != null ? Mathf.Abs(pelvisPositionAction.ReadValue<Vector3>().y) : 0f;

        // Calculate raw stillness: 1 if both are below threshold, drops toward 0 as they exceed
        float weightStillness = 1f - Mathf.Clamp01(weightShiftX / weightShiftStillThreshold);
        float pelvisStillness = 1f - Mathf.Clamp01(pelvisY / pelvisStillThreshold);

        // Use the worse of the two
        float rawStillness = Mathf.Min(weightStillness, pelvisStillness);

        // Smooth it
        currentStillness = Mathf.Lerp(currentStillness, rawStillness, smoothSpeed * Time.deltaTime);

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
}