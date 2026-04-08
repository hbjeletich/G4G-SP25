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
        [SerializeField] private KeyCode photoKey = KeyCode.Space;
        [SerializeField] private float detectionRange = 100f;
        [SerializeField] private LayerMask animalLayer;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float messageDuration = 2f;


        private bool isActive;
        private Camera mainCamera;
        private CartController cart;
        private CameraPan cameraPan;
        private UIManager ui => UIManager.Instance;

        void Start()
        {
            mainCamera = Camera.main;
            cart = FindObjectOfType<CartController>();
            cameraPan = FindObjectOfType<CameraPan>();
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                TryToggleCameraMode();
            HandleKeyboardPan();

            if (!isActive) return;

            // Auto-exit if cart moves
            if (cart != null && !cart.IsStopped())
            {
                ExitCameraMode();
                return;
            }

            DetectAnimalInView();

            if (Input.GetKey(photoKey))
                TryTakePhoto();
        }

        private void HandleKeyboardPan()
        {
            float panInput = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                panInput -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                panInput += 1f;

            cameraPan?.ManualPan(panInput);
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

            // Detect immediately so an animal already in view gets picked up
            DetectAnimalInView();
        }

        private void ExitCameraMode()
        {
            isActive = false;
            playerAnimator?.SetBool("IsHoldingCamera", false);
            ui?.SetOverlayActive(false);
            cameraPan?.ClearLockTarget();
        }

        private void DetectAnimalInView()
        {
            if (mainCamera == null) return;

            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            bool foundAnimal = Physics.Raycast(ray, out RaycastHit hit, detectionRange, animalLayer) &&
                              hit.collider.GetComponentInParent<AnimalBehavior>() != null;

            // ui?.SetOverlayReady(foundAnimal);
            // ui?.SetGuideState(foundAnimal ? UIManager.GuideState.FootRaise : UIManager.GuideState.WeightShift);
            if(foundAnimal)
            {
                ui?.SetGuideState(UIManager.GuideState.FootRaise);
                ui?.SetOverlayReady(true);

                var animal = hit.collider.GetComponentInParent<AnimalBehavior>();
                cameraPan?.SetLockTarget(animal.transform);
            } 
            else
            {
                ui?.SetGuideState(UIManager.GuideState.WeightShift);
                ui?.SetOverlayReady(false);
                cameraPan?.ClearLockTarget();
            }


            // lock onto animal?
        }

        public void TryTakePhoto()
        {
            if (cart == null || !cart.IsStopped()) 
            {
                Debug.LogError("Cart is null or not stopped!");
                return;
            }

            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (!Physics.Raycast(ray, out RaycastHit hit, detectionRange, animalLayer)) return;

            var animal = hit.collider.GetComponentInParent<AnimalBehavior>();
            if (animal == null || animal.animalData == null) 
            {
                DataLogger.Instance.LogMinigameEvent("CandidCritters", "PhotoFailed", "No animal detected");
                return;
            }

            // Capture the animal
            animal.isCaptured = true;
            animal.Flee();
            audioSource?.Play();

            string name = animal.animalData.animalName;

            DataLogger.Instance.LogMinigameEvent("CandidCritters", "PhotoTaken", name);

            // Update UI and game state
            ui?.ShowPhotoMessage(name, messageDuration);
            GameManager.Instance?.RegisterCapturedAnimal(name);
            // Reveal the target in the UI if this animal was one of the session targets
            UIManager.Instance?.RevealTarget(name);
            cart.currentZone?.NotifyAnimalCaptured(name);

            // reset camera look
            cameraPan?.ClearLockTarget();
        }

        public bool IsInCameraMode() => isActive;
    }
}