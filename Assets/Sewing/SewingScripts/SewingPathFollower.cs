using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sewing;

namespace Sewing{
    

public class SewingPathFollower : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    public List<Transform> waypoints = new List<Transform>();          // Waypoints set in the Inspector
    public float moveSpeed = 2f;               // Speed of movement
    private Animator animator;
    private int currentIndex = 0;
    private bool isMoving = false;
    private InputAction footRaiseAction;

    void Awake()
    {   
        animator = GetComponent<Animator>();
        var actionMap = inputActions.FindActionMap("MotionTracking");
        footRaiseAction = actionMap.FindAction("FootRaise");
        footRaiseAction.performed += OnFootRaise;
    }

    private void OnEnable()
    {
        footRaiseAction.Enable();
    }

    private void OnDisable()
    {
        footRaiseAction.Disable();
    }
    private void OnFootRaise(InputAction.CallbackContext ctx)
    {
        animator.SetTrigger("PlayAnimation");
        if (waypoints.Count == 0) return;

        int nextIndex = currentIndex + 1;
        if (nextIndex >= waypoints.Count)
        {
            return; // Reached end, don't continue
        }

        StartCoroutine(MoveToWaypoint(waypoints[nextIndex].position));
        currentIndex = nextIndex;
    }

    System.Collections.IEnumerator MoveToWaypoint(Vector3 targetPos)
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}
}