using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DistanceMeter : MonoBehaviour
{
    [Header("Distance Meter Settings")]
    public Slider distanceSlider;  
    public Image fillImage;        
    public Gradient barColors;   
    public float barSpeed = 0.5f;  

    private bool movingRight = true;
    private bool isFishing = false;  
    private PlayerInput playerInput;
    private InputAction fishAction;
    private float selectedDistance = 0f;

    [Header("Debug Settings")]
    public Button debugButton; 

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            fishAction = playerInput.actions["Fish"];
        }

        if (fillImage != null)
        {
            fillImage.color = barColors.Evaluate(distanceSlider.value);
        }

        // Debug Button Setup
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
        if (!isFishing) return;  // Stop updating if not fishing

        // Move the slider smoothly
        float newValue = distanceSlider.value;

        if (movingRight)
        {
            newValue += barSpeed * Time.deltaTime;
            if (newValue >= 1f) // Ensure it reaches max
            {
                newValue = 1f;
                movingRight = false;
            }
        }
        else
        {
            newValue -= barSpeed * Time.deltaTime;
            if (newValue <= 0f) // Ensure it reaches min
            {
                newValue = 0f;
                movingRight = true;
            }
        }

        distanceSlider.value = Mathf.Clamp01(newValue);

        if (fillImage != null)
        {
            fillImage.color = barColors.Evaluate(newValue);
        }
    }


    private void OnFishPressed(InputAction.CallbackContext context)
    {
        isFishing = true;
    }

    private void OnFishReleased(InputAction.CallbackContext context)
    {
        isFishing = false;
        selectedDistance = distanceSlider.value; 
        float castDistance = Mathf.Lerp(5f, 30f, selectedDistance);

        FindObjectOfType<FishingPlayerController>().CastHook(castDistance);
    }

    private void ToggleFishingDebug()
    {
        isFishing = !isFishing;
    }
}
