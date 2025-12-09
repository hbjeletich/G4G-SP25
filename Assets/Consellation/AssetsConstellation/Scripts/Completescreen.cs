using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Constellation
{
    public class StarPlacementManager : MonoBehaviour
    {
        //Declaration Area

        [Header("UI")]
        //the panel holding all celebration elements
        [SerializeField] private GameObject congratsPanel;

        [Header("3D Character - Slides from below into lower left")]
        //character model that slides up on completion
        [SerializeField] private GameObject character3DModel;
        //where character ends up after slide
        [SerializeField] private Vector3 targetPosition = new Vector3(-3f, 1f, 5f);
        //how far below target to start
        [SerializeField] private float slideUpDistance = 10f;
        [SerializeField] private float slideDuration = 0.8f;
        //controls the feel of the slide
        [SerializeField] private AnimationCurve slideEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("UI Elements")]
        //main congratulations message
        [SerializeField] private TextMeshProUGUI dialogueText;
        //how long dialogue shows before transitioning
        [SerializeField] private float dialogueHoldTime = 2f;
        //dialogue pops from this scale to 1.0
        [SerializeField] private float dialogueStartScale = 0.5f;
        
        //canvasgroup for batch alpha control + input blocking
        [SerializeField] private CanvasGroup secondaryObject;
        
        [SerializeField] private Image popupImage;
        //assigned at runtime so levels can use different images
        [SerializeField] private Sprite imageSprite;
        
        [SerializeField] private TextMeshProUGUI finalText;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        [Header("Audio")]
        [SerializeField] private AudioSource completionSound;

        //RUNTIME STATE

        //cached array of all stars, polled each frame for completion
        private StarScript[] stars;
        //prevents completion from firing multiple times
        private bool levelCompleted = false;

        void Start()
        {
            //hide celebration UI until needed
            if (congratsPanel != null)
                congratsPanel.SetActive(false);

            if (character3DModel != null)
                character3DModel.SetActive(false);

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

            //true = include inactive, designers might disable stars initially
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

            //count placed stars - each StarScript sets foundHome via player interaction
            int placedCount = 0;

            foreach (StarScript star in stars)
            {
                if (star == null) continue;
                if (star.foundHome)
                    placedCount++;
            }

            //press P to check progress
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

        //handles the celebration animation sequence
        private IEnumerator PlaySequence()
        {
            //character starts below target, slides up
            Vector3 startPos = targetPosition - new Vector3(0, slideUpDistance, 0);
            
            if (character3DModel != null)
            {
                character3DModel.transform.position = startPos;
                character3DModel.SetActive(true);
            }

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
            
            // 2. Image fades to 50% (background accent, shouldnt overpower text)
            if (popupImage != null)
            {
                float elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    elapsed += Time.deltaTime;
                    SetAlpha(popupImage, (elapsed / 0.5f) * 0.5f);
                    yield return null;
                }
                SetAlpha(popupImage, 0.5f);
            }
            
            // 3. Dialogue fades in + grows
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
            
            // 4. Hold dialogue
            yield return new WaitForSeconds(dialogueHoldTime);
            
            // 5. Dialogue fades out + shrinks
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
            
            // 6. Final text fades in
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
            
            // 7. Secondary fades in, enable input only now so player cant click invisible buttons
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

        //sets transparency on any UI graphic
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