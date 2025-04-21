using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements; 

namespace Ship
{
public class ShipPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    private Vector2 moveInput;
    public float moveSpeed = 5f; 
    public float acceleration = 5f; 
    public float deceleration = 5f; 
    public float tilt = 10f; // Ship tilt angle
    private float currentTilt = 0f;

    [Header("Obstacles and Pickups")]
    public float maxHealth = 5;
    public AudioClip[] chestPickupClips;
    public AudioClip damageClip;
    [HideInInspector] public float currentHealth; 
    private int localScenePoints = 0;
    private bool hasWon = false; 

    // New Input System
    private PlayerInput playerInput; 
    [HideInInspector] public InputAction moveAction, leftFootHeight, rightFootHeight; 

    // Components
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public static ShipPlayerController instance;
    private ShipCameraScroll cameraScroll; 
    private CapturyInputManager capturyInputManager;
    [HideInInspector] public RoomScriptable currentRoom;
    private ShipUIManager shipUIManager;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make persistent
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy duplicates
            return;
        }

        // Component initialization
        rb = GetComponent<Rigidbody>();
        cameraScroll = FindObjectOfType<ShipCameraScroll>();
        playerInput = GetComponent<PlayerInput>();
        shipUIManager = FindObjectOfType<ShipUIManager>();

        // Input system setup
        capturyInputManager = FindObjectOfType<CapturyInputManager>();
        moveAction = playerInput.actions["Move"];
        leftFootHeight = playerInput.actions["LeftFootHeight"]; 
        rightFootHeight = playerInput.actions["RightFootHeight"];
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (shipUIManager != null)
        {
            shipUIManager.UpdateScore(localScenePoints);
            shipUIManager.UpdateHealth(currentHealth);
        }
        else
        {
            return; // UI manager not found, do nothing
        }
    }


    private void OnEnable()
    {
        moveAction.Enable();
        leftFootHeight.Enable();    
        rightFootHeight.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        leftFootHeight.Disable();
        rightFootHeight.Disable();
    }
    
    private void FixedUpdate()
    {
        Move();
        AutoMoveUp();
    }

    // private void MotionMove()
    // {
    //     if (hasWon) return; 

    //     // Read foot heights
    //     float leftFootHeightValue = leftFootHeight.ReadValue<float>();
    //     float rightFootHeightValue = rightFootHeight.ReadValue<float>();

    //     // Determine movement direction based on which foot is higher
    //     float movementDirection = 0f;
    //     if (leftFootHeightValue > rightFootHeightValue + 0.05f) 
    //     {
    //         movementDirection = -1f; // Move left
    //     }
    //     else if (rightFootHeightValue > leftFootHeightValue + 0.05f) 
    //     {
    //         movementDirection = 1f; // Move right
    //     }

    //     // Check if there is input
    //     if (movementDirection != 0f)
    //     {
    //         float targetSpeed = -moveInput.x * moveSpeed;
    //         float newSpeed = Mathf.Lerp(
    //             rb.velocity.x, 
    //             targetSpeed, 
    //             Time.fixedDeltaTime * acceleration
    //         );
    //         rb.velocity = new Vector3(newSpeed, rb.velocity.y, 0);
    //     }
    //     else
    //     {
    //         // Decelerate to a stop if no input is given
    //         rb.velocity = new Vector3(Mathf.Lerp(rb.velocity.x, 0, Time.fixedDeltaTime * deceleration), rb.velocity.y, 0);
    //     }

    //     // Apply ship tilt when moving
    //     float targetTilt = movementDirection * tilt;
    //     currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.fixedDeltaTime * 5f);
    //     rb.MoveRotation(Quaternion.Euler(0, 0, currentTilt));
    // }

    private void Move()
    {
        moveInput = moveAction.ReadValue<Vector2>(); 
        if (hasWon) return;  // Skip movement if the game is won

        // Check if there is input
        if (moveInput.x != 0)
        {
            float targetSpeed = -moveInput.x * moveSpeed;
            float newSpeed = Mathf.Lerp(
                rb.velocity.x, 
                targetSpeed, 
                Time.fixedDeltaTime * acceleration
            );
            rb.velocity = new Vector3(newSpeed, rb.velocity.y, 0);
        }
        else
        {
            // Decelerate to a stop if no input is given
            float xVelocity = Mathf.Lerp(rb.velocity.x, 0, Time.fixedDeltaTime * deceleration);
            if (Mathf.Abs(xVelocity) < 0.1f)
            {
                xVelocity = 0f; // snap to zero
            }
            rb.velocity = new Vector3(xVelocity, rb.velocity.y, 0);
        }

        // Tilt ship based on movement
        float targetTilt = moveInput.x * tilt; 
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.fixedDeltaTime * 5f);
        rb.MoveRotation(Quaternion.Euler(0, 0, currentTilt));
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
        }    
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Deplete health and play sound
            HealthDeplete(1); 
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0
            ShipAudioManager.instance.SetSFXPitch(1f);
            ShipAudioManager.instance.SetSFXVolume(0.25f);
            ShipAudioManager.instance.PlaySFX(damageClip);

            // Make transparent
            other.gameObject.GetComponent<ShipObstacle>().TriggerObstacleEffect();

            // Stop ship movement
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f); 
            Debug.Log("Hit an obstacle! Velocity cleared. Health: " + currentHealth);
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

    public void HealthDeplete(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // Ensure health doesn't go below 0 (mainly for testing purposes)

        if (shipUIManager != null)
            shipUIManager.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
        {
            ShipGameManager.instance.TriggerGameOver();
        }
    }

    public void DisablePlayerController()
    {
        // Disable player movement and input
        moveAction.Disable();
        leftFootHeight.Disable();
        rightFootHeight.Disable();
        rb.velocity = Vector3.zero;

        // Make sure ship doesn't tilt over
        rb.freezeRotation = true;
        rb.isKinematic = true;
    }

    public void EnablePlayerController()
    {
        moveAction.Enable();
        leftFootHeight.Enable();    
        rightFootHeight.Enable();
        rb.isKinematic = false;
        rb.freezeRotation = false;
    }

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