using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GolfControls : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    private InputAction footRaiseAction;
    private InputAction footLowerAction;

    public bool hasFootRaise = false;

    //consider using footheight

    private void Awake()
    {
        hasFootRaise = false;

        var actionMap = inputActions.FindActionMap("MotionTracking");
        footRaiseAction = actionMap.FindAction("FootRaise");
        footLowerAction = actionMap.FindAction("FootLower");

        footRaiseAction.performed += startStrengthAssesment;
        footLowerAction.performed += stopStrengthAssesment;
    }

    private void OnEnable()
    {
        footRaiseAction.Enable();
        footLowerAction.Enable();
    }

    private void OnDisable()
    {
        footRaiseAction.Disable();
        footLowerAction.Disable();
    }

    public void startStrengthAssesment(InputAction.CallbackContext ctx){
        hasFootRaise = true;
    }

    public void stopStrengthAssesment(InputAction.CallbackContext ctx){
        hasFootRaise = false;
    }
       
    public bool simulatedLegRaise(){
        if(Input.GetMouseButtonDown(0)){
            hasFootRaise = true;
            return true;
        }
        return false;
    }

    public bool simulatedLegLower(){
        if(Input.GetMouseButtonDown(0)){
            hasFootRaise = false;
            return true;
        }
        return false;
    }
}
