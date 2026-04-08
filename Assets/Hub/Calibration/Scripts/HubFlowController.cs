using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class HubFlowController : MonoBehaviour
{
    #region Inspector Fields

    [Header("Scene Flow")]
    [Tooltip("Exercises to run, in order. Reorder in the inspector as you like.")]
    [SerializeField] private List<ExerciseDefinition> exercises = new List<ExerciseDefinition>();

    [Tooltip("Number of good reps required per exercise.")]
    [SerializeField] private int requiredReps = 3;

    [Header("Calibration")]
    [Tooltip("How many seconds the user must stay still for a good extended calibration.")]
    [SerializeField] private float calibrationHoldTime = 5f;

    [Tooltip("Pause (seconds) between phases for text readability.")]
    [SerializeField] private float phasePauseTime = 2f;

    [Header("References — Cameras")]
    [SerializeField] private CapturyCameraRT capturyCam;
    [SerializeField] private PrerecordedCameraRT prerecordedCam;
    [SerializeField] private RawImage capturedRawImage;
    [SerializeField] private RawImage prerecordedRawImage;
    [SerializeField] private RawImage fullRawImage;

    [Header("References — Components")]
    [SerializeField] private StillnessTracker stillnessTracker;
    [SerializeField] private ExerciseEvaluator exerciseEvaluator;
    [SerializeField] private Animator prerecordedAnimator;

    [Header("References — UI")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Image progressRing;
    [SerializeField] private Image stillnessBar;
    [SerializeField] private GameObject progressRingContainer;
    [SerializeField] private GameObject stillnessBarContainer;
    [SerializeField] private Slider weightShiftSlider;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip repChime;
    [SerializeField] private AudioClip exerciseCompleteChime;
    [SerializeField] private AudioClip calibrationCompleteChime;

    #endregion

    #region State

    private enum HubState
    {
        WaitingForSkeleton,
        QuickCalibration,
        ExtendedCalibration,
        ExerciseIntro,
        ExerciseActive,
        ExerciseComplete,
        AllComplete
    }

    private HubState currentState = HubState.WaitingForSkeleton;
    private int currentExerciseIndex = 0;
    private bool skeletonReady = false;
    private Coroutine flowCoroutine;

    #endregion

    #region Lifecycle

    private void Start()
    {
        // Initial UI state
        SetInstructionText("Waiting for skeleton...");
        ShowStillnessBar(false);
        ShowProgressRing(false);
        SetProgressRing(0f);

        // Start with captury cam full width, prerecorded hidden
        SetCameraMode(fullWidthCaptury: true);

        // Hook into exercise evaluator events
        if (exerciseEvaluator != null)
        {
            exerciseEvaluator.OnRepCompleted += OnRepCompleted;
            exerciseEvaluator.OnExerciseComplete += OnExerciseComplete;
        }

        // Start the main flow
        flowCoroutine = StartCoroutine(MainFlow());
    }

    private void OnDestroy()
    {
        if (exerciseEvaluator != null)
        {
            exerciseEvaluator.OnRepCompleted -= OnRepCompleted;
            exerciseEvaluator.OnExerciseComplete -= OnExerciseComplete;
        }
    }

    private void Update()
    {
        // Update stillness bar UI during extended calibration
        if (currentState == HubState.ExtendedCalibration && stillnessTracker != null)
        {
            UpdateStillnessBarUI();
        }
    }

    #endregion

    #region Main Flow Coroutine

    private IEnumerator MainFlow()
    {
        // ── Phase 1: Wait for skeleton ──
        currentState = HubState.WaitingForSkeleton;
        SetInstructionText("Waiting for skeleton...");

        // Wait until the MotionTrackingManager exists and is calibrated (quick cal)
        yield return WaitForQuickCalibration();

        // ── Phase 2: Quick calibration complete ──
        currentState = HubState.QuickCalibration;
        SetInstructionText("Connected! Preparing calibration...");
        yield return new WaitForSeconds(phasePauseTime);

        // ── Phase 3: Extended calibration ──
        currentState = HubState.ExtendedCalibration;
        SetInstructionText(
            "Stand still for calibration.\n" +
            "Feet shoulder-width apart.\n" +
            "Shoulders back, arms at your sides."
        );
        ShowStillnessBar(true);

        yield return RunExtendedCalibration();

        ShowStillnessBar(false);

        // calibrate from this good pose
        if (MotionTrackingManager.Instance != null)
        {
            MotionTrackingManager.Instance.Recalibrate();

            float calDelay = MotionTrackingManager.Instance.Config != null
                ? MotionTrackingManager.Instance.Config.calibrationDelay
                : 2f;
            yield return new WaitForSeconds(calDelay + 0.5f);
        }

        if (CalibrationGuard.Instance != null)
        {
            CalibrationGuard.Instance.SaveCalibration();
        }

        PlayAudio(calibrationCompleteChime);
        SetInstructionText("Calibration saved!");
        yield return new WaitForSeconds(phasePauseTime);

        // ── Phase 4: Exercises ──
        // Switch to split view
        SetCameraMode(fullWidthCaptury: false);

        for (currentExerciseIndex = 0; currentExerciseIndex < exercises.Count; currentExerciseIndex++)
        {
            var exercise = exercises[currentExerciseIndex];

            // Intro
            currentState = HubState.ExerciseIntro;
            SetInstructionText($"{exercise.exerciseName}\n{exercise.instructionText}");
            PlayPrerecordedAnimation(exercise.animIndex);
            ShowProgressRing(true);
            SetProgressRing(0f);
            G4G.ExerciseIndicatorManager.Instance?.Show(exercise.exerciseType);
            yield return new WaitForSeconds(phasePauseTime);

            // Active — evaluate until 3 good reps
            currentState = HubState.ExerciseActive;
            SetInstructionText($"{exercise.exerciseName}\nTry it! ({0}/{requiredReps})");
            exerciseEvaluator.StartExercise(exercise.exerciseType);

            // Wait for OnExerciseComplete to fire (checked via evaluator state)
            while (exerciseEvaluator.IsActive)
            {
                if(weightShiftSlider != null && stillnessTracker != null)
                {
                    weightShiftSlider.value = stillnessTracker.WeightShift;
                }
                yield return null;
            }

            // Exercise complete
            currentState = HubState.ExerciseComplete;
            exerciseEvaluator.StopExercise();
            G4G.ExerciseIndicatorManager.Instance?.Hide();
            PlayAudio(exerciseCompleteChime);
            SetInstructionText($"{exercise.exerciseName}\nGreat job!");
            SetProgressRing(1f);
            yield return new WaitForSeconds(phasePauseTime);

            ShowProgressRing(false);
        }

        // ── Phase 5: All done ──
        currentState = HubState.AllComplete;
        G4G.ExerciseIndicatorManager.Instance?.HideImmediate();
        SetInstructionText("You're all set! Loading game...");
        yield return new WaitForSeconds(phasePauseTime);

        // Load the game scene
        string sceneName = CalibrationGuard.Instance != null
            ? CalibrationGuard.Instance.nextSceneName
            : "NewGameSelect";

        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Calibration

    private IEnumerator WaitForQuickCalibration()
    {
        // Wait for the manager to exist
        while (MotionTrackingManager.Instance == null)
        {
            yield return null;
        }

        SetInstructionText(
            "Stand still. Feet shoulder-width apart.\n" +
            "Shoulders back, arms at your sides.\n" +
            "Calibrating..."
        );

        // Wait for the system to calibrate
        while (!MotionTrackingManager.Instance.IsSystemCalibrated)
        {
            yield return null;
        }

        skeletonReady = true;
    }

    private IEnumerator RunExtendedCalibration()
    {
        if (stillnessTracker == null)
        {
            Debug.LogWarning("HubFlowController: No StillnessTracker — skipping extended calibration.");
            yield break;
        }

        stillnessTracker.StartTracking();

        // Wait until the user has been still for the required hold time
        while (!stillnessTracker.HasBeenStillFor(calibrationHoldTime))
        {
            yield return null;
        }

        stillnessTracker.StopTracking();
    }

    #endregion

    #region Exercise Callbacks

    private void OnRepCompleted(int current, int required)
    {
        PlayAudio(repChime);
        SetProgressRing((float)current / required);

        var exercise = exercises[currentExerciseIndex];
        SetInstructionText($"{exercise.exerciseName}\nKeep going! ({current}/{required})");
    }

    private void OnExerciseComplete()
    {
        // Handled in the main coroutine (evaluator.IsActive becomes false)
    }

    #endregion

    #region Camera Control

    private void SetCameraMode(bool fullWidthCaptury)
    {
        if (capturyCam != null)
        {
            capturyCam.gameObject.SetActive(true);
            capturyCam.GetComponent<Camera>().enabled = true;

            capturyCam.SwapRenderTexture(fullWidthCaptury);
        }

        if (prerecordedCam != null)
        {
            // Disable the camera component so it stops rendering to screen
            prerecordedCam.GetComponent<Camera>().enabled = !fullWidthCaptury;
            prerecordedCam.gameObject.SetActive(!fullWidthCaptury);
        }

        // Show/hide the prerecorded raw image
        if (prerecordedRawImage != null)
        {
            prerecordedRawImage.gameObject.SetActive(!fullWidthCaptury);
        }

        // Resize the captury raw image
        if (capturedRawImage != null && fullRawImage != null)
        {
            var rect = capturedRawImage.rectTransform;
            if (fullWidthCaptury)
            {
                // Full width
                fullRawImage.gameObject.SetActive(true);
                capturedRawImage.gameObject.SetActive(false);
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
            }
            else
            {
                // Left half
                fullRawImage.gameObject.SetActive(false);
                capturedRawImage.gameObject.SetActive(true);
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0.5f, 1);
            }
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        if (prerecordedRawImage != null)
        {
            var rect = prerecordedRawImage.rectTransform;
            // Right half
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }

    #endregion

    #region Animation

    private void PlayPrerecordedAnimation(int animIndex)
    {
        if (prerecordedAnimator == null) return;

        prerecordedAnimator.SetInteger("AnimIndex", animIndex);
        prerecordedAnimator.Play("", 0, 0f);
        Debug.Log($"HubFlowController: Playing animation index {animIndex}");
    }

    #endregion

    #region UI Helpers

    private void SetInstructionText(string text)
    {
        if (instructionText != null)
            instructionText.text = text;
    }

    private void SetProgressRing(float fillAmount)
    {
        StartCoroutine(LerpProgressRing(fillAmount));
    }

    private IEnumerator LerpProgressRing(float targetFill)
    {
        if (progressRing == null) yield break;

        float startFill = progressRing.fillAmount;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            progressRing.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            yield return null;
        }

        progressRing.fillAmount = targetFill;
    }

    private void ShowProgressRing(bool show)
    {
        if (progressRingContainer != null)
            progressRingContainer.SetActive(show);
    }

    private void ShowStillnessBar(bool show)
    {
        if (stillnessBarContainer != null)
            stillnessBarContainer.SetActive(show);
    }

    private void UpdateStillnessBarUI()
    {
        if (stillnessBar != null && stillnessTracker != null)
        {
            // Fill based on how long they've been still relative to the required hold time
            float fill = Mathf.Clamp01(stillnessTracker.StillDuration / calibrationHoldTime);
            stillnessBar.fillAmount = fill;

            // Color: green when still, yellow when close, red when moving
            stillnessBar.color = Color.Lerp(Color.red, Color.green, stillnessTracker.Stillness);
        }
    }

    #endregion

    #region Audio

    private void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    #endregion
}