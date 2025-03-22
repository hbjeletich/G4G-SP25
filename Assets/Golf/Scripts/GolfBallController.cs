using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallController : MonoBehaviour
{
    Rigidbody rb;
    TrailRenderer trail;

    private Vector3 startingPos;

    void Start(){
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();

        startingPos = transform.position;
    }

    public void hitGolfBall(float hitStrength){
        trail.enabled = true;
        rb.AddForce(transform.up * hitStrength, ForceMode.Impulse);
    }

    public float getDistanceFromStart(){
        return transform.position.z - startingPos.z;
    }

    public bool isMoving(){
        if (rb.velocity.magnitude <= 0){return false;}
        return true;
    }

    public void disableTrail(){
        trail.enabled = false;
    }
}
