using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sewing;

namespace Sewing{
    

public class KeyboardSewingPath : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();          // Waypoints set in the Inspector
    public float moveSpeed = 2f;               // Speed of movement

    private int currentIndex = 0;
    private bool isMoving = false;

    

    void Awake()
    {
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.A) && !isMoving)
        {
            MoveToNextWaypoint();
        }
    }

    void MoveToNextWaypoint()
    {
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