using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sewing;
using System.Diagnostics;
using UnityEngine.SceneManagement;

namespace Sewing{
    

public class SewingPathFollower : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    public List<Transform> waypoints = new List<Transform>();          // Waypoints set in the Inspector
    public float moveSpeed = 2f;               // Speed of movement
    private int currentIndex = 0;
    private bool isMoving = false;
    private InputAction footRaiseAction, footLowerAction;
    private bool isFootRaised = false;
    public GameObject bobbinObject;
    public string triggerName = "PlayAnimation";

    public SewingSceneUIManager sewingSceneUIManager;

    void Awake()
    {   
        var actionMap = inputActions.FindActionMap("MotionTracking");
        footRaiseAction = actionMap.FindAction("FootRaise");
        footLowerAction = actionMap.FindAction("FootLower");
        footRaiseAction.performed += OnFootRaise;
        footLowerAction.performed += OnFootLower;
    }

    private void OnEnable()
    {
        footRaiseAction.Enable();
        footLowerAction.Enable();
    }

    private void OnDisable()
    {
        footRaiseAction.Disable();
        footLowerAction.Disable();
    }
    private void OnFootRaise(InputAction.CallbackContext ctx)
    {
        isFootRaised = true;

        if (currentIndex == waypoints.Count) {
            ChangeScene("5. Success");
    
    }
    }

    private void OnFootLower(InputAction.CallbackContext ctx)
    {
        isFootRaised = false;
    }

    private void NeedleMoving()
    {
            // added while loop -- only add a waypoint if not already moving
            while (!isMoving)
            {
                if (waypoints.Count == 0) return;
                if (currentIndex > waypoints.Count) return;

                int nextIndex = currentIndex + 1;
                if (nextIndex >= waypoints.Count)
                {
                    return; // Reached end, don't continue
                }

                // helena doing some testing!
                UnityEngine.Debug.Log($"Moving to waypoint: {nextIndex}");

                StartCoroutine(MoveToWaypoint(waypoints[nextIndex].position));
                currentIndex = nextIndex;
            }
    }

    System.Collections.IEnumerator MoveToWaypoint(Vector3 targetPos)
    {
         Animator targetAnimator = bobbinObject.GetComponent<Animator>();

                // Set the trigger parameter to true, triggering the animation
        targetAnimator.SetTrigger(triggerName);
        isMoving = true;
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        if (currentIndex == waypoints.Count)
        {
            sewingSceneUIManager.SewingShowCompletionUI();
        }
    }

    void Update()
    {
        if(isFootRaised)
        {
            NeedleMoving();
        }
    }
    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
}