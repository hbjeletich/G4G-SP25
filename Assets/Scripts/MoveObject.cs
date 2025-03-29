using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    // List to hold waypoints (Empty GameObjects you set up in the scene)
    public List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0; // Tracks the current waypoint
    public int RepOne = 0;
    public int RepTwo = 0;
    private Animator animator;

    void Start()
     {
        animator = GetComponent<Animator>();
     }
    // Update is called once per frame
    void Update()
    {
        // Check if the player presses A and there are more waypoints to move through
        if (Input.GetKeyDown(KeyCode.A) && currentWaypointIndex < waypoints.Count)
        {
            currentWaypointIndex++;
            RepOne++;
            animator.SetTrigger("PlayAnimation");
        }
        // Check if the player presses D and there are more waypoints to move through
        else if (Input.GetKeyDown(KeyCode.D) && currentWaypointIndex < waypoints.Count)
        {
            currentWaypointIndex++;
            RepTwo++;
            animator.SetTrigger("PlayAnimation");
        }
    }

    // Function to move the object to the current waypoint
    private void MoveToWaypoint()
    {
        if (currentWaypointIndex < waypoints.Count)
        {
            // Move the object to the position of the current waypoint
            transform.position = waypoints[currentWaypointIndex].position;
        }
    }
}
