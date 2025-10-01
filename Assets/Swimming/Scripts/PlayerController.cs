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
        [SerializeField] private float sinkForce = 15f; // squatting
        [SerializeField] private float maxSinkVelocity = 3f;
        [SerializeField] private float waterDrag = 1.5f;

        [Header("Continuous Motion Settings")]
        [SerializeField] private float footHeightForce = 10f;
        [SerializeField] private float squatForce = 12f;
        [SerializeField] private float continuousMotionThreshold = 0.1f;

        private bool isSwimming = false;
        private bool isSinking = false;

        // debug mode for using keyboard input
        [SerializeField] private bool debugMode = false;

        // input actions
        private InputAction weightShiftXAction;
        private InputAction leftFootHeightAction;
        private InputAction rightFootHeightAction;
        private InputAction squatTrackingYAction;

        // debug input actions
        [SerializeField] private InputActionAsset debugActions;
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction crouchAction;

        private Rigidbody2D rigidbody2D;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        [SerializeField] private RuntimeAnimatorController regularAnimator;
        [SerializeField] private RuntimeAnimatorController deepSeaAnimator;

        private bool isFacingRight = true;
        [SerializeField] private GameObject rightColliders;
        [SerializeField] private GameObject leftColliders;

        [SerializeField] private Transform deepSeaCutoff;
        private float deepSeaYLevel;

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.drag = waterDrag; // for underwater feel
            rigidbody2D.gravityScale = 0f;

            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = regularAnimator;
            }

            // set up motion tracking actions
            var torsoMap = inputActions.FindActionMap("Torso");
            var footMap = inputActions.FindActionMap("Foot");
            weightShiftXAction = torsoMap.FindAction("WeightShiftX");
            leftFootHeightAction = footMap.FindAction("LeftFootPosition");
            rightFootHeightAction = footMap.FindAction("RightFootPosition");
            squatTrackingYAction = torsoMap.FindAction("PelvisPosition");

            // set up keyboard input for debug mode
            var playerMap = debugActions.FindActionMap("SwimmingDebug");

            if (playerMap == null) Debug.LogError("player map not found");
            moveAction = playerMap.FindAction("Move");
            jumpAction = playerMap.FindAction("Jump");
            crouchAction = playerMap.FindAction("Crouch");

            deepSeaYLevel = deepSeaCutoff.position.y;
        }

        private void OnEnable()
        {
            weightShiftXAction.Enable();
            leftFootHeightAction.Enable();
            rightFootHeightAction.Enable();
            squatTrackingYAction.Enable();

            moveAction.Enable();
            jumpAction.Enable();
            crouchAction.Enable();

            // debug callbacks
            jumpAction.started += _ => StartSwimming();
            jumpAction.canceled += _ => StopSwimming();
            crouchAction.started += _ => StartSinking();
            crouchAction.canceled += _ => StopSinking();
        }

        private void OnDisable()
        {
            weightShiftXAction.Disable();
            leftFootHeightAction.Disable();
            rightFootHeightAction.Disable();
            squatTrackingYAction.Disable();

            moveAction.Disable();
            jumpAction.Disable();
            crouchAction.Disable();

            // debug
            jumpAction.started -= _ => StartSwimming();
            jumpAction.canceled -= _ => StopSwimming();
            crouchAction.started -= _ => StartSinking();
            crouchAction.canceled -= _ => StopSinking();
        }

        private void FixedUpdate()
        {
            if (debugMode)
            {
                // keyboard input in debug mode
                float horizontalInput = moveAction.ReadValue<Vector2>().x;
                DoHorizontalMovement(horizontalInput);

                float verticalInput = moveAction.ReadValue<Vector2>().y;
                DoContinuousVerticalMovement(verticalInput);
            }
            else
            {
                // motion tracking input when not in debug mode
                float weightShift = weightShiftXAction.ReadValue<float>();
                DoHorizontalMovement(weightShift);

                HandleContinuousVerticalMovement();
            }

            if (transform.position.y <= deepSeaYLevel)
            {
                ChangeToDeepSea();
            }
            else
            {
                ChangeToRegularSea();
            }

            animator.SetFloat("xVel", rigidbody2D.velocity.x);
            animator.SetFloat("yVel", rigidbody2D.velocity.y);
        }

        private void HandleContinuousVerticalMovement()
        {
            float leftFootY = leftFootHeightAction.ReadValue<Vector3>().y;
            float rightFootY = rightFootHeightAction.ReadValue<Vector3>().y;
            float pelvisY = squatTrackingYAction.ReadValue<Vector3>().y;

            float netVerticalForce = 0f;

            // use whichever is higher
            float footHeight = Mathf.Max(leftFootY, rightFootY);

            // 0 = no lift, 1 = maximum lift
            if (footHeight > continuousMotionThreshold)
            {
                netVerticalForce += footHeight * footHeightForce;
            }

            // 0 = standing, 1 = deep squat
            if (pelvisY > continuousMotionThreshold)
            {
                netVerticalForce -= pelvisY * squatForce;
            }

            DoContinuousVerticalMovement(netVerticalForce);
        }

        private void DoContinuousVerticalMovement(float verticalForce)
        {
            if (Mathf.Abs(verticalForce) < 0.01f) return;

            if (verticalForce > 0) // upward movement
            {
                if (rigidbody2D.velocity.y < maxSwimVelocity)
                {
                    rigidbody2D.AddForce(Vector2.up * verticalForce, ForceMode2D.Force);
                }
            }
            else // downward movement
            {
                if (rigidbody2D.velocity.y > -maxSinkVelocity)
                {
                    rigidbody2D.AddForce(Vector2.up * verticalForce, ForceMode2D.Force);
                }
            }
        }

        private void DoHorizontalMovement(float _input)
        {
            float verticalVelocity = rigidbody2D.velocity.y;
            rigidbody2D.velocity = new Vector2(_input * moveSpeed, verticalVelocity);
        }

        private void StartSwimming()
        {
            Debug.Log("Swimming up!");
            isSwimming = true;
        }

        private void StopSwimming()
        {
            isSwimming = false;
        }

        private void StartSinking()
        {
            Debug.Log("Sinking down!");
            isSinking = true;
        }

        private void StopSinking()
        {
            isSinking = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Collectable collectable = collision.gameObject.GetComponent<Collectable>();
            if (collectable != null)
            {
                collectable.OnPickup();
            }
        }

        private void ChangeToDeepSea()
        {
            animator.runtimeAnimatorController = deepSeaAnimator;
        }

        private void ChangeToRegularSea()
        {
            animator.runtimeAnimatorController = regularAnimator;
        }
    }
}