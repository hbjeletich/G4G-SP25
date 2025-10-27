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

    public GameObject settingsPanel;

    // Start is called before the first frame update
    void Start()
    {
        var footMap = inputActionAsset.FindActionMap("Foot"); //plugin unity captury 
        if (footMap == null) Debug.LogWarning("Foot map not found!");
        //var footMap = inputActions.FindActionMap("Foot");
        leftFootHeightAction = footMap.FindAction("LeftFootPosition");
        leftHipAbducted = footMap.FindAction("LeftHipAbducted");
    }

    void OnEnable()
    {
        if (leftFootHeightAction != null) leftFootHeightAction.Enable();
        
        if(leftHipAbducted != null) leftHipAbducted.performed += OnLeftHip;
    }

    void OnDisable()
    {
        if(leftFootHeightAction != null) leftFootHeightAction.Disable();

        if (leftHipAbducted != null)  leftHipAbducted.performed -= OnLeftHip;
    }

    // Update is called once per frame

    void Update()
    {
        if (leftFootHeightAction != null)
        {
            float leftFootY = leftFootHeightAction.ReadValue<Vector3>().y;

            if (leftFootY > .5)
            {
                OnButtonClick();
            }
        }
    }

    void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
    }


    public void ShowSettings()
    {
        SetActivePanel(settingsPanel);
    }

    private void SetActivePanel(GameObject activePanel)
    {
        settingsPanel.SetActive(false);

        activePanel.SetActive(true);
    }


    void OnLeftHip(InputAction.CallbackContext context)
    {
        Debug.Log("Pottery: OnLeftHip");
        // whatever you write here happens when left hip abducted
        ShowSettings();
        OnButtonClick();
    }
}
