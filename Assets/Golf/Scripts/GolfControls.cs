using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GolfControls : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    private InputAction footRaiseAction, footLowerAction, leftFootHeightAction, rightFootHeightAction;

    public bool hasFootRaise = false;
    private float totalFootHeight = 0f;
    private int footHeightSamples = 0;

    //consider using footheight

    private void Awake()
    {
        hasFootRaise = false;

        var actionMap = inputActions.FindActionMap("Foot");
        footRaiseAction = actionMap.FindAction("FootRaised");
        footLowerAction = actionMap.FindAction("FootLowered");
        leftFootHeightAction = actionMap.FindAction("LeftFootPosition");
        rightFootHeightAction = actionMap.FindAction("RightFootPosition");

        footRaiseAction.performed += startStrengthAssesment;
        footLowerAction.performed += stopStrengthAssesment;
    }

    private void Update()
    {
        if (hasFootRaise)
        {
            float leftFootY = leftFootHeightAction.ReadValue<Vector3>().y;
            float rightFootY = rightFootHeightAction.ReadValue<Vector3>().y;
            float footHeight = Mathf.Max(leftFootY, rightFootY);
            totalFootHeight += footHeight;
            footHeightSamples++;
        }
    }

    private void OnEnable()
    {
        footRaiseAction.Enable();
        footLowerAction.Enable();
        leftFootHeightAction.Enable();
        rightFootHeightAction.Enable();
    }

    private void OnDisable()
    {
        footRaiseAction.Disable();
        footLowerAction.Disable();
        leftFootHeightAction.Disable();
        rightFootHeightAction.Disable();
    }

    public void startStrengthAssesment(InputAction.CallbackContext ctx){
        float leftFootY = leftFootHeightAction.ReadValue<Vector3>().y;
        float rightFootY = rightFootHeightAction.ReadValue<Vector3>().y;
        string dataStr = $"Foot: {(leftFootY > rightFootY ? "Left" : "Right")}";
        DataLogger.Instance.LogData("FootRaise", dataStr);
            
        hasFootRaise = true;
    }

    public void stopStrengthAssesment(InputAction.CallbackContext ctx){
        float averageFootHeight = footHeightSamples > 0 ? totalFootHeight / footHeightSamples : 0f;
        DataLogger.Instance.LogData("FootLower", $"AverageFootHeight: {averageFootHeight.ToString("F2")}; TimeRaised: {(footHeightSamples * Time.deltaTime).ToString("F2")}s");
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
