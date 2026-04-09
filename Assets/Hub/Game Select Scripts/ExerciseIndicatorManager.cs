using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace G4G
{
    /// <summary>
    /// Centralized exercise indicator that any minigame can call.
    /// Slides an animated sprite in/out from the bottom-right corner.
    ///
    /// Setup:
    /// 1. Create a Canvas with a child panel anchored bottom-right (off-screen to start).
    /// 2. Add an Image + Animator on that panel for the exercise sprite animation.
    /// 3. Create one Animator Controller with an integer parameter "ExerciseType":
    ///      0 = Idle/Empty (no animation, blank sprite)
    ///      1 = HipAbduction
    ///      2 = WeightShift
    ///      3 = LegLift
    ///      4 = Squat
    ///    Each state plays the corresponding sprite animation.
    ///    Transitions: Any State -> each state, conditioned on ExerciseType == N.
    /// 4. Drag references into the inspector.
    ///
    /// Usage from any game:
    ///   ExerciseIndicatorManager.Instance.Show(ExerciseType.Squat);
    ///   ExerciseIndicatorManager.Instance.Hide();
    /// </summary>
    public class ExerciseIndicatorManager : MonoBehaviour
    {
        public static ExerciseIndicatorManager Instance;

        [Header("UI References")]
        public RectTransform indicatorPanel;
        public Animator exerciseAnimator;
        public string exerciseTypeParam = "ExerciseType";

        [Header("Slide Settings")]
        public Vector2 onScreenPosition = new Vector2(-120f, 120f);
        public Vector2 offScreenPosition = new Vector2(300f, -300f);
        public float slideDuration = 0.3f;

        [Tooltip("Ease curve for the slide.")]
        public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Coroutine slideCoroutine;
        private Coroutine tutorialCoroutine;
        private bool isVisible = false;
        private bool hasShownGameSelectTutorial = false;

        [Header("Game Select Tutorial")]
        public float gameSelectWeightShiftDuration = 4f;
        public float gameSelectSquatDuration = 4f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Start hidden
            if (indicatorPanel != null)
                indicatorPanel.anchoredPosition = offScreenPosition;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Show(ExerciseType type)
        {
            if (exerciseAnimator != null)
                exerciseAnimator.enabled = true;

            SetAnimatorState(type);

            if (!isVisible)
                SlideIn();
        }

        public void Hide()
        {
            if (!isVisible) return;
            SlideOut();
        }

        public void HideImmediate()
        {
            if (slideCoroutine != null)
                StopCoroutine(slideCoroutine);
            if (tutorialCoroutine != null)
                StopCoroutine(tutorialCoroutine);

            isVisible = false;
            if (exerciseAnimator != null)
                exerciseAnimator.enabled = false;

            if (indicatorPanel != null)
                indicatorPanel.anchoredPosition = offScreenPosition;
        }

        public void ShowGameSelectTutorial()
        {
            if (hasShownGameSelectTutorial) return;
            hasShownGameSelectTutorial = true;
            tutorialCoroutine = StartCoroutine(GameSelectTutorialRoutine());
        }

        private IEnumerator GameSelectTutorialRoutine()
        {
            Show(ExerciseType.WeightShift);
            yield return new WaitForSecondsRealtime(gameSelectWeightShiftDuration);

            Show(ExerciseType.Squat);
            yield return new WaitForSecondsRealtime(gameSelectSquatDuration);

            Hide();
            tutorialCoroutine = null;
        }

        private void SetAnimatorState(ExerciseType type)
        {
            if (exerciseAnimator != null)
                exerciseAnimator.SetInteger(exerciseTypeParam, (int)type);
        }

        private void SlideIn()
        {
            if (slideCoroutine != null)
                StopCoroutine(slideCoroutine);

            isVisible = true;
            slideCoroutine = StartCoroutine(SlideTo(onScreenPosition));
        }

        private void SlideOut()
        {
            if (slideCoroutine != null)
                StopCoroutine(slideCoroutine);

            isVisible = false;
            slideCoroutine = StartCoroutine(SlideTo(offScreenPosition, resetOnComplete: true));
        }

        private IEnumerator SlideTo(Vector2 target, bool resetOnComplete = false)
        {
            if (indicatorPanel == null) yield break;

            Vector2 start = indicatorPanel.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < slideDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = slideCurve.Evaluate(Mathf.Clamp01(elapsed / slideDuration));
                indicatorPanel.anchoredPosition = Vector2.Lerp(start, target, t);
                yield return null;
            }

            indicatorPanel.anchoredPosition = target;

            if (resetOnComplete && exerciseAnimator != null)
                exerciseAnimator.enabled = false;
        }
    }
}