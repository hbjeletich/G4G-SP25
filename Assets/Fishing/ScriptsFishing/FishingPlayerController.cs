using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fishing
{
[System.Serializable]
public struct FishingZoneSettings
{
    public float yDepth;
    public Vector2 xRange;
    public Vector2 yRandomizationRange;
}

public class FishingPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; 
    public float retractSpeed = 5f;
    public float acceleration = 5f; 
    public float deceleration = 5f; 
    public float tilt = 10f; // Tilt angle for hook
    private Vector2 moveInput;
    private float currentTilt = 0f;

    private bool isHookMoving = false;
    [HideInInspector] public bool isFishingInProgress = false;

    // Determines how far the hook is sent when cast
    public enum FishingZone
    {
        Closest,
        Middle,
        Farthest
    }

    [Header("Fishing Zones Settings")]
    public FishingZoneSettings closestZone;
    public FishingZoneSettings middleZone;
    public FishingZoneSettings farthestZone;
    public Vector2 retractPosition = new Vector2(0, 0);

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
        if (!distanceMeter.isFishing)
        {
            Move();
        }
        else
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero; 
        }
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

    #region CastHook
    public void CastHook()
    {
        FishingZone zone = distanceMeter.currentZone;  // Get the current fishing zone from the distance meter
        Debug.Log("Casting hook in zone: " + zone);

        FishingAudioManager.instance.PlaySFX(FishingAudioManager.instance.castHookSFX);
        FishingCameraController.instance.ShiftToFishingView();
        StartCoroutine(distanceMeter.FadeCanvas(0f));
        distanceMeter.isFishing = false;
        isFishingInProgress = true;

        // Determine target position based on the selected zone
        FishingZoneSettings zoneSettings = new FishingZoneSettings();
        switch (zone)
        {
            case FishingZone.Closest:
                zoneSettings = closestZone;
                break;
            case FishingZone.Middle:
                zoneSettings = middleZone;
                break;
            case FishingZone.Farthest:
                zoneSettings = farthestZone;
                break;
        }
        Vector3 startPos = new Vector3(retractPosition.x, retractPosition.y, transform.position.z);
        Vector3 targetPos = new Vector3(
            Random.Range(zoneSettings.xRange.x, zoneSettings.xRange.y),
            startPos.y - zoneSettings.yDepth + Random.Range(zoneSettings.yRandomizationRange.x, zoneSettings.yRandomizationRange.y),
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
        Vector3 startPos = new Vector3(retractPosition.x, retractPosition.y, transform.position.z);

        // Calc distance to move
        while (elapsedTime < castTime)
        {
            float newY = Mathf.Lerp(startPos.y, targetPos.y, elapsedTime / castTime);
            rb.position = new Vector2(rb.position.x, newY);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.position = new Vector2(rb.position.x, targetPos.y);

        // Allow hook to stay at target position for 2 seconds
        yield return new WaitForSeconds(2f);  

        // Retract hook back
        FishingAudioManager.instance.PlaySFX(FishingAudioManager.instance.reelInSFX); // Play reel in sound
        elapsedTime = 0f;
        float retractDuration = 1f / retractSpeed;

        while (elapsedTime < retractDuration)
        {
            float newY = Mathf.Lerp(targetPos.y, startPos.y, elapsedTime / retractDuration);
            rb.position = new Vector2(rb.position.x, newY);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rb.position = new Vector2(rb.position.x, startPos.y);
        isHookMoving = false;

        // Stop sound, shift camera, and fade in distance meter
        FishingAudioManager.instance.StopAllSFX(); 
        FishingCameraController.instance.ShiftToMeterView(); 
        StartCoroutine(distanceMeter.FadeCanvas(1f)); 

        // Restart distance meter
        isFishingInProgress = false;
        FindObjectOfType<DistanceMeter>().RestartMeter();
        distanceMeter.isFishing = true; 
    }
    #endregion

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

    private void OnDrawGizmos()
    {
        DrawZoneGizmo(closestZone, Color.green);
        DrawZoneGizmo(middleZone, Color.yellow);
        DrawZoneGizmo(farthestZone, Color.red);
    }

    private void DrawZoneGizmo(FishingZoneSettings zone, Color color)
    {
        Gizmos.color = color;

        Vector3 startPos = new Vector3(retractPosition.x, retractPosition.y, transform.position.z);

        float yCenter = startPos.y - zone.yDepth + ((zone.yRandomizationRange.x + zone.yRandomizationRange.y) * 0.5f);
        float yHeight = Mathf.Abs(zone.yRandomizationRange.y - zone.yRandomizationRange.x);

        Vector3 center = new Vector3(
            (zone.xRange.x + zone.xRange.y) * 0.5f,
            yCenter,
            startPos.z
        );

        Vector3 size = new Vector3(
            Mathf.Abs(zone.xRange.y - zone.xRange.x),
            yHeight > 0 ? yHeight : 0.5f,
            0.1f
        );

        Gizmos.DrawWireCube(center, size);
    }
}
}