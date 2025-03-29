using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Swimming
{
    public class PlayerController : MonoBehaviour
    {
        // serialized values
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float swimForce = 20f;
        [SerializeField] private float maxSwimVelocity = 4f;
        [SerializeField] private float waterDrag = 1.5f;
        private bool isSwimming = false;

        // debug mode for using keyboard input
        [SerializeField] private bool debugMode = false; 

        // input actions
        private InputAction weightShiftLeftAction;
        private InputAction weightShiftRightAction;
        private InputAction weightShiftXAction;
        private InputAction raiseFootAction;
        private InputAction lowerFootAction;

        // debug input actions
        [SerializeField] private InputActionAsset debugActions;
        private InputAction moveAction;
        private InputAction jumpAction;

        private Rigidbody2D rigidbody2D;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private bool isFacingRight = true;
        [SerializeField] private GameObject rightColliders;
        [SerializeField] private GameObject leftColliders;

        private void Awake()
        {        
            rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.drag = waterDrag; // for underwater feel

            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            // set up motion tracking actions
            var motionMap = inputActions.FindActionMap("MotionTracking");
            weightShiftLeftAction = motionMap.FindAction("WeightShiftLeft");
            weightShiftRightAction = motionMap.FindAction("WeightShiftRight");
            weightShiftXAction = motionMap.FindAction("WeightShiftX");
            raiseFootAction = motionMap.FindAction("FootRaise");
            lowerFootAction = motionMap.FindAction("FootLower");

            // set up keyboard input for debug mode
            var playerMap = debugActions.FindActionMap("SwimmingDebug");

            if (playerMap == null) Debug.LogError("player map not found");
            moveAction = playerMap.FindAction("Move");
            jumpAction = playerMap.FindAction("Jump");
        }

        private void OnEnable()
        {
            weightShiftLeftAction.Enable();
            weightShiftRightAction.Enable();
            weightShiftXAction.Enable();

            moveAction.Enable();
            jumpAction.Enable();

            // upward movement callbacks
            raiseFootAction.performed += _ => StartSwimming();
            lowerFootAction.performed += _ => StopSwimming();

            jumpAction.started += _ => StartSwimming();
            jumpAction.canceled += _ => StopSwimming();
        }

        private void OnDisable()
        {
            weightShiftLeftAction.Disable();
            weightShiftRightAction.Disable();
            weightShiftXAction.Disable();

            moveAction.Disable();
            jumpAction.Disable();

            // upward movement
            raiseFootAction.performed -= _ => StartSwimming();
            lowerFootAction.performed -= _ => StopSwimming();

            jumpAction.started -= _ => StartSwimming();
            jumpAction.canceled -= _ => StopSwimming();
        }

        private void FixedUpdate()
        {
            if (debugMode)
            {
                // keyboard input in debug mode
                float horizontalInput = moveAction.ReadValue<Vector2>().x;
                DoHorizontalMovement(horizontalInput);
            }
            else
            {
                // motion tracking input when not in debug mode
                float weightShift = weightShiftXAction.ReadValue<float>();
                DoHorizontalMovement(weightShift);
            }

            if (isSwimming)
            {
                DoSwimming();
            }

            animator.SetFloat("xVel", rigidbody2D.velocity.x);
            Debug.Log(rigidbody2D.velocity.x);
            animator.SetFloat("yVel", rigidbody2D.velocity.y);
        }

        private void DoHorizontalMovement(float _input)
        {
            float verticalVelocity = rigidbody2D.velocity.y;
            rigidbody2D.velocity = new Vector2(_input * moveSpeed, verticalVelocity);
        }

        private void DoSwimming()
        {
            if (rigidbody2D.velocity.y < maxSwimVelocity)
            {
                rigidbody2D.AddForce(Vector2.up * swimForce, ForceMode2D.Force);
            }
        }

        private void StartSwimming()
        {
            isSwimming = true;
        }

        private void StopSwimming()
        {
            isSwimming = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Collectable collectable = collision.gameObject.GetComponent<Collectable>();
            if (collectable != null)
            {
                collectable.OnPickup();
            }
        }
    }
}