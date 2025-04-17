using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_Velocity : MonoBehaviour
{
    public AK.Wwise.Event BounceEvent; // Grass bounce sound
    public GameObject GroundObject;  
    public GameObject ClubObject;   
    private bool hasBeenHit = false; // Prevents initial collision from triggering

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name); // Log what it hits

        if (collision.gameObject == ClubObject)
        {
            hasBeenHit = true;
            Debug.Log("Ball was hit by the club!");
        }
        else if (collision.gameObject == GroundObject && hasBeenHit)
        {
            Debug.Log("Ball hit the ground with velocity: " + collision.relativeVelocity.magnitude);
            if (collision.relativeVelocity.magnitude > 0.2f)
            {
                BounceEvent.Post(gameObject);
                Debug.Log("Bounce sound triggered!");
            }
        }
    }
}