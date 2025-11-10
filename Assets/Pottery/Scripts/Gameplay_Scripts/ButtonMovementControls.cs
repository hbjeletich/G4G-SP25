using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ButtonMovementControls : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    private InputAction leftFootHeightAction;
    private InputAction leftHipAbducted;
    private InputAction rightHipAbducted;
    private InputAction leftFootRaised;

    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject SelectionPanel;

    // Start is called before the first frame update
    void Awake()
    {
        var footMap = inputActionAsset.FindActionMap("Foot"); //plugin unity captury 

        if (footMap == null) Debug.LogWarning("Pottery: Foot map not found!");
        if (footMap != null) Debug.Log("Pottery: Foot map found!");
        //var footMap = inputActions.FindActionMap("Foot");



        if (leftHipAbducted == null) Debug.LogWarning("Pottery: Left Hip Abducted not found!");
        if (leftHipAbducted != null) Debug.Log("Pottery: Left Hip Abducted found!");

        if (rightHipAbducted == null) Debug.LogWarning("Pottery: right Hip Abducted not found!");
        if (rightHipAbducted != null) Debug.Log("Pottery: right Hip Abducted found!");

        if (leftFootRaised == null) Debug.LogWarning("Pottery: Left foot raised not found!");
        if (leftFootRaised != null) Debug.Log("Pottery: left foot raised found!");


        leftHipAbducted = footMap.FindAction("LeftHipAbducted");
        rightHipAbducted = footMap.FindAction("RightHipAbducted");
        leftFootRaised = footMap.FindAction("FootRaised");
    }

    void OnEnable()
    {
        leftHipAbducted.Enable();
        Debug.Log("Pottery: OnEnable");
        leftHipAbducted.performed += OnLeftHip;


        rightHipAbducted.Enable();
        Debug.Log("Pottery: OnEnable");
        rightHipAbducted.performed += OnRightHip;

        leftFootRaised.Enable();
;       Debug.Log("Pottery; OnEnable");
        leftFootRaised.performed += onLeftFoot;

    }

    void OnDisable()
    {
        leftHipAbducted.Disable();
        Debug.Log("Potteru: OnDisable");
        leftHipAbducted.performed -= OnLeftHip;
    }


    public void ShowMainMenu()
    {
        SetActivePanel(mainMenuPanel);
    }

    public void ShowSettings()
    {
        SetActivePanel(settingsPanel);
    }


    public void ShowSelection()
    {
        SetActivePanel(SelectionPanel);
    }



    ////// Set active panel below////////

    private void SetActivePanel(GameObject activePanel)
    {

        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        SelectionPanel.SetActive(false);


        activePanel.SetActive(true);
    }



    void OnRightHip(InputAction.CallbackContext context)
    {

        Debug.Log("Pottery: OnRightHip");
        ShowMainMenu();

    }

    void OnLeftHip(InputAction.CallbackContext context)
    {
        Debug.Log("Pottery: OnLeftHip");
        // whatever you write here happens when left hip abducted
        ShowSettings();
    }


    void onLeftFoot (InputAction.CallbackContext context)
    {

        Debug.Log("Left Foot raised");
        ShowSelection();

    }
}
