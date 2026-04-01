using UnityEngine;
using UnityEngine.InputSystem;

public class ExerciseEvaluator : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private int requiredReps = 3;

    [Header("Squat Settings")]
    [SerializeField] private float squatThreshold = -0.3f;
    [SerializeField] private float squatReturnThreshold = -0.1f;

    [Header("Leg Lift Settings")]
    [SerializeField] private float legLiftHoldTime = 1.0f;

    [Header("Weight Shift Settings")]
    [SerializeField] private float weightShiftWindow = 2.0f;

    [Header("Button Threshold")]
    [Tooltip("Button float value above this counts as 'pressed'. Buttons are 0 or 1 floats.")]
    [SerializeField] private float buttonThreshold = 0.5f;

    // events
    public System.Action<int, int> OnRepCompleted;
    public System.Action OnExerciseComplete;

    // Torso actions
    private InputAction weightShiftLeftAction;
    private InputAction weightShiftRightAction;
    private InputAction pelvisPositionAction;

    // Foot actions
    private InputAction footRaisedAction;
    private InputAction leftHipAbductedAction;
    private InputAction rightHipAbductedAction;

    // state
    private ExerciseType currentExercise;
    private int currentReps = 0;
    private bool isActive = false;

    // squat state
    private bool isInSquat = false;

    // leg lift state
    private bool footIsRaised = false;
    private float footRaisedStartTime = 0f;

    // weight shift state
    private bool shiftedLeft = false;
    private bool shiftedRight = false;
    private float lastShiftTime = 0f;

    // hip abduction state
    private bool isAbducted = false;

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("ExerciseEvaluator: No InputActionAsset assigned!");
            return;
        }

        // Torso map — exact names from CapturyInputActions.inputactions
        var torsoMap = inputActions.FindActionMap("Torso");
        if (torsoMap != null)
        {
            weightShiftLeftAction = torsoMap.FindAction("WeightShiftLeft");
            weightShiftRightAction = torsoMap.FindAction("WeightShiftRight");
            pelvisPositionAction = torsoMap.FindAction("PelvisPosition");
        }

        // Foot map — exact names from CapturyInputActions.inputactions
        var footMap = inputActions.FindActionMap("Foot");
        if (footMap != null)
        {
            footRaisedAction = footMap.FindAction("FootRaised");
            leftHipAbductedAction = footMap.FindAction("LeftHipAbducted");
            rightHipAbductedAction = footMap.FindAction("RightHipAbducted");
        }
    }

    private void OnEnable()
    {
        EnableAction(weightShiftLeftAction);
        EnableAction(weightShiftRightAction);
        EnableAction(pelvisPositionAction);
        EnableAction(footRaisedAction);
        EnableAction(leftHipAbductedAction);
        EnableAction(rightHipAbductedAction);
    }

    private void OnDisable()
    {
        DisableAction(weightShiftLeftAction);
        DisableAction(weightShiftRightAction);
        DisableAction(pelvisPositionAction);
        DisableAction(footRaisedAction);
        DisableAction(leftHipAbductedAction);
        DisableAction(rightHipAbductedAction);
    }

    private void Update()
    {
        if (!isActive) return;

        switch (currentExercise)
        {
            case ExerciseType.Squat:
                EvaluateSquat();
                break;
            case ExerciseType.LegLift:
                EvaluateLegLift();
                break;
            case ExerciseType.WeightShift:
                EvaluateWeightShift();
                break;
            case ExerciseType.HipAbduction:
                EvaluateHipAbduction();
                break;
        }
    }

    public void StartExercise(ExerciseType type)
    {
        currentExercise = type;
        currentReps = 0;
        isActive = true;
        ResetExerciseState();
        Debug.Log($"ExerciseEvaluator: Started evaluating {type}");
    }

    public void StopExercise()
    {
        isActive = false;
        ResetExerciseState();
    }

    public int CurrentReps => currentReps;
    public int RequiredReps => requiredReps;
    public bool IsActive => isActive;

    #region Exercise Evaluators

    private void EvaluateSquat()
    {
        if (pelvisPositionAction == null) return;

        float pelvisY = pelvisPositionAction.ReadValue<Vector3>().y;

        if (!isInSquat && pelvisY < squatThreshold)
        {
            isInSquat = true;
            Debug.Log($"ExerciseEvaluator: Squat detected (pelvisY={pelvisY:F2})");
        }
        else if (isInSquat && pelvisY > squatReturnThreshold)
        {
            isInSquat = false;
            RegisterRep();
        }
    }

    private void EvaluateLegLift()
    {
        if (footRaisedAction == null) return;

        // FootRaised is a Button type action
        bool raised = footRaisedAction.ReadValue<float>() > buttonThreshold;

        if (raised && !footIsRaised)
        {
            footIsRaised = true;
            footRaisedStartTime = Time.time;
            Debug.Log("ExerciseEvaluator: Foot raise started");
        }
        else if (!raised && footIsRaised)
        {
            if (Time.time - footRaisedStartTime >= legLiftHoldTime)
            {
                RegisterRep();
            }
            else
            {
                Debug.Log($"ExerciseEvaluator: Foot lowered too fast ({Time.time - footRaisedStartTime:F1}s < {legLiftHoldTime}s)");
            }
            footIsRaised = false;
        }
    }

    private void EvaluateWeightShift()
    {
        if (weightShiftLeftAction == null || weightShiftRightAction == null) return;


        bool leftPressed = weightShiftLeftAction.triggered;
        bool rightPressed = weightShiftRightAction.triggered;

        if (leftPressed && !shiftedLeft)
        {
            shiftedLeft = true;
            lastShiftTime = Time.time;
            Debug.Log("ExerciseEvaluator: Weight shifted left");
        }

        if (rightPressed && !shiftedRight)
        {
            shiftedRight = true;
            lastShiftTime = Time.time;
            Debug.Log("ExerciseEvaluator: Weight shifted right");
        }

        // Both sides hit within the window = one complete rep
        if (shiftedLeft && shiftedRight)
        {
            RegisterRep();
            shiftedLeft = false;
            shiftedRight = false;
        }

        // Timeout
        if ((shiftedLeft || shiftedRight) && Time.time - lastShiftTime > weightShiftWindow)
        {
            shiftedLeft = false;
            shiftedRight = false;
        }
    }

    private void EvaluateHipAbduction()
    {
        if (leftHipAbductedAction == null && rightHipAbductedAction == null) return;

        bool leftAbducted = leftHipAbductedAction != null && leftHipAbductedAction.triggered;
        bool rightAbducted = rightHipAbductedAction != null && rightHipAbductedAction.triggered;
        bool anyAbducted = leftAbducted || rightAbducted;

        if (anyAbducted && !isAbducted)
        {
            isAbducted = true;
            Debug.Log("ExerciseEvaluator: Hip abduction detected");
        }
        else if (!anyAbducted && isAbducted)
        {
            isAbducted = false;
            RegisterRep();
        }
    }

    #endregion

    #region Helpers

    private void RegisterRep()
    {
        currentReps++;
        Debug.Log($"ExerciseEvaluator: Good rep! ({currentReps}/{requiredReps})");
        OnRepCompleted?.Invoke(currentReps, requiredReps);

        if (currentReps >= requiredReps)
        {
            isActive = false;
            Debug.Log("ExerciseEvaluator: Exercise complete!");
            OnExerciseComplete?.Invoke();
        }
    }

    private void ResetExerciseState()
    {
        isInSquat = false;
        footIsRaised = false;
        footRaisedStartTime = 0f;
        shiftedLeft = false;
        shiftedRight = false;
        lastShiftTime = 0f;
        isAbducted = false;
    }

    private void EnableAction(InputAction action) => action?.Enable();
    private void DisableAction(InputAction action) => action?.Disable();

    #endregion
}