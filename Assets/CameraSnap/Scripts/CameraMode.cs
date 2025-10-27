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
        public Animator playerAnimator;
        public GameObject cameraOverlayUI;
        public KeyCode toggleKey = KeyCode.C;
        public KeyCode photoKey = KeyCode.Mouse0;
        public Color normalColor = Color.white;
        public Color readyColor = Color.green;
        public float detectionRange = 100f;
        public LayerMask animalLayer;
        public AudioSource audioSource;

        [Header("Photo UI Feedback")]
        public float messageDuration = 2f;

        private bool inCameraMode = false;
        private Camera cam;
        private UnityEngine.UI.Image overlayImage;
        private CartController cart;
        public TMPro.TMP_Text photoText;

        void Start()
        {
            cam = Camera.main;
            overlayImage = cameraOverlayUI.GetComponent<UnityEngine.UI.Image>();
            cart = FindObjectOfType<CartController>();
        }

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

            if (cameraOverlayUI != null)
                cameraOverlayUI.SetActive(inCameraMode);
        }

        void ForceExitCameraMode()
        {
            inCameraMode = false;

            if (playerAnimator != null)
                playerAnimator.SetBool("IsHoldingCamera", false);

            if (cameraOverlayUI != null)
                cameraOverlayUI.SetActive(false);
        }

        void DetectAnimalInView()
        {
            if (cam == null) return;

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, detectionRange, animalLayer))
            {
               var animal = hit.collider.GetComponentInParent<AnimalBehavior>();
if (animal != null)
                {
                    if (overlayImage != null)
                        overlayImage.color = readyColor;
                    return;
                }
            }

            if (overlayImage != null)
                overlayImage.color = normalColor;
        }

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
    audioSource.Play();
    string name = animal.animalData.animalName;
                    Debug.Log($"Captured photo of: {name}");
                    StartCoroutine(ShowPhotoMessage(name));

                    // Register capture globally
                    if (GameManager.Instance != null)
                        GameManager.Instance.RegisterCapturedAnimal(name);

                    // Notify slowdown zone to check if both animals are captured
                    SlowdownZone currentZone = cart.currentZone;

                    if (currentZone != null)
                        currentZone.CheckForZoneCompletion();
                }
            }
            
        }

        public bool IsInCameraMode() => inCameraMode;

        private IEnumerator ShowPhotoMessage(string animalName)
        {
            if (photoText == null) yield break;

            photoText.text = $"Captured photo of {animalName}!";
            photoText.canvasRenderer.SetAlpha(1f);

            // Show for messageDuration seconds
            yield return new WaitForSeconds(messageDuration);

            // Fade out smoothly
            photoText.CrossFadeAlpha(0f, 1f, false);
        }
    }
}
