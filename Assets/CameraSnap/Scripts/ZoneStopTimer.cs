using System.Collections.Generic;
using UnityEngine;

namespace CameraSnap
{
    
    // Handles the stop timeout for a slowdown zone.
    // Responsibilities:
    // Track a CartController while stopped and advance a timer
    // Reset timer when a new (unique) animal is captured
    // Update UIManager countdown UI
    // Invoke OnTimeout when the timer expires
    
    public class ZoneStopTimer : MonoBehaviour
    {
        [Tooltip("Seconds allowed while stopped before the zone auto-resumes the cart")]
        public float stopTimeout = 20f;

        // runtime
        private float elapsed = 0f;
        private CartController trackedCart;
        private HashSet<string> capturedDuringStop = new HashSet<string>();

        // event fired when timeout occurs (passes the cart that timed out)
        public event System.Action<CartController> OnTimeout;

        public bool IsTracking => trackedCart != null;

        public void StartTracking(CartController cart)
        {
            trackedCart = cart;
            elapsed = 0f;
            capturedDuringStop.Clear();
            // show initial UI
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateStopCountdown(stopTimeout, stopTimeout);
        }

        public void StopTracking()
        {
            trackedCart = null;
            elapsed = 0f;
            capturedDuringStop.Clear();
            if (UIManager.Instance != null)
                UIManager.Instance.HideStopCountdown();
        }

        
        // Notify the timer that an animal was captured while stopped in this zone.
        // Returns true if the animal was new for this stop period.
        
        public bool NotifyCaptured(string animalName)
        {
            if (string.IsNullOrEmpty(animalName)) return false;
            bool isNew = capturedDuringStop.Add(animalName);
            if (isNew)
                elapsed = 0f; // reset timer on new capture

            return isNew;
        }

        void Update()
        {
            if (trackedCart != null && trackedCart.currentZone != null && trackedCart.IsStopped())
            {
                elapsed += Time.deltaTime;
                float remaining = Mathf.Max(0f, stopTimeout - elapsed);
                if (UIManager.Instance != null)
                    UIManager.Instance.UpdateStopCountdown(remaining, stopTimeout);

                if (elapsed >= stopTimeout)
                {
                    // Fire timeout and stop tracking (zone or owner will handle cart resuming and cleanup)
                    OnTimeout?.Invoke(trackedCart);
                    StopTracking();
                }
            }
            else
            {
                // not stopped, ensure UI is hidden and elapsed cleared
                if (elapsed != 0f)
                {
                    elapsed = 0f;
                    if (UIManager.Instance != null)
                        UIManager.Instance.HideStopCountdown();
                }
            }
        }
    }
}
