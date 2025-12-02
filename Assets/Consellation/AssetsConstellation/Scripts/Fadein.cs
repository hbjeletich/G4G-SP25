using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class SmoothPanelTransition : MonoBehaviour
{
    [Header("Fade & Timing")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private AnimationCurve fadeEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Entry Animation")]
    [SerializeField] private bool useScaleEntry = true;
    [SerializeField] private float scaleFromSize = 0.8f;  // Start slightly smaller
    [SerializeField] private float scaleOvershoot = 1.1f; // Pop to slightly bigger
    [SerializeField] private AnimationCurve scaleEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Floating Motion")]
    [SerializeField] private float driftAmplitude = 10f;
    [SerializeField] private float driftSpeed = 1f;
    [SerializeField] private float swayAngle = 5f;
    [SerializeField] private float scaleAmplitude = 0.02f;
    [SerializeField] private float scaleSpeed = 1f;

    [Header("Particles & Polish")]
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private AudioClip popSound;

    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Vector2 startPos;
    private Vector2 endPos;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private AudioSource audioSource;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Start invisible and non-interactable
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        originalScale = rect.localScale;
        originalRotation = rect.localRotation;

        endPos = rect.anchoredPosition;
        startPos = endPos + new Vector2(0, -200f);
        rect.anchoredPosition = startPos;

        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && popSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (particles != null)
            particles.Play();

        if (popSound != null && audioSource != null)
            audioSource.PlayOneShot(popSound);

        StopAllCoroutines();
        StartCoroutine(AnimateFloatyPanel());
    }

    private IEnumerator AnimateFloatyPanel()
    {
        float elapsed = 0f;

        // ENTRY PHASE: Fade + drift + scale pop
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = fadeEase.Evaluate(elapsed / fadeDuration);

            // Fade in
            canvasGroup.alpha = t;

            // Position: drift up from bottom
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            // Scale: small → overshoot → normal
            if (useScaleEntry)
            {
                float scaleT = scaleEase.Evaluate(t);
                float currentScale;

                if (scaleT < 0.7f)
                {
                    // Scale up with overshoot
                    currentScale = Mathf.Lerp(scaleFromSize, scaleOvershoot, scaleT / 0.7f);
                }
                else
                {
                    // Settle back to normal
                    currentScale = Mathf.Lerp(scaleOvershoot, 1f, (scaleT - 0.7f) / 0.3f);
                }

                rect.localScale = originalScale * currentScale;
            }

            yield return null;
        }

        // Finalize entry
        rect.anchoredPosition = endPos;
        canvasGroup.alpha = 1f;
        rect.localScale = originalScale;

        // IDLE PHASE: Continuous gentle float
        while (true)
        {
            float time = Time.time;

            float floatY = Mathf.Sin(time * driftSpeed) * driftAmplitude;
            float rotateZ = Mathf.Sin(time * driftSpeed * 0.5f) * swayAngle;
            float scaleFactor = 1f + Mathf.Sin(time * scaleSpeed) * scaleAmplitude;

            rect.anchoredPosition = endPos + new Vector2(0, floatY);
            rect.localRotation = originalRotation * Quaternion.Euler(0, 0, rotateZ);
            rect.localScale = originalScale * scaleFactor;

            yield return null;
        }
    }

    // Optional: Call from inspector button or other script
    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / (fadeDuration * 0.5f));
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
}