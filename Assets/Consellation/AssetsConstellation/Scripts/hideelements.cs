using UnityEngine;
using UnityEngine.UI;

public class HideElementsOnClick : MonoBehaviour
{
    [Header("Drag Elements to Hide")]
    public GameObject bigDipperImage;
    public GameObject infoText;
    
    [Header("Optional: Add More Elements")]
    public GameObject[] additionalElementsToHide;

    [Header("Fade Out Settings (Optional)")]
    public bool useFadeOut = false;
    public float fadeOutDuration = 0.3f;

    void Start()
    {
        // Get the button component on this GameObject
        Button button = GetComponent<Button>();
        
        if (button != null)
        {
            // Add listener to button click
            button.onClick.AddListener(HideElements);
        }
        else
        {
            Debug.LogWarning("No Button component found on " + gameObject.name);
        }
    }

    public void HideElements()
    {
        if (useFadeOut)
        {
            // Fade out before hiding
            if (bigDipperImage != null)
                StartCoroutine(FadeOutAndHide(bigDipperImage));
            
            if (infoText != null)
                StartCoroutine(FadeOutAndHide(infoText));
            
            foreach (GameObject obj in additionalElementsToHide)
            {
                if (obj != null)
                    StartCoroutine(FadeOutAndHide(obj));
            }
        }
        else
        {
            // Instant hide
            if (bigDipperImage != null)
                bigDipperImage.SetActive(false);
            
            if (infoText != null)
                infoText.SetActive(false);
            
            foreach (GameObject obj in additionalElementsToHide)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    System.Collections.IEnumerator FadeOutAndHide(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = obj.AddComponent<CanvasGroup>();
        
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        obj.SetActive(false);
    }
}