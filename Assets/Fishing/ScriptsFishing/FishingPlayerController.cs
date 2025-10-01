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

    [Header("Fishing Line")]
    public Transform startOfLine;
    public Vector3 hookOffset = new Vector3(-0.2f, 0.65f, 0f);
    public Vector3 distanceOffset = new Vector3(0, 0, 0);

    [HideInInspector] public bool isFishingInProgress = false;
    [SerializeField] private int lineSegmentCount = 20;
    [SerializeField] private AnimationCurve tensionCurve;
    [SerializeField] private float tensionSpeed = 2f;
    private float tension = 1f;
    private float lineCurveHeight = 0.2f;
    private float motionInputX = 0f;
    private bool isLeftHipActive = false;
    private bool isRightHipActive = false;

    // New Input System
    private PlayerInput playerInput; 
    // [HideInInspector] public InputAction moveAction, fishAction; // Filler
    [HideInInspector] public InputAction leftHipAction, rightHipAction, weightShiftLeftAction, weightShiftRightAction, weightShiftXAction; // Motion input actions
    private Rigidbody2D rb;
    private DistanceMeter distanceMeter; 
    private LineRenderer lineRenderer; 

    private void Awake()
    {
        // Component initialization
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        distanceMeter = FindObjectOfType<DistanceMeter>();
        lineRenderer = GetComponent<LineRenderer>();

        // Input system setup
        // moveAction = playerInput.actions["Move"];
        // fishAction = playerInput.actions["Fish"];
        leftHipAction = playerInput.actions["LeftHipAbducted"];
        rightHipAction = playerInput.actions["RightHipAbducted"];
        weightShiftLeftAction = playerInput.actions["WeightShiftLeft"]; 
        weightShiftRightAction = playerInput.actions["WeightShiftRight"];
    }

    private void OnEnable()
    {
        // moveAction.Enable();
        // fishAction.Enable();
        leftHipAction.Enable();
        rightHipAction.Enable();
        weightShiftLeftAction.Enable();
        weightShiftRightAction.Enable();
        weightShiftXAction.Enable();

        leftHipAction.performed += LeftMotionMovement;
        rightHipAction.performed += RightMotionMovement;
        leftHipAction.canceled += StopMotionMovement;
        rightHipAction.canceled += StopMotionMovement;
    }

    private void OnDisable()
    {
        // moveAction.Disable();
        // fishAction.Disable();
        leftHipAction.Disable();
        rightHipAction.Disable();
        weightShiftLeftAction.Disable();
        weightShiftRightAction.Disable();
        weightShiftXAction.Disable();

        leftHipAction.performed -= LeftMotionMovement;
        rightHipAction.performed -= RightMotionMovement;
        leftHipAction.canceled -= StopMotionMovement;
        rightHipAction.canceled -= StopMotionMovement;
    }
    
    private void FixedUpdate()
    {
        if (!distanceMeter.isFishing)
        {
            HandleMotionMovement();
            UpdateFishingLine();
        }
        else
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero; 
        }
    }

    private void UpdateFishingLine()
    {
        if (lineRenderer == null || startOfLine == null)
            return;

        // Define start and end of line
        Vector3 start = startOfLine.position;
        Vector3 end = transform.position + hookOffset;

        start.z = -0.1f;
        end.z = -0.1f;

        float distance = Vector3.Distance(start, end + distanceOffset);
        float normalized = Mathf.InverseLerp(2f, 6f, distance);
        tension = Mathf.Clamp01(normalized);

        float maxCurveHeight = 2.0f;
        lineCurveHeight = tensionCurve.Evaluate(1f - tension) * maxCurveHeight;

        // Calculate line points
        lineRenderer.positionCount = lineSegmentCount;
        for (int i = 0; i < lineSegmentCount; i++)
        {
            float t = i / (float)(lineSegmentCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            float curve = Mathf.Sin(t * Mathf.PI) * lineCurveHeight;
            point.y -= curve;

            point.z = -0.1f;
            lineRenderer.SetPosition(i, point);
        }
        lineRenderer.enabled = true;
    }


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

        // Allow hook to stay at target position for 5 seconds
        yield return new WaitForSeconds(5f);  

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

    #region Movement
    private void LeftMotionMovement(InputAction.CallbackContext context)
    {
        isLeftHipActive = true;
    }

    private void RightMotionMovement(InputAction.CallbackContext context)
    {
        isRightHipActive = true;
    }

    private void StopMotionMovement(InputAction.CallbackContext context)
    {
        // Use context to detect which side to stop if needed
        isLeftHipActive = false;
        isRightHipActive = false;
    }

    private void OnLeftHip(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isFishingInProgress)
        {
            CastHook(); 
        }
        else 
        {
            LeftMotionMovement(ctx);
        }
    }

    private void OnRightHip(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isFishingInProgress)
        {
            CastHook(); 
        }
        else 
        {
            RightMotionMovement(ctx);
        }
    }

    // private void Move()
    // {
    //     moveInput = moveAction.ReadValue<Vector2>(); // Read movement input

    //     float movementBoost = isHookMoving ? 1.2f : 1f; // Boost speed when hook is moving
    //     float targetSpeed = moveInput.x * moveSpeed * movementBoost;
    //     float dampenFactor = moveInput.x == 0 ? 0.9f : 1f;
    //     float newSpeed = Mathf.Lerp(
    //         rb.velocity.x * dampenFactor,
    //         targetSpeed,
    //         Time.fixedDeltaTime * (moveInput.x != 0 ? acceleration : deceleration)
    //     );
    //     rb.velocity = new Vector2(newSpeed, rb.velocity.y);

    //     // Apply tilt 
    //     float targetTilt = moveInput.x * tilt;
    //     currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.fixedDeltaTime * 5f);
    //     rb.MoveRotation(Quaternion.Euler(0, 0, -currentTilt));
    // }

    private void HandleMotionMovement()
    {
        motionInputX = 0f;
        if (isLeftHipActive) motionInputX -= 1f;
        if (isRightHipActive) motionInputX += 1f;
        
        float targetSpeed = motionInputX * moveSpeed;
        float newSpeed = Mathf.Lerp(
            rb.velocity.x,
            targetSpeed,
            Time.fixedDeltaTime * (Mathf.Abs(motionInputX) > 0.01f ? acceleration : deceleration)
        );
        rb.velocity = new Vector2(newSpeed, rb.velocity.y);

        // Apply tilt
        float targetTilt = motionInputX * tilt;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.fixedDeltaTime * 5f);
        rb.MoveRotation(Quaternion.Euler(0, 0, -currentTilt));
    }
    #endregion

    public void DisablePlayerController()
    {
        rb.isKinematic = true; 
        rb.velocity = Vector2.zero; 
    }

    public void EnablePlayerController()
    {
        rb.isKinematic = false; 
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