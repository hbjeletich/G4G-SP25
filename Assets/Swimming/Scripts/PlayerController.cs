using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Swimming
{
    public class PlayerController : MonoBehaviour
    {
        // serialized values
        [SerializeField] private InputActionAsset inputActions; // NEED THIS!!
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

        // input actions - NEED THIS
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

        [Header("Tutorial")]
        [SerializeField] private float tutorialInputThreshold = 0.3f;

        private bool tutorialComplete = false;

        private void Awake()
        {
            if(DataLogger.Instance != null)
                DataLogger.Instance.LogMinigameEvent("ScubaScavenge", "Player started", Time.time.ToString("F2"));

            rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.drag = waterDrag; // for underwater feel
            rigidbody2D.gravityScale = 0f;

            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = regularAnimator;
            }

            // THIS IS HOW YOU SET UP ACTIONS IN AWAKE!
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

            StartCoroutine(RunMovementTutorial());
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

                if(isSwimming)
                {
                    DoContinuousVerticalMovement(swimForce);
                }
                else if(isSinking)
                {
                    DoContinuousVerticalMovement(-sinkForce);
                }

                // float verticalInput = moveAction.ReadValue<Vector2>().y;
                // float verticalForce = verticalInput * footHeightForce;  // or swimForce
                // DoContinuousVerticalMovement(verticalForce);
            }
            else
            {
                // motion tracking input when not in debug mode
                float weightShift = weightShiftXAction.ReadValue<float>();
                if(Mathf.Abs(weightShift) > 0.1f)
                {
                    string dataStr = $"WeightShiftX: {weightShift:F2}; Shifting: {((weightShift > 0) ? "Right" : "Left")}";
                    if(DataLogger.Instance != null) DataLogger.Instance.LogInput("WeightShiftX", weightShift.ToString("F2"));
                }
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

            Debug.Log($"Left Foot Height: {leftFootY}, Right Foot Height: {rightFootY}, Pelvis Y: {pelvisY}");

            float netVerticalForce = 0f;

            // use whichever is higher
            float footHeight = Mathf.Max(leftFootY, rightFootY);

            // 0 = no lift, 1 = maximum lift
            if (footHeight > continuousMotionThreshold)
            {
                string dataSrt = $"FootHeight: {footHeight:F2}; Foot: {(leftFootY > rightFootY ? "Left" : "Right")}";
                if(DataLogger.Instance != null)
                {
                    DataLogger.Instance.LogInput("FootHeight", footHeight.ToString("F2"));
                }
                netVerticalForce += footHeight * footHeightForce;
            }

            // 0 = standing, -1 = deep squat
            if (-pelvisY > continuousMotionThreshold/2)
            {
                if(DataLogger.Instance != null)
                {
                    DataLogger.Instance.LogInput("PelvisSquat", pelvisY.ToString("F2"));
                }
                netVerticalForce = pelvisY * squatForce;
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

        private IEnumerator RunMovementTutorial()
        {
            // Brief pause before tutorial starts
            yield return new WaitForSeconds(1f);

            // Step 1: Weight Shift (left/right)
            G4G.ExerciseIndicatorManager.Instance?.Show(ExerciseType.WeightShift);
            yield return WaitForInput(() =>
            {
                float ws = weightShiftXAction.ReadValue<float>();
                float kb = debugMode ? moveAction.ReadValue<Vector2>().x : 0f;
                return Mathf.Abs(ws) > tutorialInputThreshold || Mathf.Abs(kb) > 0.1f;
            });

            // Step 2: Leg Lift (swim up)
            G4G.ExerciseIndicatorManager.Instance?.Show(ExerciseType.LegLift);
            yield return WaitForInput(() =>
            {
                float leftY = leftFootHeightAction.ReadValue<Vector3>().y;
                float rightY = rightFootHeightAction.ReadValue<Vector3>().y;
                float foot = Mathf.Max(leftY, rightY);
                return foot > tutorialInputThreshold || (debugMode && isSwimming);
            });

            // Step 3: Squat (sink down)
            G4G.ExerciseIndicatorManager.Instance?.Show(ExerciseType.Squat);
            yield return WaitForInput(() =>
            {
                float pelvisY = squatTrackingYAction.ReadValue<Vector3>().y;
                return -pelvisY > tutorialInputThreshold / 2f || (debugMode && isSinking);
            });

            // Tutorial complete
            tutorialComplete = true;
            G4G.ExerciseIndicatorManager.Instance?.Hide();
        }

        private IEnumerator WaitForInput(System.Func<bool> condition)
        {
            // Small grace period so they see the indicator before we start checking
            yield return new WaitForSeconds(0.5f);
            while (!condition())
            {
                yield return null;
            }
            // Brief pause after success so it doesn't feel instant
            yield return new WaitForSeconds(0.8f);
        }
    }
}