using UnityEngine;

public class RotateRings : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 10f;

    [Header("Rotation Axis")]
    [Tooltip("Axis around which the rings will rotate")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Options")]
    [Tooltip("Use local space instead of world space")]
    public bool useLocalSpace = true;

    void Update()
    {
        // Rotate the rings based on the specified axis and speed
        if (useLocalSpace)
        {
            // Rotate in local space (relative to the object's own orientation)
            transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            // Rotate in world space
            transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}