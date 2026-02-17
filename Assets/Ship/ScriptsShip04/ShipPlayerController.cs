using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
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

    // Internal variables for motion
    private float motionInputX = 0f;
    private bool isLeftHipActive = false;
    private bool isRightHipActive = false;

    // New Input System
    public InputActionAsset playerInput;
    [HideInInspector] public InputAction weightShiftXAction;
    // [HideInInspector] public InputAction moveAction;
    //[HideInInspector] public InputAction leftHipAction, rightHipAction, weightShiftLeftAction, weightShiftRightAction, weightShiftXAction; // Motion input actions
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
        // moveAction = playerInput.actions["Move"];

        var motionMap = playerInput.FindActionMap("MotionTracking");
            if (motionMap != null)
            {
                motionMap.Enable();
                weightShiftXAction = motionMap.FindAction("WeightShiftX");
            }
                //leftHipAction = motionMap.FindAction("LeftHipAbducted");
                //rightHipAction = motionMap.FindAction("RightHipAbducted");
                //weightShiftLeftAction = motionMap.FindAction("WeightShiftLeft"); 
                //weightShiftRightAction = motionMap.FindAction("WeightShiftRight");
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
        // moveAction.Enable();
       /* leftHipAction.Enable();
        rightHipAction.Enable();
        weightShiftLeftAction.Enable();*/
        //weightShiftRightAction.Enable();
        weightShiftXAction.Enable();

       /* leftHipAction.performed += LeftMotionMovement;
        rightHipAction.performed += RightMotionMovement;
        leftHipAction.canceled += StopMotionMovement;
        rightHipAction.canceled += StopMotionMovement;*/
    }

    private void OnDisable()
    {
        // moveAction.Disable();
        /*leftHipAction.Disable();
        rightHipAction.Disable();
        weightShiftLeftAction.Disable();
        weightShiftRightAction.Disable();*/
        weightShiftXAction.Disable();

      /*  leftHipAction.performed -= LeftMotionMovement;
        rightHipAction.performed -= RightMotionMovement;
        leftHipAction.canceled -= StopMotionMovement;
        rightHipAction.canceled -= StopMotionMovement;*/
    }
    
    private void FixedUpdate()
    {
        // Move();  // Filler movement
        HandleMotionMovement();
        AutoMoveUp();

    }

    #region Movement
   /* private void LeftMotionMovement(InputAction.CallbackContext context)
    {
        isLeftHipActive = true;
    }

    private void RightMotionMovement(InputAction.CallbackContext context)
    {
        isRightHipActive = true;
    }

    private void StopMotionMovement(InputAction.CallbackContext context)
    {
        isLeftHipActive = false;
        isRightHipActive = false;
    }

    private void OnLeftHip(InputAction.CallbackContext ctx)
    {
        LeftMotionMovement(ctx);
    }

    private void OnRightHip(InputAction.CallbackContext ctx)
    {
        RightMotionMovement(ctx);
    }*/

    private void HandleMotionMovement()
    {
       //f (hasWon) return;
        
        float horizontalInput = 0f;

        horizontalInput = weightShiftXAction.ReadValue<float>();
            Debug.Log(horizontalInput);
            Debug.Log(weightShiftXAction.ReadValue<float>());
        float targetSpeed = horizontalInput * moveSpeed;
        float currentSpeed = rb.velocity.x;
            float newSpeed;

            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                string dataStr = $"WeightShiftX: {horizontalInput:F2}; Shifting: {((horizontalInput > 0) ? "Right" : "Left")}";
                DataLogger.Instance.LogInput("WeightShiftX", horizontalInput.ToString("F2"));
                newSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * acceleration);
            } else
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

    // private void Move()
    // {
    //     moveInput = moveAction.ReadValue<Vector2>(); 
    //     if (hasWon) return;  // Skip movement if the game is won

    //     // Check if there is input
    //     if (moveInput.x != 0)
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
    //         float xVelocity = Mathf.Lerp(rb.velocity.x, 0, Time.fixedDeltaTime * deceleration);
    //         if (Mathf.Abs(xVelocity) < 0.1f)
    //         {
    //             xVelocity = 0f; // snap to zero
    //         }
    //         rb.velocity = new Vector3(xVelocity, rb.velocity.y, 0);
    //     }

    //     // Tilt ship based on movement
    //     float targetTilt = moveInput.x * tilt; 
    //     currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.fixedDeltaTime * 5f);
    //     rb.MoveRotation(Quaternion.Euler(0, 0, currentTilt));
    // }

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
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Deplete health and play sound
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

    #region Enable/Disable Player Controller
    public void DisablePlayerController()
    {
            // Disable player movement and input
            // moveAction.Disable();
            /*leftHipAction.Disable();
            rightHipAction.Disable();
            rb.velocity = Vector3.zero;*/
            weightShiftXAction.Disable();
            rb.velocity = Vector2.zero;
        // Make sure ship doesn't tilt over
        rb.freezeRotation = true;
        rb.isKinematic = true;
    }

    public void EnablePlayerController()
    {
            weightShiftXAction.Enable();
            // moveAction.Enable();
            /*leftHipAction.Enable();
        rightHipAction.Enable();*/
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