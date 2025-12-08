using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuAnimationController : MonoBehaviour
{
    [Header("Drag Your GameObjects Here")]
    public GameObject character;
    public GameObject speechBubble;
    public GameObject bigDipperImage;
    public GameObject infoText;

    [Header("Speech Bubble Text")]
    public string characterSpeech = "Welcome, explorer! Let me show you the wonders of the night sky!";

    [Header("Character Slide Animation")]
    public float characterSlideDuration = 1f;
    [Range(0f, 2f)]
    public float characterSlideEaseStrength = 1f;

    [Header("Speech Bubble Timing")]
    public float speechFadeInDuration = 0.5f;
    public float speechDisplayDuration = 3f;
    public float speechFadeOutDuration = 0.5f;

    [Header("Big Dipper Image Animation")]
    public float imageFadeDuration = 1.2f;
    [Range(0f, 1f)]
    public float imageStartScale = 0.7f;
    [Range(-45f, 45f)]
    public float imageStartRotation = -8f;
    [Range(0f, 2f)]
    public float imageEaseStrength = 1.2f;
    public bool useImageBounce = true;
    [Range(0f, 0.3f)]
    public float imageBounceAmount = 0.1f;

    [Header("Info Text Animation")]
    public float textFadeInDelay = 0.2f;
    public float textFadeDuration = 0.8f;
    public bool textSlideUp = true;
    public float textSlideDistance = 30f;

    [Header("General Timing")]
    public float initialDelay = 0.5f;
    public float delayBeforeImageAppears = 0.5f;

    private Vector3 characterStartPos;

    void Start()
    {
        characterStartPos = character.transform.position;
        character.transform.position = new Vector3(characterStartPos.x - 10f, characterStartPos.y, characterStartPos.z);
        
        speechBubble.SetActive(false);
        bigDipperImage.SetActive(false);
        if (infoText != null) infoText.SetActive(false);

        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        yield return new WaitForSeconds(initialDelay);

        // 1. Slide character in
        yield return StartCoroutine(SlideCharacterIn());

        // 2. Show and fade in speech bubble
        yield return StartCoroutine(ShowSpeechBubble());

        // 3. Display speech for set duration
        yield return new WaitForSeconds(speechDisplayDuration);

        // 4. Fade out speech bubble
        yield return StartCoroutine(HideSpeechBubble());

        // 5. Wait before showing image
        yield return new WaitForSeconds(delayBeforeImageAppears);

        // 6. Show Big Dipper and info text with refined animations
        yield return StartCoroutine(ShowBigDipperAndText());
    }

    IEnumerator SlideCharacterIn()
    {
        float elapsed = 0f;
        Vector3 startPos = character.transform.position;
        
        while (elapsed < characterSlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / characterSlideDuration);
            
            // Smooth ease out - using safe exponent
            float exponent = Mathf.Max(1f, 2f + characterSlideEaseStrength);
            float smoothT = 1f - Mathf.Pow(1f - t, exponent);
            smoothT = Mathf.Clamp01(smoothT);
            
            Vector3 newPos = Vector3.Lerp(startPos, characterStartPos, smoothT);
            
            // Safety check
            if (!float.IsNaN(newPos.x) && !float.IsNaN(newPos.y) && !float.IsNaN(newPos.z))
            {
                character.transform.position = newPos;
            }
            
            yield return null;
        }
        
        character.transform.position = characterStartPos;
    }

    IEnumerator ShowSpeechBubble()
    {
        speechBubble.SetActive(true);
        
        CanvasGroup bubbleGroup = speechBubble.GetComponent<CanvasGroup>();
        if (bubbleGroup == null) bubbleGroup = speechBubble.AddComponent<CanvasGroup>();
        
        bubbleGroup.alpha = 0;
        float elapsed = 0f;
        
        while (elapsed < speechFadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / speechFadeInDuration;
            bubbleGroup.alpha = t;
            yield return null;
        }
        
        bubbleGroup.alpha = 1f;

        Text bubbleText = speechBubble.GetComponentInChildren<Text>();
        if (bubbleText != null) bubbleText.text = characterSpeech;
    }

    IEnumerator HideSpeechBubble()
    {
        CanvasGroup bubbleGroup = speechBubble.GetComponent<CanvasGroup>();
        if (bubbleGroup == null) yield break;
        
        float elapsed = 0f;
        
        while (elapsed < speechFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / speechFadeOutDuration;
            bubbleGroup.alpha = 1f - t;
            yield return null;
        }
        
        speechBubble.SetActive(false);
    }

    IEnumerator ShowBigDipperAndText()
    {
        // Setup Big Dipper
        bigDipperImage.SetActive(true);
        
        CanvasGroup dipperGroup = bigDipperImage.GetComponent<CanvasGroup>();
        if (dipperGroup == null) dipperGroup = bigDipperImage.AddComponent<CanvasGroup>();
        dipperGroup.alpha = 0;
        
        Vector3 imageOriginalScale = bigDipperImage.transform.localScale;
        Quaternion imageOriginalRotation = bigDipperImage.transform.localRotation;
        
        bigDipperImage.transform.localScale = imageOriginalScale * imageStartScale;
        bigDipperImage.transform.localRotation = Quaternion.Euler(0, 0, imageStartRotation);
        
        // Start image animation
        StartCoroutine(AnimateBigDipper(dipperGroup, imageOriginalScale, imageOriginalRotation));
        
        // Wait before starting text
        if (infoText != null && textFadeInDelay > 0)
        {
            yield return new WaitForSeconds(textFadeInDelay);
        }
        
        // Start text animation
        if (infoText != null)
        {
            yield return StartCoroutine(AnimateInfoText());
        }
    }

    IEnumerator AnimateBigDipper(CanvasGroup dipperGroup, Vector3 originalScale, Quaternion originalRotation)
    {
        float elapsed = 0f;
        
        while (elapsed < imageFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / imageFadeDuration);
            
            // Custom ease out with adjustable strength
            float smoothT = 1f - Mathf.Pow(1f - t, Mathf.Max(1f, 2f + imageEaseStrength));
            smoothT = Mathf.Clamp01(smoothT);
            
            // Fade in
            dipperGroup.alpha = smoothT;
            
            // Scale animation with optional subtle bounce
            float scaleT = smoothT;
            if (useImageBounce && smoothT > 0.7f)
            {
                float bouncePhase = (smoothT - 0.7f) / 0.3f;
                float bounce = Mathf.Sin(bouncePhase * Mathf.PI) * imageBounceAmount;
                scaleT = Mathf.Clamp01(smoothT + bounce);
            }
            
            float currentScale = Mathf.Lerp(imageStartScale, 1f, scaleT);
            if (float.IsNaN(currentScale) || float.IsInfinity(currentScale))
                currentScale = 1f;
            
            bigDipperImage.transform.localScale = new Vector3(
                originalScale.x * currentScale,
                originalScale.y * currentScale,
                originalScale.z * currentScale
            );
            
            // Smooth rotation
            float currentRotation = Mathf.Lerp(imageStartRotation, 0f, smoothT);
            if (!float.IsNaN(currentRotation) && !float.IsInfinity(currentRotation))
            {
                bigDipperImage.transform.localRotation = Quaternion.Euler(0, 0, currentRotation);
            }
            
            yield return null;
        }
        
        dipperGroup.alpha = 1f;
        bigDipperImage.transform.localScale = originalScale;
        bigDipperImage.transform.localRotation = originalRotation;
    }

    IEnumerator AnimateInfoText()
    {
        infoText.SetActive(true);
        
        CanvasGroup textGroup = infoText.GetComponent<CanvasGroup>();
        if (textGroup == null) textGroup = infoText.AddComponent<CanvasGroup>();
        textGroup.alpha = 0;
        
        Vector3 textOriginalPos = infoText.transform.localPosition;
        Vector3 textStartPos = textOriginalPos;
        
        if (textSlideUp)
        {
            textStartPos = new Vector3(textOriginalPos.x, textOriginalPos.y - textSlideDistance, textOriginalPos.z);
            infoText.transform.localPosition = textStartPos;
        }
        
        float elapsed = 0f;
        
        while (elapsed < textFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / textFadeDuration);
            
            // Gentle ease out
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);
            smoothT = Mathf.Clamp01(smoothT);
            
            textGroup.alpha = smoothT;
            
            if (textSlideUp)
            {
                Vector3 newPos = Vector3.Lerp(textStartPos, textOriginalPos, smoothT);
                if (!float.IsNaN(newPos.x) && !float.IsNaN(newPos.y) && !float.IsNaN(newPos.z))
                {
                    infoText.transform.localPosition = newPos;
                }
            }
            
            yield return null;
        }
        
        textGroup.alpha = 1f;
        infoText.transform.localPosition = textOriginalPos;
    }
}