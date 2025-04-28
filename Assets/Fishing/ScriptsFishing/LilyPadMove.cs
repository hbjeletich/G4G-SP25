using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyPadMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 2f;        
    [SerializeField] private float speed = 1f; 

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        MoveLilyPad();
    }

    void MoveLilyPad()
    {
        float offsetX = Mathf.Sin(Time.time * speed) * (moveDistance / 2f);
        transform.position = new Vector3(startPosition.x + offsetX, startPosition.y, startPosition.z);
    }
}