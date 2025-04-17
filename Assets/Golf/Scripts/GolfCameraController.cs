using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfCameraController : MonoBehaviour
{
    private float followBallDelay = 1f;

    public bool isChasingBall = false;
    private float offsetYFromBall = 10f;
    private float offsetZFromBall = -10f;
    private float offsetXFromBall = 15f;

    private float smoothTime = .3f;
    private Vector3 _velocity = Vector3.zero;

    public Transform ballTransform;

    private Vector3 startTransform;
    private Quaternion startRotation;

    void Start(){
        startTransform = transform.position;
        startRotation = transform.rotation;
    }

    public void followBall(){
        Vector3 ballPosition = ballTransform.position;
        Vector3 targetPos = new Vector3(ballPosition.x + offsetXFromBall, ballPosition.y + offsetYFromBall, ballPosition.z + offsetZFromBall);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, smoothTime);
        transform.LookAt(ballTransform);
    }

    public void toStartTransform(){
        Debug.Log("running camera to start!");
        transform.position = startTransform;
        transform.rotation = startRotation;
    }
}
