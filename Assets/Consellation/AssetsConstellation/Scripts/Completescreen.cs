using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

namespace Constellation
{
    public class StarPlacementManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject congratsPanel;

        [Header("3D Character - Slides from below into lower left")]
        [SerializeField] private GameObject character3DModel;
        [SerializeField] private Vector3 targetPosition = new Vector3(-3f, 1f, 5f);
        [SerializeField] private float slideUpDistance = 10f;
        [SerializeField] private float slideDuration = 0.8f;
        [SerializeField] private AnimationCurve slideEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Outline Video (Quad with Chromakey + VideoPlayer)")]
        [SerializeField] private GameObject outlineQuad;
        [SerializeField] private float outlineHoldTime = 6f;
        [SerializeField] private float crossfadeDuration = 1f;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private float dialogueHoldTime = 2f;
        [SerializeField] private float dialogueStartScale = 0.5f;
        
        [SerializeField] private CanvasGroup secondaryObject;
        
        [SerializeField] private Image popupImage;
        [SerializeField] private Sprite imageSprite;
        [SerializeField] private float popupFadeDuration = 0.5f; 
        
        [SerializeField] private TextMeshProUGUI finalText;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        [Header("Audio")]
        [SerializeField] private AudioSource completionSound;

        private StarScript[] stars;
        private bool levelCompleted = false;
        private VideoPlayer outlineVideoPlayer;

        void Start()
        {
            if (congratsPanel != null)
                congratsPanel.SetActive(false);

            if (character3DModel != null)
                character3DModel.SetActive(false);

            // Grab the VideoPlayer from the quad and hide it
            if (outlineQuad != null)
            {
                outlineVideoPlayer = outlineQuad.GetComponent<VideoPlayer>();
                if (outlineVideoPlayer != null)
                {
                    outlineVideoPlayer.playOnAwake = false;
                    outlineVideoPlayer.isLooping = false;
                }
                outlineQuad.SetActive(false);
            }

            // Hide UI elements initially — alpha only, don't touch scale
            if (dialogueText != null)
                SetAlpha(dialogueText, 0);

            if (popupImage != null)
                SetAlpha(popupImage, 0);

            if (finalText != null)
                SetAlpha(finalText, 0);

            if (secondaryObject != null)
            {
                secondaryObject.alpha = 0;
            }

            stars = FindObjectsOfType<StarScript>(true);
            if (stars.Length == 0)
            {
                Debug.LogError("No StarScript objects found in the scene!");
                return;
            }

            if (debugMode)
                Debug.Log($"Tracking {stars.Length} stars for placement progress.");
        }

        void Update()
        {
            if (levelCompleted || stars == null || stars.Length == 0)
                return;

            int placedCount = 0;

            foreach (StarScript star in stars)
            {
                if (star == null) continue;
                if (star.foundHome)
                    placedCount++;
            }

            if (debugMode && Input.GetKeyDown(KeyCode.P))
                Debug.Log($"Progress: {placedCount}/{stars.Length} stars placed");

            if (placedCount == stars.Length)
                OnLevelComplete();
        }

        private void OnLevelComplete()
        {
            if (congratsPanel == null)
            {
                Debug.LogError("❌ Congrats panel is NOT assigned in the inspector!");
                return;
            }
            else
            {
                Debug.Log($"✅ Congrats panel found: {congratsPanel.name}, activeSelf={congratsPanel.activeSelf}, inHierarchy={congratsPanel.activeInHierarchy}");
            }

            levelCompleted = true;

            if (completionSound != null)
                completionSound.Play();

            Debug.Log("🌟 All stars have found their homes!");

            if (congratsPanel != null)
            {
                congratsPanel.SetActive(true);
                StartCoroutine(PlaySequence());
            }
        }

        private IEnumerator PlaySequence()
        {
            // Setup character - starts below, ends at target position
            Vector3 startPos = targetPosition - new Vector3(0, slideUpDistance, 0);
            
            if (character3DModel != null)
            {
                character3DModel.transform.position = startPos;
                character3DModel.SetActive(true);
            }

            // Setup UI elements - active but invisible (alpha 0, normal scale)
            if (dialogueText != null)
            {
                dialogueText.gameObject.SetActive(true);
                SetAlpha(dialogueText, 0);
                dialogueText.transform.localScale = new Vector3(dialogueStartScale, dialogueStartScale, dialogueStartScale);
            }

            if (popupImage != null)
            {
                popupImage.gameObject.SetActive(true);
                if (imageSprite != null)
                    popupImage.sprite = imageSprite;
                SetAlpha(popupImage, 0);
            }

            if (finalText != null)
            {
                finalText.gameObject.SetActive(true);
                SetAlpha(finalText, 0);
            }

            if (secondaryObject != null)
            {
                secondaryObject.gameObject.SetActive(true);
                secondaryObject.alpha = 0;
            }

            // 1. Character slides UP from below
            if (character3DModel != null)
            {
                float elapsed = 0f;
                while (elapsed < slideDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = slideEaseCurve.Evaluate(elapsed / slideDuration);
                    character3DModel.transform.position = Vector3.Lerp(startPos, targetPosition, t);
                    yield return null;
                }
                character3DModel.transform.position = targetPosition;
            }
            
            // 2. Outline video: prepare, play, hold, then crossfade into constellation image
            if (outlineQuad != null && outlineVideoPlayer != null)
            {
                var renderer = outlineQuad.GetComponent<MeshRenderer>();
                if (renderer != null)
                    renderer.enabled = false;

                outlineQuad.SetActive(true);

                Vector3 quadOriginalScale = outlineQuad.transform.localScale;

                // Get the material and save original _TintColor
                Material quadMat = renderer != null ? renderer.material : null;
                Color originalTint = Color.white;
                if (quadMat != null && quadMat.HasProperty("_TintColor"))
                    originalTint = quadMat.GetColor("_TintColor");

                // Prepare the video while invisible
                outlineVideoPlayer.Prepare();
                while (!outlineVideoPlayer.isPrepared)
                    yield return null;

                if (debugMode)
                    Debug.Log("▶ Outline video prepared and playing");

                // First frame is ready — show the renderer
                if (renderer != null)
                    renderer.enabled = true;

                outlineVideoPlayer.Play();

                // Hold for the specified time
                yield return new WaitForSeconds(outlineHoldTime);

                if (debugMode)
                    Debug.Log("🔄 Starting crossfade");

                // 3. Crossfade: quad fades + shrinks while image grows + fades in
                Vector3 imageFullScale = popupImage != null ? popupImage.transform.localScale : Vector3.one;
                if (popupImage != null)
                    popupImage.transform.localScale = imageFullScale * 0.8f;

                float elapsed = 0f;
                while (elapsed < crossfadeDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / crossfadeDuration);
                    float eased = t * t * (3f - 2f * t);

                    // Quad: shrink from 100% to 70% and fade out via _TintColor alpha
                    outlineQuad.transform.localScale = quadOriginalScale * Mathf.Lerp(1f, 0.7f, eased);
                    if (quadMat != null && quadMat.HasProperty("_TintColor"))
                    {
                        Color tint = originalTint;
                        tint.a = Mathf.Lerp(1f, 0f, eased);
                        quadMat.SetColor("_TintColor", tint);
                    }

                    // Image: grow from 80% to 100% and fade in to 50%
                    if (popupImage != null)
                    {
                        popupImage.transform.localScale = imageFullScale * Mathf.Lerp(0.8f, 1f, eased);
                        SetAlpha(popupImage, eased * 0.5f);
                    }

                    yield return null;
                }

                // Finalize
                if (popupImage != null)
                {
                    popupImage.transform.localScale = imageFullScale;
                    SetAlpha(popupImage, 0.5f);
                }

                outlineVideoPlayer.Stop();

                // Restore quad to original state
                outlineQuad.transform.localScale = quadOriginalScale;
                if (quadMat != null && quadMat.HasProperty("_TintColor"))
                    quadMat.SetColor("_TintColor", originalTint);
                outlineQuad.SetActive(false);
            }
            else
            {
                // Fallback if no outline video — just fade in the image normally
                if (popupImage != null)
                {
                    float elapsed = 0f;
                    while (elapsed < popupFadeDuration)
                    {
                        elapsed += Time.deltaTime;
                        SetAlpha(popupImage, (elapsed / popupFadeDuration) * 0.5f);
                        yield return null;
                    }
                    SetAlpha(popupImage, 0.5f);
                }
            }
            
            // 4. Dialogue fades in + grows
            if (dialogueText != null)
            {
                float elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / 0.5f;
                    SetAlpha(dialogueText, t);
                    float scale = Mathf.Lerp(dialogueStartScale, 1f, t);
                    dialogueText.transform.localScale = new Vector3(scale, scale, scale);
                    yield return null;
                }
                SetAlpha(dialogueText, 1);
                dialogueText.transform.localScale = Vector3.one;
            }
            
            // 5. Hold dialogue
            yield return new WaitForSeconds(dialogueHoldTime);
            
            // 6. Dialogue fades out + shrinks
            {
                float elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / 0.5f;
                    float scale = Mathf.Lerp(1f, dialogueStartScale, t);
                    
                    if (dialogueText != null)
                    {
                        SetAlpha(dialogueText, 1 - t);
                        dialogueText.transform.localScale = new Vector3(scale, scale, scale);
                    }
                    yield return null;
                }
                if (dialogueText != null)
                    SetAlpha(dialogueText, 0);
            }
            
            // 7. Final text fades in
            if (finalText != null)
            {
                float elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    elapsed += Time.deltaTime;
                    SetAlpha(finalText, elapsed / 0.5f);
                    yield return null;
                }
                SetAlpha(finalText, 1);
            }
            
            // 8. Secondary object fades in last
            if (secondaryObject != null)
            {
                secondaryObject.interactable = true;
                secondaryObject.blocksRaycasts = true;
                float elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    elapsed += Time.deltaTime;
                    secondaryObject.alpha = elapsed / 0.5f;
                    yield return null;
                }
                secondaryObject.alpha = 1;
            }
        }

        private void SetAlpha(Graphic graphic, float alpha)
        {
            Color c = graphic.color;
            c.a = alpha;
            graphic.color = c;
        }

        [ContextMenu("Force Complete Level")]
        private void ForceComplete()
        {
            OnLevelComplete();
        }
    }
}