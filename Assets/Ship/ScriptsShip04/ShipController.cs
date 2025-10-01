using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Ship
{
    public class ShipController : MonoBehaviour
    {
        [Header("Movement Settings")]
        private Vector2 moveInput;
        public float moveSpeed = 5f;
        public float acceleration = 5f;
        public float deceleration = 5f;
        public float tilt = 10f; // Ship tilt angle
        private float currentTilt = 0f;

        [Header("Obstacles and Pickups")]
        public AudioClip[] chestPickupClips;
        public AudioClip damageClip;
        private int localScenePoints = 0;
        private bool hasWon = false;

        // Internal variables for motion
        private float motionInputX = 0f;
        private bool isLeftHipActive = false;
        private bool isRightHipActive = false;

        // New Input System
        public InputActionAsset playerInput;
        [HideInInspector] public InputAction weightShiftXAction;

        // Components
        [HideInInspector] public Rigidbody rb;
        private ShipCameraScroll cameraScroll;
        private CapturyInputManager capturyInputManager;
        [HideInInspector] public RoomScriptable currentRoom;
        private ShipUIManager shipUIManager;

        private void Awake()
        {
            // Component initialization
            rb = GetComponent<Rigidbody>();
            cameraScroll = FindObjectOfType<ShipCameraScroll>();
            //playerInput = GetComponent<PlayerInput>();
            shipUIManager = FindObjectOfType<ShipUIManager>();

            // Input system setup
            capturyInputManager = FindObjectOfType<CapturyInputManager>();

            var motionMap = playerInput.FindActionMap("MotionTracking");
            if (motionMap != null)
            {
                motionMap.Enable();
                weightShiftXAction = motionMap.FindAction("WeightShiftX");
            }

            if (motionMap == null) Debug.Log("Could not find map.");
            if (weightShiftXAction == null) Debug.Log("Could not find action.");


        }

        private void Start()
        {
 
            if (shipUIManager != null)
            {
                shipUIManager.UpdateScore(localScenePoints);
            }
            else
            {
                return; // UI manager not found, do nothing
            }
        }


        private void OnEnable()
        {
            weightShiftXAction.Enable();
        }

        private void OnDisable()
        {
            weightShiftXAction.Disable();
        }

        private void FixedUpdate()
        {
            Debug.Log($"WeightShiftX enabled: {weightShiftXAction.enabled}");
            Debug.Log($"Action map enabled: {weightShiftXAction.actionMap.enabled}");
            Debug.Log($"PlayerController WeightShiftX value: {weightShiftXAction.ReadValue<float>()}");
            HandleMotionMovement();
            AutoMoveUp();

        }

        #region Movement

        private void HandleMotionMovement()
        {

            float horizontalInput = 0f;

            horizontalInput = -weightShiftXAction.ReadValue<float>();
            Debug.Log(horizontalInput);
            Debug.Log(weightShiftXAction.ReadValue<float>());
            float targetSpeed = horizontalInput * moveSpeed;
            float currentSpeed = rb.velocity.x;
            float newSpeed;

            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                newSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * acceleration);
            }
            else
            {
                newSpeed = Mathf.Lerp(currentSpeed, 0f, Time.fixedDeltaTime * deceleration);
                if (Mathf.Abs(newSpeed) < 0.1f)
                {
                    newSpeed = 0f;
                }
            }

            rb.velocity = new Vector2(newSpeed, rb.velocity.y);

            // Apply tilt
            float targetTilt = horizontalInput * tilt;
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.fixedDeltaTime * 5f);
            rb.MoveRotation(Quaternion.Euler(0, 0, -currentTilt));
        }

      
        private void AutoMoveUp()
        {
            if (cameraScroll != null)
            {
                Vector3 targetPosition = new Vector3(
                    transform.position.x,
                    cameraScroll.transform.position.y - 2f,
                    transform.position.z
                );
                rb.MovePosition(targetPosition);
           //   Debug.Log(rb.velocity.y);
            }
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Obstacle"))
            {
                // play sound
              
                ShipAudioManager.instance.SetSFXPitch(1f);
                ShipAudioManager.instance.SetSFXVolume(0.25f);
                ShipAudioManager.instance.PlaySFX(damageClip);

                // Red viginette effect
                if (shipUIManager != null)
                    shipUIManager.RedViginette();

                // Make transparent
                other.gameObject.GetComponent<ShipObstacle>().TriggerObstacleEffect();

                // Stop ship movement
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
                Debug.Log("Hit an obstacle! Velocity cleared.");
            }

            if (other.CompareTag("Chest"))
            {
                if (chestPickupClips.Length > 0)
                {
                    AudioClip randomClip = chestPickupClips[Random.Range(0, chestPickupClips.Length)];
                    ShipAudioManager.instance.SetSFXVolume(0.5f);
                    ShipAudioManager.instance.PlaySFX(randomClip);
                }
                Destroy(other.gameObject);  // Destroy chest

                // Update local score 
                localScenePoints += 5;

                // Update UI
                if (shipUIManager != null)
                    shipUIManager.UpdateScore(localScenePoints);

                // Update global total points
                ShipGameManager.totalPoints += 5;
            }

            if (other.CompareTag("ChestParticles"))
            {
                Destroy(other.gameObject);
            }
        }

        #region Enable/Disable Player Controller
        public void DisablePlayerController()
        {
            weightShiftXAction.Disable();
            rb.velocity = Vector2.zero;

            // Make sure ship doesn't tilt over
            rb.freezeRotation = true;
            rb.isKinematic = true;
        }

        public void EnablePlayerController()
        {
            weightShiftXAction.Enable();

            rb.isKinematic = false;
            rb.freezeRotation = false;
        }
        #endregion

        public void UpdatePlayerRoom(string newRoomName)
        {
            if (RoomManager.RoomDictionary.TryGetValue(newRoomName, out var newRoom))
            {
                currentRoom = newRoom;
            }
            else
            {
                Debug.LogWarning("Room not found: " + newRoomName);
            }
        }
    }
}