using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

        [HideInInspector] public bool isFishing = false;
        [HideInInspector] public FishingPlayerController.FishingZone currentZone = FishingPlayerController.FishingZone.Closest;

        private float timeCounter = 0f;
        private float smoothedValue = 0f;
        private bool meterActive = false;

        private PlayerInput playerInput;
        private InputAction fishAction;

        private void Awake()
        {
            playerInput = FindObjectOfType<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogError("PlayerInput component not found in the scene.");
                return;
            }

            fishAction = playerInput.actions["Fish"];
            isFishing = true;
        }

        private void OnEnable()
        {
            if (fishAction != null)
            {
                fishAction.Enable();
                fishAction.performed += OnFishPressed;
                fishAction.canceled += OnFishReleased;
            }
        }

        private void OnDisable()
        {
            if (fishAction != null)
            {
                fishAction.Disable();
                fishAction.performed -= OnFishPressed;
                fishAction.canceled -= OnFishReleased;
            }
        }

        private void Update()
        {
            if (!isFishing) return;

            // Drive ping-pong animation
            timeCounter += Time.deltaTime * barSpeed;
            float visualValue = Mathf.PingPong(timeCounter, 1f);
            smoothedValue = Mathf.Lerp(smoothedValue, visualValue, Time.deltaTime * markerSmoothSpeed);
            UpdateMarkerPosition();
        }

        private void UpdateMarkerPosition()
        {
            if (marker == null) return;

            // We'll assume the marker's parent is properly sized
            RectTransform parent = marker.transform.parent.GetComponent<RectTransform>();
            float width = parent.rect.width;

            Vector2 newPos = marker.anchoredPosition;
            newPos.x = smoothedValue * width;
            marker.anchoredPosition = newPos;
        }

        private void OnFishPressed(InputAction.CallbackContext context)
        {
            var playerController = FindObjectOfType<FishingPlayerController>();
            if (playerController != null && playerController.isFishingInProgress)
            {
                Debug.Log("Input ignored: fishing in progress.");
                return;
            }

            if (!meterActive)
            {
                meterActive = true;
                RestartMeter();
                Debug.Log("Meter started.");
            }
            else
            {
                isFishing = true;
                Debug.Log("Started fishing action (charging).");
            }
        }

        private void OnFishReleased(InputAction.CallbackContext context)
        {
            if (!meterActive || !isFishing)
            {
                Debug.Log("FishReleased ignored: meter not active or not fishing.");
                return;
            }

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
            timeCounter = 0f;
            smoothedValue = 0f;
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            switch (other.tag)
            {
                case "ClosestTrigger":
                    currentZone = FishingPlayerController.FishingZone.Closest;
                    break;
                case "MiddleTrigger":
                    currentZone = FishingPlayerController.FishingZone.Middle;
                    break;
                case "FarthestTrigger":
                    currentZone = FishingPlayerController.FishingZone.Farthest;
                    break;
            }
            Debug.Log("Entered zone: " + currentZone);
        }
    }
}
