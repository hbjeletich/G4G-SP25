using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Sewing;

namespace Sewing {
public class ScissorsMove : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    // List to hold waypoints (Empty GameObjects you set up in the scene)
    public List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0; // Tracks the current waypoint
    public int RepOne = 0;
    public int RepTwo = 0;
    private Animator animator;
    private bool movementAvailable = true;
    private InputAction leftHipAction;
    private InputAction rightHipAction;
    private InputAction footRaiseAction;

    public float moveSpeed = 3f; // Speed of movement
    public float rotationSpeed = 5f; // Speed of rotation

    void Awake()
     {
        animator = GetComponent<Animator>();
        var actionMap = inputActions.FindActionMap("MotionTracking");
        leftHipAction = actionMap.FindAction("LeftHipAbducted");
        rightHipAction = actionMap.FindAction("RightHipAbducted");
        footRaiseAction = actionMap.FindAction("FootRaise");

        footRaiseAction.performed += OnFootRaise;
        leftHipAction.performed += OnLeftHip;
        rightHipAction.performed += OnRightHip;
        
     }
    // Update is called once per frame
    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
    // void Update()
    // {
    //     if (currentWaypointIndex == waypoints.Count) {
    //         ChangeScene("4. Sewing");
    // }
    // }

    private void OnEnable()
    {
        leftHipAction.Enable();
        rightHipAction.Enable();
        footRaiseAction.Enable();
    }

    private void OnDisable()
    {
        leftHipAction.Disable();
        rightHipAction.Disable();
        footRaiseAction.Disable();
    }
    private void OnFootRaise(InputAction.CallbackContext ctx)
    {
        if (currentWaypointIndex == waypoints.Count) {
            ChangeScene("4. Sewing");
    }
    }
    private void OnLeftHip(InputAction.CallbackContext ctx)
    {
        if (movementAvailable == true && currentWaypointIndex < waypoints.Count)
        {
            movementAvailable = false;
                RepOne++;
                animator.SetTrigger("PlayAnimation");
                StartCoroutine(MoveToWaypoint(waypoints[currentWaypointIndex])); // Start movement to waypoint
        }
         }
    private void OnRightHip(InputAction.CallbackContext ctx){

        if (movementAvailable == true && currentWaypointIndex < waypoints.Count)
        {
            movementAvailable = false;
                RepTwo++;
                animator.SetTrigger("PlayAnimation");
                StartCoroutine(MoveToWaypoint(waypoints[currentWaypointIndex])); // Start movement to waypoint
        }
    }
   private IEnumerator MoveToWaypoint(Transform targetWaypoint)
    {
        SoundManager.PlaySound(SoundType.SCISSORS);
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 targetPosition = targetWaypoint.position;
        Quaternion targetRotation = targetWaypoint.rotation;

        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            // Smooth movement
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            // Smooth rotation
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, fractionOfJourney);

            yield return null; // Wait for the next frame
        }

        transform.position = targetPosition; // Ensure it exactly reaches the target position
        transform.rotation = targetRotation; // Ensure it exactly reaches the target rotation

        currentWaypointIndex++;
        movementAvailable = true;
    }
}
}