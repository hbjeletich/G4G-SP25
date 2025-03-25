using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishObject : MonoBehaviour
{
    [Header("Fish Behavior")]
    public float moveSpeed = 2f;
    public float radiusX = 4f;
    public float radiusY = 2f;
    
    public FishData fishData;
    private Vector2 startPos;
    private Vector2 targetPos;  
    // public float sizeScaling(?)

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        MoveToTarget();
    }

      private void MoveToTarget()
    {
        Vector2 currentPos = transform.position;
        transform.position = Vector2.MoveTowards(
            currentPos, 
            targetPos, 
            moveSpeed * Time.deltaTime);

        Vector2 direction = new Vector2(
            targetPos.x - currentPos.x, 
            targetPos.y - currentPos.y).normalized;

        if (Vector2.Distance(currentPos, targetPos) < 0.1f)
        {
            SetRandomTargetPos(); 
        }
    }

    private void SetRandomTargetPos()
    {
        targetPos = startPos + new Vector2(
            Random.Range(-radiusX, radiusX),   
            Random.Range(-radiusY, radiusY)   
        );
    }
}
