using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsTestPlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float moveSpeed = 3f;

    private InputAction footRaiseAction;
    private InputAction footHeightAction;
    private Rigidbody2D rigidbody2D;
    private bool isGrounded;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        var actionMap = inputActions.FindActionMap("MotionTracking");
        footRaiseAction = actionMap.FindAction("FootRaise");
        footHeightAction = actionMap.FindAction("FootHeight");

        footRaiseAction.performed += OnFootRaise;
    }

    private void OnEnable()
    {
        footRaiseAction.Enable();
        footHeightAction.Enable();
    }

    private void OnDisable()
    {
        footRaiseAction.Disable();
        footHeightAction.Disable();
    }

    private void Update()
    {
        // get foot height value and use it for horizontal movement
        float footHeight = footHeightAction.ReadValue<float>();
        float horizontalMove = Mathf.Clamp(footHeight * 2, 0, 1) * moveSpeed;

        rigidbody2D.velocity = new Vector2(horizontalMove, rigidbody2D.velocity.y);
    }

    private void OnFootRaise(InputAction.CallbackContext ctx)
    {
        // jump when foot is raised, but only if on ground
        if (isGrounded)
        {
            rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}