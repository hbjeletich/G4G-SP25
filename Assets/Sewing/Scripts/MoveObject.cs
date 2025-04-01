using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MoveObject : MonoBehaviour
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

    void Awake()
     {
        animator = GetComponent<Animator>();
        var actionMap = inputActions.FindActionMap("MotionTracking");
        leftHipAction = actionMap.FindAction("LeftHipAbducted");
        rightHipAction = actionMap.FindAction("RightHipAbducted");

        leftHipAction.performed += OnLeftHip;
        rightHipAction.performed += OnRightHip;
     }
    // Update is called once per frame
    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
    void Update()
    {
        if (currentWaypointIndex == waypoints.Count) {
            ChangeScene("3. Puzzle");
    }
    }

    private void OnEnable()
    {
        leftHipAction.Enable();
        rightHipAction.Enable();
    }

    private void OnDisable()
    {
        leftHipAction.Disable();
        rightHipAction.Disable();
    }
    private void OnLeftHip(InputAction.CallbackContext ctx)
    {
        if (movementAvailable == true && currentWaypointIndex < waypoints.Count)
        {
            movementAvailable = false;
            RepOne++;
            animator.SetTrigger("PlayAnimation");
        }
         }
    private void OnRightHip(InputAction.CallbackContext ctx){

        if (movementAvailable == true && currentWaypointIndex < waypoints.Count)
        {
            movementAvailable = false;
            RepTwo++;
            animator.SetTrigger("PlayAnimation");
        }
    }
    // Function to move the object to the current waypoint
    private void MoveToWaypoint()
    {
        if (currentWaypointIndex < waypoints.Count)
        {
            transform.position = waypoints[currentWaypointIndex].position;
             transform.rotation = waypoints[currentWaypointIndex].rotation;
            
                currentWaypointIndex++;
                movementAvailable = true;
            
        }
    }
}