using UnityEngine;

namespace CameraSnap
{
    public class CameraMode : MonoBehaviour
    {
        public Animator playerAnimator;
        public GameObject cameraOverlayUI;
        public KeyCode toggleKey = KeyCode.C;
        public KeyCode photoKey = KeyCode.Mouse0;

        private bool inCameraMode = false;

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleCameraMode();
            }

            if (inCameraMode && Input.GetKeyDown(photoKey))
            {
                TakePhoto();
            }
        }

        void ToggleCameraMode()
        {
            inCameraMode = !inCameraMode;

            // Play animation
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("IsHoldingCamera", inCameraMode);
            }

            // Show/hide UI overlay
            if (cameraOverlayUI != null)
            {
                cameraOverlayUI.SetActive(inCameraMode);
            }

            
        }

        void TakePhoto()
        {
            Debug.Log("[CameraMode] Taking photo...");

            // wip
        }

        public bool IsInCameraMode()
        {
            return inCameraMode;
        }
    }
}
