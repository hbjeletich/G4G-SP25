using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimatedOutlinePopup : MonoBehaviour
{
    [Header("Popup Settings")]
    [SerializeField] private Image popupImage; // The "Well Done!" ram image
    [SerializeField] private float animationDuration = 10f;
    [SerializeField] private float outlineWidth = 5f;
    [SerializeField] private Color outlineColor = Color.white;
    
    [Header("Optional Scale Animation")]
    [SerializeField] private bool scaleOnAppear = true;
    [SerializeField] private float scaleAnimDuration = 0.5f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Material outlineMaterial;
    private CanvasGroup canvasGroup;
    
    void Awake()
    {
        // Get or add CanvasGroup for fade control
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        // Initially hide the popup
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }
    
    public void ShowPopup()
    {
        gameObject.SetActive(true);
        StartCoroutine(AnimatePopup());
    }
    
    private IEnumerator AnimatePopup()
    {
        // Fade in the popup first
        float fadeTime = 0.3f;
        float elapsed = 0;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 1;
        
        // Optional scale animation
        if (scaleOnAppear)
        {
            yield return StartCoroutine(ScaleAnimation());
        }
        
        // Start the outline drawing animation
        yield return StartCoroutine(DrawOutlineAnimation());
    }
    
    private IEnumerator ScaleAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 startScale = originalScale * 0.5f;
        transform.localScale = startScale;
        
        float elapsed = 0;
        while (elapsed < scaleAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleAnimDuration;
            float curveValue = scaleCurve.Evaluate(t);
            transform.localScale = Vector3.Lerp(startScale, originalScale, curveValue);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    private IEnumerator DrawOutlineAnimation()
    {
        // Create outline using UI Outline component animation
        Outline outline = popupImage.gameObject.GetComponent<Outline>();
        if (outline == null)
            outline = popupImage.gameObject.AddComponent<Outline>();
        
        outline.effectColor = outlineColor;
        outline.effectDistance = Vector2.zero;
        outline.enabled = true;
        
        float elapsed = 0;
        Vector2 maxDistance = new Vector2(outlineWidth, outlineWidth);
        
        // Animate the outline growing
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // You can use different easing functions here
            float easedT = EaseOutCubic(t);
            
            outline.effectDistance = Vector2.Lerp(Vector2.zero, maxDistance, easedT);
            
            yield return null;
        }
        
        outline.effectDistance = maxDistance;
    }
    
    private float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }
    
    public void HidePopup()
    {
        StartCoroutine(FadeOut());
    }
    
    private IEnumerator FadeOut()
    {
        float fadeTime = 0.5f;
        float elapsed = 0;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeTime);
            yield return null;
        }
        
        gameObject.SetActive(false);
    }
}
