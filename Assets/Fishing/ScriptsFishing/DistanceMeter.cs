using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DistanceMeter : MonoBehaviour
{
    [Header("Distance Meter Settings")]
    public Slider distanceSlider;  
    public RectTransform marker;
    public float barSpeed = 0.5f;  

    private float timeCounter = 0f;
    [HideInInspector] public bool isFishing = false;  
    private PlayerInput playerInput;
    private InputAction fishAction;
    private float selectedDistance = 0f;
    private bool isMeterActive = false;

    [Header("Debug Settings")]
    public Button debugButton; 

    private void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found in the scene.");
            return;
        }
        if (playerInput != null)
        {
            fishAction = playerInput.actions["Fish"];
        }

        if (debugButton != null)
        {
            debugButton.onClick.AddListener(ToggleFishingDebug);
        }
    }

    private void OnEnable()
    {
        if (fishAction != null)
        {
            fishAction.Enable();
            fishAction.performed += OnFishPressed;
            fishAction.canceled += OnFishReleased;
        }
    }

    private void OnDisable()
    {
        if (fishAction != null)
        {
            fishAction.Disable();
            fishAction.performed -= OnFishPressed;
            fishAction.canceled -= OnFishReleased;
        }
    }

   private void Update()
    {
        if (!isFishing) return;

        timeCounter += Time.deltaTime * barSpeed;
        distanceSlider.value = Mathf.PingPong(timeCounter, 1f);

        UpdateMarkerPosition();
    }

    private void UpdateMarkerPosition()
    {
        if (marker == null || distanceSlider == null) return;

        float sliderWidth = ((RectTransform)distanceSlider.fillRect.parent).rect.width;
        Vector2 newPos = marker.anchoredPosition;
        newPos.x = distanceSlider.value * sliderWidth;
        marker.anchoredPosition = newPos;
    }

     private void OnFishPressed(InputAction.CallbackContext context)
    {
        if (!isMeterActive)
        {
            isMeterActive = true;
            RestartMeter();
            Debug.Log("Meter started.");
        }
        else
        {
            isFishing = true;
            Debug.Log("Started fishing action (charging).");
        }
    }

    private void OnFishReleased(InputAction.CallbackContext context)
    {
        if (!isMeterActive || !isFishing)
        {
            Debug.Log("FishReleased ignored: meter not active or not fishing.");
            return;
        }
        isFishing = false;
        isMeterActive = false;
        Debug.Log("Fishing action released.");

        selectedDistance = distanceSlider.value;

        FishingPlayerController playerController = FindObjectOfType<FishingPlayerController>();
        if (playerController != null)
        {
            var selectedZone = playerController.DetermineFishingZone(selectedDistance);
            playerController.CastHook(selectedZone);
            Debug.Log("Selected distance: " + selectedDistance);
        }
    }

    public void RestartMeter()
    {
        selectedDistance = 0f;
        distanceSlider.value = 0f;
        isFishing = true;
        timeCounter = 0f;
    }

    private void ToggleFishingDebug()
    {
        isFishing = !isFishing;
    }
}
