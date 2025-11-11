using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class SmoothPanelTransition : MonoBehaviour
{
    [Header("Fade & Timing")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("Floating Motion")]
    [SerializeField] private float driftAmplitude = 10f;      // pixels
    [SerializeField] private float driftSpeed = 1f;           // cycles per second
    [SerializeField] private float swayAngle = 5f;            // degrees of rotation sway
    [SerializeField] private float scaleAmplitude = 0.02f;    // 2% scale oscillation
    [SerializeField] private float scaleSpeed = 1f;           // cycles per second

    [Header("Particles (Optional)")]
    [SerializeField] private ParticleSystem particles;

    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Vector2 startPos;
    private Vector2 endPos;
    private Vector3 originalScale;
    private Quaternion originalRotation;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // start invisible and non-interactable
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        originalScale = rect.localScale;
        originalRotation = rect.localRotation;

        endPos = rect.anchoredPosition;
        startPos = endPos + new Vector2(0, -200f); // float in from below
        rect.anchoredPosition = startPos;
    }

    public void Show()
    {
        gameObject.SetActive(true); // must activate before coroutine
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (particles != null)
            particles.Play();

        StopAllCoroutines();
        StartCoroutine(AnimateFloatyPanel());
    }

    private IEnumerator AnimateFloatyPanel()
    {
        float elapsed = 0f;

        // Fade & drift to center
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / fadeDuration);

            // Fade
            canvasGroup.alpha = t;

            // Position drift
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        rect.anchoredPosition = endPos;
        canvasGroup.alpha = 1f;

        // Continuous float / sway
        while (true)
        {
            float floatY = Mathf.Sin(Time.time * driftSpeed) * driftAmplitude;
            float rotateZ = Mathf.Sin(Time.time * driftSpeed * 0.5f) * swayAngle;
            float scaleFactor = 1f + Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;

            rect.anchoredPosition = endPos + new Vector2(0, floatY);
            rect.localRotation = originalRotation * Quaternion.Euler(0, 0, rotateZ);
            rect.localScale = originalScale * scaleFactor;

            yield return null;
        }
    }
}
