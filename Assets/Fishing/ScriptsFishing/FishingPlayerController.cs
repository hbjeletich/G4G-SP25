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
    public float retractSpeed = 5f;
    public float acceleration = 5f; 
    public float deceleration = 5f; 
    public float tilt = 10f; // Tilt angle for hook
    private float currentTilt = 0f;
    private bool isHookMoving = false;


    // Determines how far the hook is sent when cast
    public enum FishingZone
    {
        Closest,
        Middle,
        Farthest
    }

    // New Input System
    private PlayerInput playerInput; 
    [HideInInspector] public InputAction moveAction, fishAction, leftFootHeight, rightFootHeight; 
    [HideInInspector] public FishingPlayerController instance; // Singleton instance
    private Rigidbody2D rb;
    private DistanceMeter distanceMeter; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else if (instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        // Component initialization
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        distanceMeter = FindObjectOfType<DistanceMeter>();

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

    public FishingZone DetermineFishingZone(float value)
    {
        if (value < 0.33f)
            return FishingZone.Closest;
        else if (value < 0.66f)
            return FishingZone.Middle;
        else
            return FishingZone.Farthest;
    }

    public void CastHook(FishingZone zone)
    {
        Debug.Log("Casting hook in zone: " + zone);

        float depth = 0f;
        Vector2 xRange = Vector2.zero;

        switch (zone)
        {
            case FishingZone.Closest:
                depth = 5f;
                xRange = new Vector2(-3f, 3f);
                break;
            case FishingZone.Middle:
                depth = 15f;
                xRange = new Vector2(-4f, 4f);
                break;
            case FishingZone.Farthest:
                depth = 25f;
                xRange = new Vector2(-5f, 5f);
                break;
        }

        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(
            Random.Range(xRange.x, xRange.y),
            startPos.y - depth + Random.Range(-1f, 1f),
            startPos.z
        );
        StartCoroutine(CastHookRoutine(targetPos));
    }

    private IEnumerator CastHookRoutine(Vector3 targetPos)
    {
        isHookMoving = true;
        distanceMeter.isFishing = false;

        float elapsedTime = 0f;
        float castTime = 1.5f;
        Vector3 startPos = transform.position;

        // Calc distance to move
        while (elapsedTime < castTime)
        {
            float newY = Mathf.Lerp(startPos.y, targetPos.y, elapsedTime / castTime);
            rb.position = new Vector2(rb.position.x, newY);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.position = new Vector2(rb.position.x, targetPos.y);

        yield return new WaitForSeconds(2f);  // Allow hook to stay at target position for 2 seconds

        elapsedTime = 0f;
        float retractDuration = 1f / retractSpeed;

        // Retract hook back
        while (elapsedTime < retractDuration)
        {
            float newY = Mathf.Lerp(targetPos.y, startPos.y, elapsedTime / retractDuration);
            rb.position = new Vector2(rb.position.x, newY);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.position = new Vector2(rb.position.x, startPos.y);
        isHookMoving = false;
        distanceMeter.isFishing = true; // Allow distance meter to be used again

        // Restart distance meter
        FindObjectOfType<DistanceMeter>().RestartMeter();
    }


    private void Move()
    {
        moveInput = moveAction.ReadValue<Vector2>(); // Read movement input

        float movementBoost = isHookMoving ? 1.2f : 1f; // Boost speed when hook is moving
        float targetSpeed = moveInput.x * moveSpeed * movementBoost;
        float dampenFactor = moveInput.x == 0 ? 0.9f : 1f;
        float newSpeed = Mathf.Lerp(
            rb.velocity.x * dampenFactor,
            targetSpeed,
            Time.fixedDeltaTime * (moveInput.x != 0 ? acceleration : deceleration)
        );
        rb.velocity = new Vector2(newSpeed, rb.velocity.y);

        // Apply tilt 
        float targetTilt = moveInput.x * tilt;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.fixedDeltaTime * 5f);
        rb.MoveRotation(Quaternion.Euler(0, 0, -currentTilt));
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fish"))
        {
          return; // Add logic for catching fish here later, most will be in the Fish script
        }
    }
}