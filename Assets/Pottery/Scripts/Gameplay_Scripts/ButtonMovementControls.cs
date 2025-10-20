using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

//public class ButtonMovementControls : MonoBehaviour
//{
//    private Button myButton;

//    public InputActionAsset inputActionAsset;
//    private InputAction leftFootHeightAction;
    



//    // Start is called before the first frame update
//    void Start()
//    {
//        myButton = GetComponent<Button>();

//        myButton.onClick.AddListener(OnButtonClick);

//        var footMap = inputActions.FindActionMap("Foot");
//        leftFootHeightAction = footMap.FindAction("LeftFootPosition");
//    }

//    void OnEnable()
//    {
//        leftFootHeightAction.Enable();
//    }

//    void OnDisable()
//    {
//        leftFootHeightAction.Disable();
//    }

//        // Update is called once per frame
//    void Update()
//    {
//        float leftFootY = leftFootHeightAction.ReadValue<Vector3>().y;

//        if(leftFootY > .5)
//        {
//            OnButtonClick();
//        }
//    }

//    void OnButtonClick()
//    {
//        Debug.Log("Button was clicked!");
//    }
//}
