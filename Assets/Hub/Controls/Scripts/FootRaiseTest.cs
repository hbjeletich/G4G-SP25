using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FootRaiseTest : MonoBehaviour
{
    public InputActionAsset inputActions; // Assigned in the inspector
    private InputAction footRaise;
    private InputAction footLower;
    private InputAction footHeight;

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("FootRaiseTest: InputActionAsset is not assigned! Check the Inspector.");
            return;
        }

        var footTrackingMap = inputActions.FindActionMap("MotionTracking");

        if (footTrackingMap == null)
        {
            Debug.LogError("FootRaiseTest: Could not find Action Map 'MotionTracking'. Check the InputActionAsset.");
            return;
        }
        Debug.Log("FootRaiseTest: Found Action Map 'MotionTracking'.");

        footRaise = footTrackingMap.FindAction("FootRaise");
        footLower = footTrackingMap.FindAction("FootLower");
        footHeight = footTrackingMap.FindAction("FootHeight");

        if (footRaise == null)
            Debug.LogError("FootRaiseTest: Could not find Action 'FootRaise'. Check the InputActionAsset.");
        else
            Debug.Log("FootRaiseTest: Found Action 'FootRaise'.");

        if (footLower == null)
            Debug.LogError("FootRaiseTest: Could not find Action 'FootLower'. Check the InputActionAsset.");
        else
            Debug.Log("FootRaiseTest: Found Action 'FootLower'.");

        if (footHeight == null)
            Debug.LogError("FootRaiseTest: Could not find Action 'FootHeight'. Check the InputActionAsset.");
        else
            Debug.Log("FootRaiseTest: Found Action 'FootHeight'.");

        if (footRaise != null)
            footRaise.performed += OnFootRaise;

        if (footLower != null)
            footLower.performed += OnFootLower;

        if (footHeight != null)
            footHeight.performed += OnFootHeight;
    }

    private void OnFootRaise(InputAction.CallbackContext ctx)
    {
        Debug.Log("Input Action: Foot Raised!");
    }

    private void OnFootLower(InputAction.CallbackContext ctx)
    {
        Debug.Log("Input Action: Foot Lowered!");
    }

    private void OnFootHeight(InputAction.CallbackContext ctx)
    {
        float height = ctx.ReadValue<float>();
        Debug.Log("Input Action: Foot Height - " + height);
    }

    private void OnEnable()
    {
        if (footRaise != null) footRaise.Enable();
        if (footLower != null) footLower.Enable();
        if (footHeight != null) footHeight.Enable();
    }

    private void OnDisable()
    {
        if (footRaise != null) footRaise.Disable();
        if (footLower != null) footLower.Disable();
        if (footHeight != null) footHeight.Disable();
    }
}

