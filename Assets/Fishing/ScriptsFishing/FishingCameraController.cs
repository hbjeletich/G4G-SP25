using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishing
{
    public class FishingCameraController : MonoBehaviour
    {
    public static FishingCameraController instance;

    [Header("Camera Position Settings")]
    public Vector3 fishingModeOffset = new Vector3(0, -2f, -10); 
    public Vector3 meterModeOffset = new Vector3(0, 0, -10);
    public float cameraMoveSpeed = 2f;

    private Transform target;
    private Vector3 desiredPosition;

    private void Awake()
    {
        if (instance == null)
        instance = this;
        else
            Destroy(gameObject);

        target = Camera.main.transform;  
        desiredPosition = meterModeOffset; // Start with meter mode offset
    }

    private void Update()
    {
        if (target == null) return;
        target.position = Vector3.Lerp(target.position, desiredPosition, Time.deltaTime * cameraMoveSpeed);
    }

    public void ShiftToMeterView()
    {
        desiredPosition = meterModeOffset;
    }

    public void ShiftToFishingView()
    {
        desiredPosition = fishingModeOffset;
    }
}
}
