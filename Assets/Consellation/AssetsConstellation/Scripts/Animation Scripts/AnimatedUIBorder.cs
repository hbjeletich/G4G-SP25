using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Simple animated border using UI Images
/// Creates 4 border segments that grow progressively
/// </summary>
public class AnimatedUIBorder : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float animationDuration = 10f;
    [SerializeField] private float borderThickness = 5f;
    [SerializeField] private Color borderColor = Color.white;
    [SerializeField] private bool clockwiseAnimation = true;
    
    [Header("Popup Settings")]
    [SerializeField] private float popupScaleDuration = 0.5f;
    [SerializeField] private AnimationCurve popupCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Image topBorder, rightBorder, bottomBorder, leftBorder;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        CreateBorders();
        StartCoroutine(AnimateSequence());
    }
    
    private void CreateBorders()
    {
        // Create border container
        GameObject borderContainer = new GameObject("BorderContainer");
        borderContainer.transform.SetParent(transform);
        RectTransform containerRect = borderContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;
        containerRect.localScale = Vector3.one;
        
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;
        
        // Top border
        topBorder = CreateBorderSegment("TopBorder", borderContainer.transform);
        RectTransform topRect = topBorder.rectTransform;
        topRect.anchorMin = new Vector2(0, 1);
        topRect.anchorMax = new Vector2(0, 1);
        topRect.pivot = new Vector2(0, 0.5f);
        topRect.anchoredPosition = new Vector2(0, -borderThickness / 2);
        topRect.sizeDelta = new Vector2(0, borderThickness);
        
        // Right border
        rightBorder = CreateBorderSegment("RightBorder", borderContainer.transform);
        RectTransform rightRect = rightBorder.rectTransform;
        rightRect.anchorMin = new Vector2(1, 1);
        rightRect.anchorMax = new Vector2(1, 1);
        rightRect.pivot = new Vector2(0.5f, 1);
        rightRect.anchoredPosition = new Vector2(-borderThickness / 2, 0);
        rightRect.sizeDelta = new Vector2(borderThickness, 0);
        
        // Bottom border
        bottomBorder = CreateBorderSegment("BottomBorder", borderContainer.transform);
        RectTransform bottomRect = bottomBorder.rectTransform;
        bottomRect.anchorMin = new Vector2(1, 0);
        bottomRect.anchorMax = new Vector2(1, 0);
        bottomRect.pivot = new Vector2(1, 0.5f);
        bottomRect.anchoredPosition = new Vector2(0, borderThickness / 2);
        bottomRect.sizeDelta = new Vector2(0, borderThickness);
        
        // Left border
        leftBorder = CreateBorderSegment("LeftBorder", borderContainer.transform);
        RectTransform leftRect = leftBorder.rectTransform;
        leftRect.anchorMin = new Vector2(0, 0);
        leftRect.anchorMax = new Vector2(0, 0);
        leftRect.pivot = new Vector2(0.5f, 0);
        leftRect.anchoredPosition = new Vector2(borderThickness / 2, 0);
        leftRect.sizeDelta = new Vector2(borderThickness, 0);
    }
    
    private Image CreateBorderSegment(string name, Transform parent)
    {
        GameObject segment = new GameObject(name);
        segment.transform.SetParent(parent);
        segment.transform.localScale = Vector3.one;
        
        Image img = segment.AddComponent<Image>();
        img.color = borderColor;
        img.raycastTarget = false;
        
        return img;
    }
    
    private IEnumerator AnimateSequence()
    {
        // Fade in
        yield return StartCoroutine(FadeIn());
        
        // Popup scale animation
        yield return StartCoroutine(PopupScale());
        
        // Animate borders drawing
        yield return StartCoroutine(AnimateBorders());
    }
    
    private IEnumerator FadeIn()
    {
        float duration = 0.3f;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
        
        canvasGroup.alpha = 1;
    }
    
    private IEnumerator PopupScale()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 startScale = originalScale * 0.5f;
        transform.localScale = startScale;
        
        float elapsed = 0;
        
        while (elapsed < popupScaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popupScaleDuration;
            float curveValue = popupCurve.Evaluate(t);
            transform.localScale = Vector3.Lerp(startScale, originalScale, curveValue);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    private IEnumerator AnimateBorders()
    {
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;
        
        // Each border gets 1/4 of the total animation time
        float timePerBorder = animationDuration / 4f;
        
        Image[] borders = clockwiseAnimation 
            ? new Image[] { topBorder, rightBorder, bottomBorder, leftBorder }
            : new Image[] { leftBorder, bottomBorder, rightBorder, topBorder };
        
        float[] maxSizes = { width, height, width, height };
        bool[] isHorizontal = { true, false, true, false };
        
        for (int i = 0; i < 4; i++)
        {
            yield return StartCoroutine(AnimateBorderSegment(borders[i], maxSizes[i], isHorizontal[i], timePerBorder));
        }
    }
    
    private IEnumerator AnimateBorderSegment(Image border, float maxSize, bool isHorizontal, float duration)
    {
        RectTransform rect = border.rectTransform;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Smooth easing
            float easedT = Mathf.SmoothStep(0, 1, t);
            float currentSize = Mathf.Lerp(0, maxSize, easedT);
            
            if (isHorizontal)
            {
                rect.sizeDelta = new Vector2(currentSize, borderThickness);
            }
            else
            {
                rect.sizeDelta = new Vector2(borderThickness, currentSize);
            }
            
            yield return null;
        }
        
        // Ensure final size is exact
        if (isHorizontal)
        {
            rect.sizeDelta = new Vector2(maxSize, borderThickness);
        }
        else
        {
            rect.sizeDelta = new Vector2(borderThickness, maxSize);
        }
    }
    
    public void Hide()
    {
        StartCoroutine(FadeOut());
    }
    
    private IEnumerator FadeOut()
    {
        float duration = 0.5f;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            yield return null;
        }
        
        gameObject.SetActive(false);
    }
}
