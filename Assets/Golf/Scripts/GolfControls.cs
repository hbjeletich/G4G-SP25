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

    // Start is called before the first frame update
    void Start()
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
       
}
