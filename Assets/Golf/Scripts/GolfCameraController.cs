using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfCameraController : MonoBehaviour
{
    private float followBallDelay = 1f;

    public bool isChasingBall = false;
    private float offsetYFromBall = 10f;
    private float offsetZFromBall = -15f;
    private float offsetXFromBall = 5f;

    private float smoothTime = 1f;
    private Vector3 _velocity = Vector3.zero;

    public Transform ballTransform;

    public void followBall(){
        Vector3 ballPosition = ballTransform.position;
        Vector3 targetPos = new Vector3(ballPosition.x + offsetXFromBall, ballPosition.y + offsetYFromBall, ballPosition.z + offsetZFromBall);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, smoothTime);
        transform.LookAt(ballTransform);
    }
}
