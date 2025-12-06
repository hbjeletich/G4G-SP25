using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private float dialogueHoldTime = 2f;
        
        [SerializeField] private Image popupImage;
        [SerializeField] private Sprite imageSprite;
        
        [SerializeField] private TextMeshProUGUI finalText;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        // ⭐ ADDED — just this
        [Header("Audio")]
        [SerializeField] private AudioSource completionSound;

        private StarScript[] stars;
        private bool levelCompleted = false;

        void Start()
        {
            if (congratsPanel != null)
                congratsPanel.SetActive(false);

            if (character3DModel != null)
                character3DModel.SetActive(false);

            // Hide UI elements initially
            if (dialogueText != null)
                SetAlpha(dialogueText, 0);

            if (popupImage != null)
                SetAlpha(popupImage, 0);

            if (finalText != null)
                SetAlpha(finalText, 0);

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

            // ⭐ ADDED — plays the sound
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

            // Setup UI elements - make sure they're active and set alpha to 0
            if (dialogueText != null)
            {
                dialogueText.gameObject.SetActive(true);
                SetAlpha(dialogueText, 0);
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

            // 1. Character slides UP from below
            if (character3DModel != null)
            {
                float elapsed = 0f;
                while (elapsed < slideDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / slideDuration;
                    character3DModel.transform.position = Vector3.Lerp(startPos, targetPosition, t);
                    yield return null;
                }
                character3DModel.transform.position = targetPosition;
            }
            
            // 2. Image fades in first (to 50% opacity)
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
            
            // 3. Dialogue fades in
            if (dialogueText != null)
            {
                float elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    elapsed += Time.deltaTime;
                    SetAlpha(dialogueText, elapsed / 0.5f);
                    yield return null;
                }
                SetAlpha(dialogueText, 1);
            }
            
            // 4. Hold dialogue
            yield return new WaitForSeconds(dialogueHoldTime);
            
            // 5. Dialogue fades out
            if (dialogueText != null)
            {
                float elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    elapsed += Time.deltaTime;
                    SetAlpha(dialogueText, 1 - (elapsed / 0.5f));
                    yield return null;
                }
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
