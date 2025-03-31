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
    [HideInInspector] public InputAction moveAction; 
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

        rb = GetComponent<Rigidbody2D>();

        // New Input System setup
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        moveAction.Enable();
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }
    
    private void FixedUpdate()
    {
        Move(); 
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