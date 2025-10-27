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
    void Awake()
    {
        var footMap = inputActionAsset.FindActionMap("Foot"); //plugin unity captury 
        if (footMap == null) Debug.LogWarning("Pottery: Foot map not found!");
        if (footMap != null) Debug.Log("Pottery: Foot map found!");
        //var footMap = inputActions.FindActionMap("Foot");

        leftHipAbducted = footMap.FindAction("LeftHipAbducted");
        if (leftHipAbducted == null) Debug.LogWarning("Pottery: Left Hip Abducted not found!");
        if (leftHipAbducted != null) Debug.Log("Pottery: Left Hip Abducted found!");
    }

    void OnEnable()
    {
        leftHipAbducted.Enable();
        Debug.Log("Pottery: OnEnable");
        leftHipAbducted.performed += OnLeftHip;
    }

    void OnDisable()
    {
        leftHipAbducted.Disable();
        Debug.Log("Potteru: OnDisable");
        leftHipAbducted.performed -= OnLeftHip;
    }

    // Update is called once per frame

    void Update()
    {

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
    }
}
