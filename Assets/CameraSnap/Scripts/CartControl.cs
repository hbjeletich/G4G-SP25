using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

namespace CameraSnap
{
public class CartController : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float speed = 5f;
    private float currentDistance = 0f;
    private bool isMoving = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleMovement();
        }

        if (!isMoving) return;

        currentDistance += speed * Time.deltaTime;

        float totalLength = splineContainer.CalculateLength();
        float t = currentDistance / totalLength;

        if (t <= 1f)
        {
            splineContainer.Evaluate(t, out float3 pos, out float3 tangent, out float3 up);
            transform.position = (Vector3)pos;

            // Optional: rotate cart only on Y axis (ignore mouse rotation merging for now)
            Vector3 forward = new Vector3(tangent.x, 0f, tangent.z).normalized;
            if (forward.sqrMagnitude > 0f)
                transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }

    public void ToggleMovement()
    {
        isMoving = !isMoving;
    }
}
}