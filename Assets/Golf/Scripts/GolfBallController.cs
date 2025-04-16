using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallController : MonoBehaviour
{
    Rigidbody rb;
    TrailRenderer trail;

    private Vector3 startingPos;
    private Quaternion startingRotation;

    public AK.Wwise.Event BounceEvent;
    public AK.Wwise.Event HitEvent;

    void Start(){
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();

        startingPos = transform.position;
        startingRotation = transform.rotation;
    }

    public void hitGolfBall(float hitStrength){
        trail.enabled = true;
        rb.AddForce(transform.up * hitStrength, ForceMode.Impulse);
        if (HitEvent != null)
        {
            HitEvent.Post(gameObject);
            
        }
    }

    public int getDistanceFromStart(){
        return Mathf.RoundToInt(transform.position.z - startingPos.z);
    }

    public bool isMoving(){
        if(rb.velocity.magnitude <= 10f){
            rb.AddForce(-rb.velocity.normalized);
        }

        if (rb.velocity.magnitude <= .1f){
            rb.velocity = Vector3.zero;
            return false;
        }
        return true;
    }

    public void disableTrail(){
        trail.enabled = false;
    }

    public void toStartingState(){
        transform.position = startingPos;
        transform.rotation = startingRotation;
        trail.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 0.2f)
        {
            BounceEvent.Post(gameObject);
        }
    }
}
