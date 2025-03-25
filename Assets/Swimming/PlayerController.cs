using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Swimming {
    public class PlayerController : MonoBehaviour
    {
        // serialized values
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private float moveSpeed = 3f;

        // input actons
        private InputAction weightShiftLeftAction;
        private InputAction weightShiftRightAction;
        private InputAction weightShiftXAction;

        private Rigidbody2D rigidbody2D;

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();

            var actionMap = inputActions.FindActionMap("MotionTracking");
            weightShiftLeftAction = actionMap.FindAction("WeightShiftLeft");
            weightShiftRightAction = actionMap.FindAction("WeightShiftRight");
            weightShiftXAction = actionMap.FindAction("WeightShiftX");
        }

        private void OnEnable()
        {
            weightShiftLeftAction.Enable();
            weightShiftRightAction.Enable();
            weightShiftXAction.Enable();
        }

        private void OnDisable()
        {
            weightShiftLeftAction.Disable();
            weightShiftRightAction.Disable();
            weightShiftXAction.Disable();
        }

        private void Update()
        {
            // get foot height value and use it for horizontal movement
            float weightShift = weightShiftXAction.ReadValue<float>();

            rigidbody2D.velocity = new Vector2(weightShift * moveSpeed, 0);
        }
    }

}