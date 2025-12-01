using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CameraSnap
{
    
    // Centralized UI manager. Exposes simple methods for showing/hiding UI and
    // displaying short messages. Intended to be a single source of truth for
    // UI manipulations so other game scripts don't directly toggle UI objects.
    
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("End Game")]
        public GameObject endGamePanel;


        [Header("Camera Overlay")]
        public GameObject cameraOverlayUI;
        public Image overlayImage;
        public Color normalColor = Color.white;
        public Color readyColor = Color.green;

        [Header("Photo Message")]
        public TMP_Text photoText;
        public float defaultMessageDuration = 2f;

    [Header("Stop Countdown")]
    [Tooltip("Screen UI text that shows remaining time until auto-resume")]
    public TMP_Text stopCountdownText;

    [Header("Find Animal UI")]
    [Tooltip("Image slots (left->right) used by the in-game 'find the animal' system; shows silhouettes and reveals found images.")]
    public List<UnityEngine.UI.Image> targetSlots = new List<UnityEngine.UI.Image>(3);

    // Mapping from animal name -> slot index for quick reveal
    private System.Collections.Generic.Dictionary<string, int> targetIndexByName = new System.Collections.Generic.Dictionary<string, int>();

    [Header("End Panel")]
    [Tooltip("Image slots used exclusively on the end-of-run panel. These are populated with all animals (silhouettes by default) and reveal when found.")]
    public List<UnityEngine.UI.Image> endPanelSlots = new List<UnityEngine.UI.Image>();

    // Mapping for end panel (name -> index)
    private System.Collections.Generic.Dictionary<string, int> endPanelIndexByName = new System.Collections.Generic.Dictionary<string, int>();

        [Header("Misc UI")]
        public GameObject stopCartObject;
        public GameObject zoneIcon;

        [Header("Player Guide")]
        [Tooltip("UI GameObject that contains the guide image and animator")]
        public GameObject guideObject;
        public Animator guideAnimator;
        [Tooltip("Animator integer parameter name used to switch guide states (1=Squat,2=Hip,3=Foot)")]
        public string guideStateParam = "State";

        public enum GuideState
        {
            WeightShift = 0,
            Squat = 1,
            HipAbduction = 2,
            FootRaise = 3
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            // Initialize session targets from GameManager if available
            if (GameManager.Instance != null)
            {
                var targets = GameManager.Instance.GetRandomTargets(targetSlots.Count);
                SetSessionTargets(targets);
            }
        }

        

        public void SetStopCartVisible(bool visible)
        {
            if (stopCartObject != null) stopCartObject.SetActive(visible);
        }

       
       
        public void SetZoneIcon(GameObject icon)
        {
            zoneIcon = icon;
        }

        public void SetZoneIconVisible(bool visible)
        {
            if (zoneIcon != null) zoneIcon.SetActive(visible);
        }

        public void SetOverlayActive(bool active)
        {
            if (cameraOverlayUI != null) cameraOverlayUI.SetActive(active);
        }

        public void SetOverlayReady(bool ready)
        {
            if (overlayImage != null) overlayImage.color = ready ? readyColor : normalColor;
        }

        // Guide control
        public void SetGuideState(GuideState state)
        {
            if (guideObject == null || guideAnimator == null)
            {
                return;
            }

            guideObject.SetActive(true);
            // Set integer parameter (Animator should handle transitions)
            guideAnimator.SetInteger(guideStateParam, (int)state);
        }

      
        /// Set the silhouettes for the current play session. The provided list is
        /// assigned to `targetSlots` left-to-right. If there are fewer targets than
        /// slots, the remaining slots are cleared.
     
        public void SetSessionTargets(System.Collections.Generic.List<AnimalData> targets)
        {
            targetIndexByName.Clear();

            for (int i = 0; i < targetSlots.Count; i++)
            {
                var slot = targetSlots[i];
                if (slot == null) continue;

                if (targets != null && i < targets.Count && targets[i] != null)
                {
                    var data = targets[i];
                    // show silhouette image (fall back to found image if silhouette missing)
                    slot.sprite = data.silhouetteImage != null ? data.silhouetteImage : data.foundImage;
                    slot.color = Color.white;
                    slot.enabled = true;
                    if (!string.IsNullOrEmpty(data.animalName))
                        targetIndexByName[data.animalName] = i;
                }
                else
                {
                    // clear slot when no assigned target
                    slot.sprite = null;
                    slot.enabled = false;
                }
            }
        }

        
        public void SetEndPanelAnimals(System.Collections.Generic.List<AnimalData> animals)
        {
            endPanelIndexByName.Clear();

            for (int i = 0; i < endPanelSlots.Count; i++)
            {
                var slot = endPanelSlots[i];
                if (slot == null) continue;

                if (animals != null && i < animals.Count && animals[i] != null)
                {
                    var data = animals[i];
                    slot.sprite = data.silhouetteImage != null ? data.silhouetteImage : data.foundImage;
                    slot.color = Color.white;
                    slot.enabled = true;
                    if (!string.IsNullOrEmpty(data.animalName))
                        endPanelIndexByName[data.animalName] = i;
                }
                else
                {
                    slot.sprite = null;
                    slot.enabled = false;
                }
            }
        }

    
        /// Reveal the matched target slot for the given animal name (swap silhouette
        /// to the found image). Safe to call even if the animal isn't a target.
     
        public void RevealTarget(string animalName)
        {
            if (string.IsNullOrEmpty(animalName)) return;
            if (!targetIndexByName.TryGetValue(animalName, out int idx)) return;
            if (idx < 0 || idx >= targetSlots.Count) return;

            var slot = targetSlots[idx];
            if (slot == null) return;

            // Find the AnimalData to get its revealed image
            var data = GameManager.Instance?.GetAllAnimals().Find(a => a != null && a.animalName == animalName);
            if (data == null)
            {
                return;
            }

            if (data.foundImage != null)
            {
                slot.sprite = data.foundImage;
                slot.color = Color.white;
            }
            else
            {
                // fallback: keep silhouette but tint green to indicate found
                slot.color = Color.green;
            }
        }

        
        public void RevealEndPanelTarget(string animalName)
        {
            if (string.IsNullOrEmpty(animalName)) return;
            if (!endPanelIndexByName.TryGetValue(animalName, out int idx)) return;
            if (idx < 0 || idx >= endPanelSlots.Count) return;

            var slot = endPanelSlots[idx];
            if (slot == null) return;

            var data = GameManager.Instance?.GetAllAnimals().Find(a => a != null && a.animalName == animalName);
            if (data == null) return;

            if (data.foundImage != null)
            {
                slot.sprite = data.foundImage;
                slot.color = Color.white;
            }
            else
            {
                slot.color = Color.green;
            }
        }

       
        public void ShowEndPanel()
        {
            if (endGamePanel != null) endGamePanel.SetActive(true);
        }

        public void HideEndPanel()
        {
            if (endGamePanel != null) endGamePanel.SetActive(false);
        }

        public void HideGuide()
        {
            if (guideObject == null) return;
            guideObject.SetActive(false);
        }

        public void SetOverlayColor(Color c)
        {
            if (overlayImage != null) overlayImage.color = c;
        }

        public void ShowPhotoMessage(string animalName, float duration = -1f)
        {
            if (photoText == null) return;
            if (duration <= 0f) duration = defaultMessageDuration;
            StartCoroutine(PhotoMessageRoutine(animalName, duration));
        }

        
        public void UpdateStopCountdown(float remaining, float total)
        {
            if (stopCountdownText == null) return;

            if (remaining <= 0f)
            {
                HideStopCountdown();
                return;
            }

            // Ensure visible and update text
            stopCountdownText.gameObject.SetActive(true);
            stopCountdownText.text = $"{Mathf.CeilToInt(remaining)}";
        }

        public void HideStopCountdown()
        {
            if (stopCountdownText != null) stopCountdownText.gameObject.SetActive(false);
        }

        IEnumerator PhotoMessageRoutine(string animalName, float duration)
        {
            photoText.text = $"<b>Captured:</b> {animalName}";
            photoText.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            photoText.gameObject.SetActive(false);
            photoText.text = string.Empty;
        }
    }
}
