using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public class Star
{
    public bool isFilled;
    public Image image;

    public Star(Image img)
    {
        image = img;
        isFilled = false;
    }
}

public class StarProgressBar : MonoBehaviour
{
    [Header("UI References")]
    public GameObject progressBarContainer;
    public Slider slider;
    public TextMeshProUGUI resultText;
    [Header("Star Settings")]
    public Sprite starFilledSprite;
    public Sprite starEmptySprite;
    public Star[] stars;
    public float[] starLengths;
    public string[] starTexts = new string[] { "GOOD!", "GREAT!", "AMAZING!", "PERFECT!" };
    [Header("Position Settings")]
    public Vector3 offscreenPosition;
    public Vector3 onscreenPosition;
    public float moveDuration = 0.5f;
    [Header("Sound Settings")]
    public AudioClip starSound;
    public float pitchIncrement = 0.1f;

    [Header("Animation Settings")]
    public float scaleOvershoot = 1.5f;
    public float wobbleAngle = 15f;
    public int wobbleCount = 3;

    private int currentStarIndex = 0;
    private float duration = 0.6f;

    private bool isMoving = false;
    private bool isActive = false;

    void Start()
    {
        resultText.text = "";
        progressBarContainer.transform.localPosition = offscreenPosition;
        ResetProgressBar();
    }
    
    public void ShowProgressBar()
    {
        if(isActive) return;
        isActive = true;

        Vector3 targetPos;
        if(progressBarContainer.transform.localPosition == offscreenPosition)
            targetPos = onscreenPosition;
        else
            targetPos = offscreenPosition;

        StartCoroutine(MoveProgressBar(targetPos));
    }

    public void HideProgressBar()
    {
        if(!isActive) return;
        isActive = false;
        StartCoroutine(MoveProgressBar(offscreenPosition));
        Invoke(nameof(ResetProgressBar), moveDuration);
    }

    private void ResetProgressBar()
    {
        slider.value = 0f;
        resultText.text = "";
        currentStarIndex = 0;

        foreach(Star star in stars)
        {
            star.isFilled = false;
            star.image.sprite = starEmptySprite;
        }
    } 

    public void UpdateProgress(float progress)
    {
        slider.value = progress;

        // Fill stars as we cross each threshold
        while (currentStarIndex < stars.Length 
            && currentStarIndex < starLengths.Length 
            && progress >= starLengths[currentStarIndex])
        {
            CompleteStar(currentStarIndex);
            UpdateTextForStar(currentStarIndex);
            currentStarIndex++;
        }

        // 4th text when fully complete
        if (progress >= 1f && currentStarIndex >= stars.Length && starTexts.Length > stars.Length)
        {
            resultText.text = starTexts[stars.Length];
            StartCoroutine(PulseText());
        }
    }

    public void CompleteStar(int index)
    {
        if(index < stars.Length)
        {
            if(starSound != null)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.pitch = 1f + currentStarIndex * pitchIncrement;
                audioSource.PlayOneShot(starSound);
            }

            stars[index].isFilled = true;
            stars[index].image.sprite = starFilledSprite;
            StartCoroutine(AnimateStar(stars[index].image.rectTransform));
        }
    }

    private void UpdateTextForStar(int index)
    {
        if(index < starTexts.Length)
        {
            resultText.text = starTexts[index];
        }

        StartCoroutine(PulseText());
    }

    private IEnumerator PulseText()
    {
        Vector3 originalScale = resultText.transform.localScale;
        float pulseDuration = 0.3f;
        float elapsed = 0f;

        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / pulseDuration);
            float scaleFactor = 1f + 0.5f * Mathf.Sin(t * Mathf.PI);
            resultText.transform.localScale = originalScale * scaleFactor;
            yield return null;
        }

        resultText.transform.localScale = originalScale;
    }

    private IEnumerator MoveProgressBar(Vector3 targetPos)
    {
        Vector3 startPos = progressBarContainer.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            progressBarContainer.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        progressBarContainer.transform.localPosition = targetPos;
    }

    private IEnumerator AnimateStar(RectTransform starTransform)
    {
        Vector3 originalScale = starTransform.localScale;
        Quaternion originalRotation = starTransform.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float scaleFactor;
            if (t < 0.25f)
            {
                float punchT = t / 0.25f;
                scaleFactor = Mathf.Lerp(1f, scaleOvershoot, 1f - (1f - punchT) * (1f - punchT));
            }
            else
            {
                float settleT = (t - 0.25f) / 0.75f;
                float damped = Mathf.Pow(1f - settleT, 2f);
                scaleFactor = 1f + (scaleOvershoot - 1f) * damped * Mathf.Cos(settleT * Mathf.PI * 2f);
            }
            starTransform.localScale = originalScale * scaleFactor;

            float wobbleDecay = 1f - t;
            float angle = wobbleAngle * wobbleDecay * Mathf.Sin(t * wobbleCount * 2f * Mathf.PI);
            starTransform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        starTransform.localScale = originalScale;
        starTransform.localRotation = originalRotation;
    }
}