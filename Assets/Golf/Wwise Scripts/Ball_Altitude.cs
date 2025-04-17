using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Altitude : MonoBehaviour
{
    public AK.Wwise.Event WindEvent;   // Wind sound event
    public float windMinHeight = 5f;   // Minimum height for wind
    public float windMaxHeight = 50f;  // Adjusted max height based on actual values
    private float currentWindVolume = 0f; // To track current wind volume

    private Rigidbody rb;
    private bool windPlaying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Start the wind
        WindEvent.Post(gameObject);
        windPlaying = true;
        //Debug.Log("Wind Sound Started");
    }

    void Update()
    {
        float currentHeight = transform.position.y;
        float currentSpeed = rb.velocity.magnitude;

        // Calculate the wind volume based on height
        if (currentHeight > windMinHeight && currentSpeed > 0.5f)
        {
            float normalizedHeight = Mathf.InverseLerp(windMinHeight, windMaxHeight, currentHeight);
            currentWindVolume = Mathf.Lerp(0f, 100f, normalizedHeight); 
        }
        else
        {
            currentWindVolume = 0f; 
        }

        // Debug Logs
        //Debug.Log($"Ball Height: {currentHeight} | Wind Volume: {currentWindVolume}");

        // Set RTPC 
        AkUnitySoundEngine.SetRTPCValue("Wind_Volume", currentWindVolume);
    }
}