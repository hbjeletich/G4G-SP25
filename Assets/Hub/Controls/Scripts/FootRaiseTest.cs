using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FootRaiseTest : MonoBehaviour
{
    public InputAction footHeightAction;
    public InputAction footRaiseAction;
    public InputAction footLowerAction;

    private void OnEnable()
    {
        footHeightAction.Enable();
        footRaiseAction.Enable();
        footLowerAction.Enable();
    }

    private void OnDisable()
    {
        footHeightAction.Disable();
        footRaiseAction.Disable();
        footLowerAction.Disable();
    }

    private void Update()
    {
        if(footRaiseAction.triggered)
        {
            Debug.Log("foot raised!");
        }

        if (footLowerAction.triggered)
        {
            Debug.Log("foot lowered!");
        }
    }
}
