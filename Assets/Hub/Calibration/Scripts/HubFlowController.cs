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
    [SerializeField] private Image weightShiftHandle;
    [SerializeField] private float weightShiftGreenMin = 0.15f;
    [SerializeField] private float weightShiftGreenMax = 0.45f;
    [SerializeField] private float weightShiftRedMin = 0.7f;

    [Header("Text Panels")]
    [Tooltip("Panel containing calibration instructions. Slides on/off screen.")]
    [SerializeField] private RectTransform calibrationPanel;
    [SerializeField] private TextMeshProUGUI calibrationText;
    [SerializeField] private Vector2 calibrationOnScreen = Vector2.zero;
    [SerializeField] private Vector2 calibrationOffScreen = new Vector2(0f, -400f);

    [Tooltip("Panel containing exercise label and description. Slides on/off screen.")]
    [SerializeField] private RectTransform exercisePanel;
    [SerializeField] private TextMeshProUGUI exerciseLabel;
    [SerializeField] private TextMeshProUGUI exerciseDescription;
    [SerializeField] private Vector2 exerciseOnScreen = Vector2.zero;
    [SerializeField] private Vector2 exerciseOffScreen = new Vector2(0f, -400f);

    [SerializeField] private float panelSlideDuration = 0.4f;
    [SerializeField] private AnimationCurve panelSlideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip repChime;
    [SerializeField] private AudioClip exerciseCompleteChime;
    [SerializeField] private AudioClip calibrationCompleteChime;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private KeyCode debugAdvanceKey = KeyCode.Space;
    [SerializeField] private KeyCode debugSkipCalibrationKey = KeyCode.S;
    [SerializeField] private KeyCode debugCompleteExerciseKey = KeyCode.Return;
    [SerializeField] private KeyCode debugPrevAnimKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode debugNextAnimKey = KeyCode.RightArrow;

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

    // Debug
    private bool debugAdvancePressed = false;
    private int debugAnimIndex = 0;
    private int currentAnimIndex = -1;

    #endregion

    #region Lifecycle

    private void Start()
    {
        SetInstructionText("Waiting for skeleton...");
        ShowStillnessBar(false);
        ShowProgressRing(false);
        ShowWeightShiftSlider(false);
        SetProgressRing(0f);

        // Start panels off-screen
        if (calibrationPanel != null)
            calibrationPanel.anchoredPosition = calibrationOffScreen;
        if (exercisePanel != null)
            exercisePanel.anchoredPosition = exerciseOffScreen;

        // Always split view — prerecorded skeleton always visible next to player
        SetCameraMode(fullWidthCaptury: false);

        if (exerciseEvaluator != null)
        {
            exerciseEvaluator.OnRepCompleted += OnRepCompleted;
            exerciseEvaluator.OnExerciseComplete += OnExerciseComplete;
        }

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
        if (currentState == HubState.ExtendedCalibration && stillnessTracker != null)
        {
            UpdateStillnessBarUI();
        }

        if (debugMode)
        {
            HandleDebugInput();
        }
    }

    #endregion

    #region Main Flow Coroutine

    private IEnumerator MainFlow()
    {
        // ── Phase 1: Wait for skeleton ──
        currentState = HubState.WaitingForSkeleton;

        if (debugMode)
        {
            SetCalibrationText("[DEBUG] Press SPACE to skip skeleton wait");
            yield return SlidePanel(calibrationPanel, calibrationOffScreen, calibrationOnScreen);
            yield return WaitForDebugAdvance();
            skeletonReady = true;
        }
        else
        {
            SetCalibrationText("Waiting for skeleton...");
            yield return SlidePanel(calibrationPanel, calibrationOffScreen, calibrationOnScreen);
            yield return WaitForQuickCalibration();
        }

        // ── Phase 2: Quick calibration complete ──
        currentState = HubState.QuickCalibration;
        SetCalibrationText("Connected! Preparing calibration...");
        yield return new WaitForSeconds(phasePauseTime);

        // ── Phase 3: Extended calibration ──
        currentState = HubState.ExtendedCalibration;
        ShowStillnessBar(true);

        if (debugMode)
        {
            SetCalibrationText("[DEBUG] Press S to skip calibration");
            yield return RunExtendedCalibrationDebug();
        }
        else
        {
            SetCalibrationText(
                "Stand still for calibration.\n" +
                "Feet shoulder-width apart.\n" +
                "Shoulders back, arms at your sides."
            );
            yield return RunExtendedCalibration();
        }

        ShowStillnessBar(false);

        if (!debugMode && MotionTrackingManager.Instance != null)
        {
            MotionTrackingManager.Instance.Recalibrate();

            float calDelay = MotionTrackingManager.Instance.Config != null
                ? MotionTrackingManager.Instance.Config.calibrationDelay
                : 2f;
            yield return new WaitForSeconds(calDelay + 0.5f);
        }

        if (!debugMode && CalibrationGuard.Instance != null)
        {
            CalibrationGuard.Instance.SaveCalibration();
        }

        PlayAudio(calibrationCompleteChime);
        SetCalibrationText("Calibration saved!");
        yield return new WaitForSeconds(phasePauseTime);

        // ── Transition: slide calibration out, exercise in ──
        yield return SlidePanel(calibrationPanel, calibrationOnScreen, calibrationOffScreen);

        // ── Phase 4: Exercises ──
        for (currentExerciseIndex = 0; currentExerciseIndex < exercises.Count; currentExerciseIndex++)
        {
            var exercise = exercises[currentExerciseIndex];
            bool isWeightShift = exercise.exerciseType == ExerciseType.WeightShift;

            // Intro — set text then slide in
            currentState = HubState.ExerciseIntro;
            SetExerciseText(exercise.exerciseName, exercise.instructionText);
            PlayPrerecordedAnimation(exercise.animIndex);
            debugAnimIndex = exercise.animIndex;
            ShowProgressRing(true);
            SetProgressRing(0f);
            ShowWeightShiftSlider(isWeightShift);
            G4G.ExerciseIndicatorManager.Instance?.Show(exercise.exerciseType);

            yield return SlidePanel(exercisePanel, exerciseOffScreen, exerciseOnScreen);

            if (debugMode)
            {
                yield return WaitForDebugAdvance();
            }
            else
            {
                yield return new WaitForSeconds(phasePauseTime);
            }

            // Active
            currentState = HubState.ExerciseActive;

            if (debugMode)
            {
                SetExerciseText($"[DEBUG] {exercise.exerciseName}", "ENTER=complete  ←/→=change anim");
                yield return WaitForDebugExerciseComplete();
            }
            else
            {
                SetExerciseText(exercise.exerciseName, $"Try it! ({0}/{requiredReps})");
                exerciseEvaluator.StartExercise(exercise.exerciseType);

                while (exerciseEvaluator.IsActive)
                {
                    if (isWeightShift && weightShiftSlider != null && stillnessTracker != null)
                    {
                        weightShiftSlider.value = stillnessTracker.WeightShift;
                        UpdateWeightShiftHandleColor(stillnessTracker.WeightShift);
                    }
                    yield return null;
                }
            }

            // Exercise complete
            currentState = HubState.ExerciseComplete;
            if (!debugMode) exerciseEvaluator.StopExercise();
            G4G.ExerciseIndicatorManager.Instance?.Hide();
            ShowWeightShiftSlider(false);
            PlayAudio(exerciseCompleteChime);
            SetExerciseText(exercise.exerciseName, "Great job!");
            SetProgressRing(1f);
            yield return new WaitForSeconds(phasePauseTime);

            // Slide exercise panel out before next exercise or end
            yield return SlidePanel(exercisePanel, exerciseOnScreen, exerciseOffScreen);
            ShowProgressRing(false);
        }

        // ── Phase 5: All done ──
        currentState = HubState.AllComplete;
        G4G.ExerciseIndicatorManager.Instance?.HideImmediate();

        if (debugMode)
        {
            SetCalibrationText("[DEBUG] All done! Press SPACE to load scene");
            yield return SlidePanel(calibrationPanel, calibrationOffScreen, calibrationOnScreen);
            yield return WaitForDebugAdvance();
        }
        else
        {
            SetCalibrationText("You're all set! Loading game...");
            yield return SlidePanel(calibrationPanel, calibrationOffScreen, calibrationOnScreen);
            yield return new WaitForSeconds(phasePauseTime);
        }

        string sceneName = CalibrationGuard.Instance != null
            ? CalibrationGuard.Instance.nextSceneName
            : "NewGameSelect";

        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Calibration

    private IEnumerator WaitForQuickCalibration()
    {
        while (MotionTrackingManager.Instance == null)
        {
            yield return null;
        }

        SetInstructionText(
            "Stand still. Feet shoulder-width apart.\n" +
            "Shoulders back, arms at your sides.\n" +
            "Calibrating..."
        );

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

        while (!stillnessTracker.HasBeenStillFor(calibrationHoldTime))
        {
            yield return null;
        }

        stillnessTracker.StopTracking();
    }

    private IEnumerator RunExtendedCalibrationDebug()
    {
        if (stillnessTracker != null)
            stillnessTracker.StartTracking();

        while (true)
        {
            if (Input.GetKeyDown(debugSkipCalibrationKey))
            {
                Debug.Log("[Debug] Skipping calibration");
                break;
            }

            if (stillnessTracker != null && stillnessTracker.HasBeenStillFor(calibrationHoldTime))
                break;

            yield return null;
        }

        if (stillnessTracker != null)
            stillnessTracker.StopTracking();
    }

    #endregion

    #region Debug

    private void HandleDebugInput()
    {
        if (Input.GetKeyDown(debugAdvanceKey))
        {
            debugAdvancePressed = true;
        }

        if (currentState == HubState.ExerciseActive || currentState == HubState.ExerciseIntro)
        {
            if (Input.GetKeyDown(debugPrevAnimKey))
            {
                debugAnimIndex = Mathf.Max(0, debugAnimIndex - 1);
                PlayPrerecordedAnimation(debugAnimIndex);
                Debug.Log($"[Debug] Animation index: {debugAnimIndex}");
            }
            if (Input.GetKeyDown(debugNextAnimKey))
            {
                debugAnimIndex++;
                PlayPrerecordedAnimation(debugAnimIndex);
                Debug.Log($"[Debug] Animation index: {debugAnimIndex}");
            }
        }
    }

    private IEnumerator WaitForDebugAdvance()
    {
        debugAdvancePressed = false;
        while (!debugAdvancePressed)
        {
            yield return null;
        }
        debugAdvancePressed = false;
    }

    private IEnumerator WaitForDebugExerciseComplete()
    {
        while (!Input.GetKeyDown(debugCompleteExerciseKey))
        {
            yield return null;
        }
    }

    private void OnGUI()
    {
        if (!debugMode) return;

        float w = 320f;
        float h = 300f;
        float x = Screen.width - w - 10f;
        float y = 10f;

        GUI.Box(new Rect(x, y, w, h), "");

        GUILayout.BeginArea(new Rect(x + 10, y + 5, w - 20, h - 10));

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 14;
        GUILayout.Label("Calibration Debug", titleStyle);

        GUILayout.Space(5);
        GUILayout.Label($"State: {currentState}");
        GUILayout.Label($"Exercise: {currentExerciseIndex}/{exercises.Count}");

        if (currentExerciseIndex < exercises.Count)
        {
            var ex = exercises[currentExerciseIndex];
            GUILayout.Label($"  Name: {ex.exerciseName}");
            GUILayout.Label($"  Type: {ex.exerciseType}");
        }

        GUILayout.Label($"Anim Index: {debugAnimIndex}");

        GUILayout.Space(8);
        GUILayout.Label("Keys:", titleStyle);

        GUIStyle ctrlStyle = new GUIStyle(GUI.skin.label);
        ctrlStyle.fontSize = 11;
        GUILayout.Label($"  [{debugAdvanceKey}] Advance / Skip", ctrlStyle);
        GUILayout.Label($"  [{debugSkipCalibrationKey}] Skip Calibration", ctrlStyle);
        GUILayout.Label($"  [{debugCompleteExerciseKey}] Complete Exercise", ctrlStyle);
        GUILayout.Label($"  [←/→] Change Animation", ctrlStyle);

        GUILayout.Space(8);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("◀ Prev Anim"))
        {
            debugAnimIndex = Mathf.Max(0, debugAnimIndex - 1);
            PlayPrerecordedAnimation(debugAnimIndex);
        }
        if (GUILayout.Button("Next Anim ▶"))
        {
            debugAnimIndex++;
            PlayPrerecordedAnimation(debugAnimIndex);
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Advance"))
        {
            debugAdvancePressed = true;
        }

        GUILayout.EndArea();
    }

    #endregion

    #region Exercise Callbacks

    private void OnRepCompleted(int current, int required)
    {
        PlayAudio(repChime);
        SetProgressRing((float)current / required);

        var exercise = exercises[currentExerciseIndex];
        SetExerciseText(exercise.exerciseName, $"Keep going! ({current}/{required})");
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
            prerecordedCam.GetComponent<Camera>().enabled = !fullWidthCaptury;
            prerecordedCam.gameObject.SetActive(!fullWidthCaptury);
        }

        if (prerecordedRawImage != null)
        {
            prerecordedRawImage.gameObject.SetActive(!fullWidthCaptury);
        }

        if (capturedRawImage != null && fullRawImage != null)
        {
            var rect = capturedRawImage.rectTransform;
            if (fullWidthCaptury)
            {
                fullRawImage.gameObject.SetActive(true);
                capturedRawImage.gameObject.SetActive(false);
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
            }
            else
            {
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
        if (animIndex == currentAnimIndex) return;

        currentAnimIndex = animIndex;
        prerecordedAnimator.SetInteger("AnimIndex", animIndex);
        Debug.Log($"HubFlowController: Playing animation index {animIndex}");
    }

    #endregion

    #region UI Helpers

    private void SetInstructionText(string text)
    {
        if (instructionText != null)
            instructionText.text = text;
    }

    private void SetCalibrationText(string text)
    {
        if (calibrationText != null)
            calibrationText.text = text;
        SetInstructionText(text);
    }

    private void SetExerciseText(string label, string description)
    {
        if (exerciseLabel != null)
            exerciseLabel.text = label;
        if (exerciseDescription != null)
            exerciseDescription.text = description;
        SetInstructionText($"{label}\n{description}");
    }

    private IEnumerator SlidePanel(RectTransform panel, Vector2 from, Vector2 to)
    {
        if (panel == null) yield break;

        panel.anchoredPosition = from;
        float elapsed = 0f;

        while (elapsed < panelSlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = panelSlideCurve.Evaluate(Mathf.Clamp01(elapsed / panelSlideDuration));
            panel.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        panel.anchoredPosition = to;
    }

    private void ShowWeightShiftSlider(bool show)
    {
        if (weightShiftSlider != null)
            weightShiftSlider.gameObject.SetActive(show);
    }

    private void UpdateWeightShiftHandleColor(float value)
    {
        if (weightShiftHandle == null) return;

        float abs = Mathf.Abs(value);

        if (abs < weightShiftGreenMin)
        {
            // Dead zone — grey
            weightShiftHandle.color = Color.gray;
        }
        else if (abs <= weightShiftGreenMax)
        {
            // Sweet spot — grey to green
            float t = Mathf.InverseLerp(weightShiftGreenMin, weightShiftGreenMax, abs);
            weightShiftHandle.color = Color.Lerp(Color.gray, Color.green, t);
        }
        else if (abs < weightShiftRedMin)
        {
            // Transition — green to red
            float t = Mathf.InverseLerp(weightShiftGreenMax, weightShiftRedMin, abs);
            weightShiftHandle.color = Color.Lerp(Color.green, Color.red, t);
        }
        else
        {
            // Too far — red
            weightShiftHandle.color = Color.red;
        }
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
            float fill = Mathf.Clamp01(stillnessTracker.StillDuration / calibrationHoldTime);
            stillnessBar.fillAmount = fill;
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