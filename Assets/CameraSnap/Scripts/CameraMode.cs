using UnityEngine;
using System.Collections;

namespace CameraSnap
{
    // This lets the player access camera mode. The player will open the camera and it will
    // indicate to the player that there is no animals detected until the player has an animal in view.
    // Note: If doing zoom in and out, maybe detection range will change depending on zoom.

    public class CameraMode : MonoBehaviour
    {
        [Header("Camera Controls")]
        [SerializeField] private Animator playerAnimator;
        [SerializeField] private KeyCode toggleKey = KeyCode.C;
        [SerializeField] private KeyCode photoKey = KeyCode.Mouse0;
        [SerializeField] private float detectionRange = 100f;
        [SerializeField] private LayerMask animalLayer;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float messageDuration = 2f;

        private bool isActive;
        private Camera mainCamera;
        private CartController cart;
        private UIManager ui => UIManager.Instance;

        void Start()
        {
            mainCamera = Camera.main;
            cart = FindObjectOfType<CartController>();
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                TryToggleCameraMode();

            if (!isActive) return;

            // Auto-exit if cart moves
            if (cart != null && !cart.IsStopped())
            {
                ExitCameraMode();
                return;
            }

            DetectAnimalInView();

            if (Input.GetKeyDown(photoKey))
                TryTakePhoto();
        }

        public void TryToggleCameraMode()
        {
            if (cart == null || !cart.IsStopped()) return;
            
            if (isActive)
                ExitCameraMode();
            else
                EnterCameraMode();
        }

        private void EnterCameraMode()
        {
            isActive = true;
            playerAnimator?.SetBool("IsHoldingCamera", true);
            ui?.SetOverlayActive(true);
            ui?.SetGuideState(UIManager.GuideState.WeightShift);
            Debug.Log("[Camera] Entered camera mode");
        }

        private void ExitCameraMode()
        {
            isActive = false;
            playerAnimator?.SetBool("IsHoldingCamera", false);
            ui?.SetOverlayActive(false);
            Debug.Log("[Camera] Exited camera mode");
        }

        private void DetectAnimalInView()
        {
            if (mainCamera == null) return;

            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            bool foundAnimal = Physics.Raycast(ray, out RaycastHit hit, detectionRange, animalLayer) &&
                              hit.collider.GetComponentInParent<AnimalBehavior>() != null;

            ui?.SetOverlayReady(foundAnimal);
            ui?.SetGuideState(foundAnimal ? UIManager.GuideState.FootRaise : UIManager.GuideState.WeightShift);
        }

        public void TryTakePhoto()
        {
            if (cart == null || !cart.IsStopped()) return;

            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (!Physics.Raycast(ray, out RaycastHit hit, detectionRange, animalLayer)) return;

            var animal = hit.collider.GetComponentInParent<AnimalBehavior>();
            if (animal == null || animal.animalData == null) return;

            // Capture the animal
            animal.isCaptured = true;
            audioSource?.Play();

            string name = animal.animalData.animalName;
            Debug.Log($"[Camera] Captured photo of: {name}");

            // Update UI and game state
            ui?.ShowPhotoMessage(name, messageDuration);
            GameManager.Instance?.RegisterCapturedAnimal(name);
            // Reveal the target in the UI if this animal was one of the session targets
            UIManager.Instance?.RevealTarget(name);
            cart.currentZone?.NotifyAnimalCaptured(name);
        }

        public bool IsInCameraMode() => isActive;
    }
}
