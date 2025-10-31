using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CameraSnap
{
    /// <summary>
    /// Centralized UI manager. Exposes simple methods for showing/hiding UI and
    /// displaying short messages. Intended to be a single source of truth for
    /// UI manipulations so other game scripts don't directly toggle UI objects.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("End Game")]
        public GameObject endGamePanel;
        public TMP_Text endGameText;

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

            DontDestroyOnLoad(gameObject);
        }

        // Shows the end-game summary and pauses the game.
        public void ShowEndSummary(List<AnimalData> allAnimals, HashSet<string> captured)
        {
            if (endGamePanel == null || endGameText == null) return;

            StringBuilder summary = new StringBuilder();
            summary.AppendLine("<b> Photo Summary</b>\n");

            summary.AppendLine("<color=green><b>Captured:</b></color>");
            int capturedCount = 0;
            foreach (var animal in allAnimals)
            {
                if (captured.Contains(animal.animalName))
                {
                    summary.AppendLine($" {animal.animalName}");
                    capturedCount++;
                }
            }

            summary.AppendLine("\n<color=red><b>Missed:</b></color>");
            foreach (var animal in allAnimals)
            {
                if (!captured.Contains(animal.animalName))
                    summary.AppendLine($" {animal.animalName}");
            }

            summary.AppendLine($"\n<b>Total Captured:</b> {capturedCount}/{allAnimals.Count}");
            summary.AppendLine("<b> End game. Press R or raise foot to restart</b>\n");

            endGameText.text = summary.ToString();
            endGamePanel.SetActive(true);
            Time.timeScale = 0f;
        }

        public void SetStopCartVisible(bool visible)
        {
            if (stopCartObject != null) stopCartObject.SetActive(visible);
        }

        /// <summary>
        /// Zone icon is often a per-zone GameObject; UIManager allows a single visible
        /// slot so scripts can pass ownership when a zone becomes active.
        /// </summary>
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
                Debug.LogError("[UIManager] Guide UI or Animator not assigned.");
                return;
            }

            guideObject.SetActive(true);
            // Set integer parameter (Animator should handle transitions)
            guideAnimator.SetInteger(guideStateParam, (int)state);
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

        /// <summary>
        /// Update the stop countdown UI. remaining and total are in seconds.
        /// Pass remaining <= 0 to hide the countdown.
        /// </summary>
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
