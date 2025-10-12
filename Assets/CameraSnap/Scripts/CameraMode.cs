using UnityEngine;
using System.Collections;

namespace CameraSnap
{
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

        void TryToggleCameraMode()
        {
            if (cart == null || !cart.IsStopped()) return;
            ToggleCameraMode();
        }

        void ToggleCameraMode()
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
                var animal = hit.collider.GetComponentInParent<AnimalIdentifier>();
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

        void TryTakePhoto()
        {
            if (cart == null || !cart.IsStopped()) return;
            TakePhoto();
        }

        void TakePhoto()
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, detectionRange, animalLayer))
            {
                var animal = hit.collider.GetComponentInParent<AnimalIdentifier>();
                if (animal != null)
                {
                    Debug.Log($"Captured photo of: {animal.animalData.animalName}");

                    //  Show UI message
                    StartCoroutine(ShowPhotoMessage(animal.animalData.animalName));

                   
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
