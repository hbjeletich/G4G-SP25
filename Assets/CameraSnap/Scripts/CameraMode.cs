using UnityEngine;
using System.Collections;

namespace CameraSnap
{
    // This lets the player access camera mode. The player will open the camera and it will
    // indicate to the player that there is no animals detected until the player has an animal in view.
    // Note: If doing zoom in and out, maybe detection range will change depending on zoom.

    public class CameraMode : MonoBehaviour
    {
        [Header("Camera Settings")]
        public Animator playerAnimator;  //Animator for camera coming up screen
        public KeyCode toggleKey = KeyCode.C; //Key for entering camera mode
        public KeyCode photoKey = KeyCode.Mouse0; //Key for taking photo
        public float detectionRange = 100f; //How far camera mode can see the animal
        public LayerMask animalLayer; //Camera mode uses this to detect animal
        public AudioSource audioSource; //Camera click sound

        [Header("Photo UI Feedback")]
        public float messageDuration = 2f;

        private bool inCameraMode = false;
        private Camera cam; //get main cam
        private CartController cart;

        void Start()
        {
            cam = Camera.main;
            cart = FindObjectOfType<CartController>();

            if (UIManager.Instance == null)
            {
                Debug.LogError("[CameraMode] UIManager not found in scene. Add a UIManager GameObject and assign UI references.");
                return;
            }
        }
//Checks for entering or exiting camera mode, continuously detects animals in view, allows
//taking photos, auto-exits if cart starts moving

        void Update()
        {
            // Only allow camera when cart is stopped
            if (Input.GetKeyDown(toggleKey))
                TryToggleCameraMode();

            if (!inCameraMode) return;

            DetectAnimalInView();

            if (Input.GetKeyDown(photoKey))
                TryTakePhoto();

            // If cart moves again, exit camera mode automatically
            if (cart != null && !cart.IsStopped() && inCameraMode)
                ForceExitCameraMode();
        }
//Turn camera mode off and on, plays animation of camera coming in, shows or hides overlay UI,
//prevents entering when cart is moving
        public void TryToggleCameraMode()
        {
            if (cart == null || !cart.IsStopped()) return;
            ToggleCameraMode();
        }

        public void ToggleCameraMode()
        {
            inCameraMode = !inCameraMode;

            if (playerAnimator != null)
                playerAnimator.SetBool("IsHoldingCamera", inCameraMode);

            if (UIManager.Instance == null)
            {
                Debug.LogError("[CameraMode] UIManager not found; cannot toggle overlay.");
                return;
            }

            UIManager.Instance.SetOverlayActive(inCameraMode);
        }

        void ForceExitCameraMode()
        {
            inCameraMode = false;

            if (playerAnimator != null)
                playerAnimator.SetBool("IsHoldingCamera", false);

            if (UIManager.Instance == null)
            {
                Debug.LogError("[CameraMode] UIManager not found; cannot force-exit overlay.");
                return;
            }

            UIManager.Instance.SetOverlayActive(false);
        }
//Uses raycast from center of screen. If it hits an object on the animal layer, it checks the animal
//behavior script of the prefab. If animal is found, overlay gets green and if not it stays normal color
        void DetectAnimalInView()
        {
            if (cam == null) return;

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, detectionRange, animalLayer))
            {
                var animal = hit.collider.GetComponentInParent<AnimalBehavior>();
                if (animal != null)
                {
                    if (UIManager.Instance == null)
                    {
                        Debug.LogError("[CameraMode] UIManager missing; cannot set overlay ready state.");
                        return;
                    }

                    UIManager.Instance.SetOverlayReady(true);
                    return;
                }
            }
            if (UIManager.Instance == null)
            {
                Debug.LogError("[CameraMode] UIManager missing; cannot set overlay ready state.");
                return;
            }

            UIManager.Instance.SetOverlayReady(false);
        }
//If ray hits animal, it marks that animal as captured. Plays camera shutter sound, shows captured message
//reports the capture to game manager( change to animal manager), notifies slowdown zone
        public void TryTakePhoto()
        {
            if (cart == null || !cart.IsStopped()) return;
            TakePhoto();
        }

        void TakePhoto()
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, detectionRange, animalLayer))
            {
                var animal = hit.collider.GetComponentInParent<AnimalBehavior>();
                if (animal != null && animal.animalData != null)
                {
                    animal.isCaptured = true;
                    if (audioSource != null)
                        audioSource.Play();

                    string name = animal.animalData.animalName;
                    Debug.Log($"Captured photo of: {name}");

                    if (UIManager.Instance == null)
                    {
                        Debug.LogError("[CameraMode] UIManager not found; cannot show photo message.");
                        return;
                    }

                    UIManager.Instance.ShowPhotoMessage(name, messageDuration);

                    // Register capture globally
                    if (GameManager.Instance != null)
                        GameManager.Instance.RegisterCapturedAnimal(name);

                    // Notify slowdown zone about the capture (it will handle completion and timeout reset)
                    SlowdownZone currentZone = cart.currentZone;

                    if (currentZone != null)
                        currentZone.NotifyAnimalCaptured(name);
                }
            }
            
        }

        public bool IsInCameraMode() => inCameraMode;
    }
}
