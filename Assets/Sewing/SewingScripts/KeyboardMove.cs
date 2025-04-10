using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sewing {
public class KeyboardMove : MonoBehaviour
{

    //script to test with keyboard controls when not in lab


    // List to hold waypoints (Empty GameObjects you set up in the scene)
    public List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0; // Tracks the current waypoint
    public int RepOne = 0;
    public int RepTwo = 0;
    private Animator animator;
    private bool movementAvailable = true;

    public float moveSpeed = 3f; // Speed of movement
    public float rotationSpeed = 5f; // Speed of rotation
    private bool isMoving = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void Update()
    {
        if (movementAvailable == true)
        {
            // Check if the player presses A and there are more waypoints to move through
            if (Input.GetKeyDown(KeyCode.A) && currentWaypointIndex < waypoints.Count)
            {
                movementAvailable = false;
                RepOne++;
                animator.SetTrigger("PlayAnimation");
                StartCoroutine(MoveToWaypoint(waypoints[currentWaypointIndex])); // Start movement to waypoint
            }
            // Check if the player presses D and there are more waypoints to move through
            else if (Input.GetKeyDown(KeyCode.D) && currentWaypointIndex < waypoints.Count)
            {
                movementAvailable = false;
                RepTwo++;
                animator.SetTrigger("PlayAnimation");
                StartCoroutine(MoveToWaypoint(waypoints[currentWaypointIndex])); // Start movement to waypoint
            }
            else if (currentWaypointIndex == waypoints.Count)
            {
                ChangeScene("3. Puzzle");
            }
        }
    }

    // Coroutine to move the object to the current waypoint smoothly
    private IEnumerator MoveToWaypoint(Transform targetWaypoint)
    {
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