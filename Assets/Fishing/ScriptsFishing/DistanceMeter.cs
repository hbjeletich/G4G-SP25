using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace Fishing
{
    public class DistanceMeter : MonoBehaviour
    {
        [Header("Distance Meter Settings")]
        public RectTransform marker;
        public float barSpeed = 0.5f;
        [SerializeField] private float markerSmoothSpeed = 5f;
        public CanvasGroup canvasGroup;
        public float fadeDuration = 0.5f;

        [Header("Zone Thresholds (0 to 1)")]
        [SerializeField] private float closestThreshold = 0.33f;
        [SerializeField] private float middleThreshold = 0.66f;

        [Header("Text UI Settings")]
        public TextMeshProUGUI distanceText;
        public int maxDistance = 50;

        [HideInInspector] public bool isFishing = false;
        [HideInInspector] public FishingPlayerController.FishingZone currentZone = FishingPlayerController.FishingZone.Closest;

        private float timeCounter = 0f;
        private float smoothedValue = 0f;
        private bool meterActive = false;

        private PlayerInput playerInput;
        private InputAction leftHipAction, rightHipAction;

        private void Awake()
        {
            playerInput = FindObjectOfType<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogError("PlayerInput component not found in the scene.");
                return;
            }

            // fishAction = playerInput.actions["Fish"];
            leftHipAction = playerInput.actions["LeftHipAbducted"];
            rightHipAction = playerInput.actions["RightHipAbducted"];
            isFishing = true;
        }

        private void OnEnable()
        {
            // moveAction.Enable();
            // fishAction.Enable();
            leftHipAction.Enable();
            rightHipAction.Enable();
        }

        private void OnDisable()
        {
            // moveAction.Disable();
            // fishAction.Disable();
            leftHipAction.Disable();
            rightHipAction.Disable();
        }

        private void Update()
        {
            if (!isFishing) return;

            // Drive ping-pong animation
            timeCounter += Time.deltaTime * barSpeed;
            float visualValue = Mathf.PingPong(timeCounter, 1f);
            smoothedValue = Mathf.Lerp(smoothedValue, visualValue, Time.deltaTime * markerSmoothSpeed);

            UpdateMarkerPosition();
            UpdateDistanceText();
        }

        private void UpdateMarkerPosition()
        {
            if (marker == null) return;

            RectTransform parent = marker.transform.parent.GetComponent<RectTransform>();
            float width = parent.rect.width;

            Vector2 newPos = marker.anchoredPosition;
            newPos.x = smoothedValue * width;
            marker.anchoredPosition = newPos;
        }

        private void UpdateDistanceText()
        {
            if (distanceText == null) return;

            float distance = smoothedValue * maxDistance;

            // Round to nearest multiple of 10
            int snappedDistance = Mathf.RoundToInt(distance / 10f) * 10;

            // Clamp it so it never exceeds maxDistance
            snappedDistance = Mathf.Clamp(snappedDistance, 0, maxDistance);

            distanceText.text = $"{snappedDistance} m";
        }


        private void OnFishPressed(InputAction.CallbackContext context)
        {
            var playerController = FindObjectOfType<FishingPlayerController>();
            if (playerController != null && playerController.isFishingInProgress)
            {
                return;  // Ignore if fishing is already in progress
            }

            if (!meterActive)
            {
                meterActive = true;
                RestartMeter();
            }
            else
            {
                isFishing = true;
            }
        }

        private void OnFishReleased(InputAction.CallbackContext context)
        {
            if (!meterActive || !isFishing)
            {
                Debug.Log("FishReleased ignored: meter not active or not fishing.");
                return;
            }

            if (smoothedValue < closestThreshold)
            {
                currentZone = FishingPlayerController.FishingZone.Closest;
            }
            else if (smoothedValue < middleThreshold)
            {
                currentZone = FishingPlayerController.FishingZone.Middle;
            }
            else
            {
                currentZone = FishingPlayerController.FishingZone.Farthest;
            }
            Debug.Log($"OnFishReleased: smoothedValue = {smoothedValue}, assigned zone = {currentZone}");

            isFishing = false;
            meterActive = false;

            var playerController = FindObjectOfType<FishingPlayerController>();
            if (playerController != null)
            {
                playerController.CastHook();
            }
        }

        public void RestartMeter()
        {
            isFishing = true;
            Debug.Log("Restarting meter.");
        }

        public IEnumerator FadeCanvas(float targetAlpha)
        {
            float startAlpha = canvasGroup.alpha;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float t = timer / fadeDuration;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                canvasGroup.interactable = targetAlpha > 0.5f;
                canvasGroup.blocksRaycasts = targetAlpha > 0.5f;
                yield return null;
            }
            canvasGroup.alpha = targetAlpha;
        }
    }
}
