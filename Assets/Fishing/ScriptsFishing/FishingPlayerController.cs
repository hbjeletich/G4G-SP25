using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements; 

public class FishingPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    private Vector2 moveInput;
    public float moveSpeed = 5f; 
    public float acceleration = 5f; 
    public float deceleration = 5f; 
    public float tilt = 10f; // Tilt angle for hook
    private float currentTilt = 0f;

    // New Input System
    private PlayerInput playerInput; 
    [HideInInspector] public InputAction moveAction, fishAction, leftFootHeight, rightFootHeight; 
    [HideInInspector] public FishingPlayerController instance; // Singleton instance
    private Rigidbody2D rb;

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
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        // Input system setup
        moveAction = playerInput.actions["Move"];
        fishAction = playerInput.actions["Fish"];
        leftFootHeight = playerInput.actions["LeftFootHeight"]; 
        rightFootHeight = playerInput.actions["RightFootHeight"];
    }

    private void OnEnable()
    {
        moveAction.Enable();
        fishAction.Enable();
        leftFootHeight.Enable();    
        rightFootHeight.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        fishAction.Disable();
        leftFootHeight.Disable();
        rightFootHeight.Disable();
    }
    
    private void FixedUpdate()
    {
        Move();
    }

    //  private void MotionMove()
    // {
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

    public void CastHook(float distance)
    {
        Debug.Log("Casting hook with distance: " + distance);
        
        // Move the hook downward based on the distance
        StartCoroutine(CastHookRoutine(distance));
    }

    private IEnumerator CastHookRoutine(float distance)
    {
        float elapsedTime = 0f;
        float castTime = 1.5f; // Time for the hook to reach the max distance
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(0, -distance, 0);

        while (elapsedTime < castTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / castTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; // Ensure the hook reaches exactly the target
    }

    private void Move()
    {
        moveInput = moveAction.ReadValue<Vector2>(); // Read movement input

        // Target velocity based on input
        float targetSpeed = moveInput.x * moveSpeed;
        float newSpeed = Mathf.Lerp(
            rb.velocity.x, 
            targetSpeed, 
            Time.fixedDeltaTime * (moveInput.x != 0 ? acceleration : deceleration)
        );
        rb.velocity = new Vector3(newSpeed, rb.velocity.y, 0);

        // Tilt ship based on movement
        float targetTilt = moveInput.x * tilt; 
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.fixedDeltaTime * 5f);
        rb.MoveRotation(Quaternion.Euler(0, 0, currentTilt));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fish"))
        {
          return; // Add logic for catching fish here later, most will be in the Fish script
        }
    }
}