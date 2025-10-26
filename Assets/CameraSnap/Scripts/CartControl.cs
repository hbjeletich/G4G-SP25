using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;


//This script allows the cart to move along a spline. This allows for easily editing the spline in the scene and it working
// without issue.


namespace CameraSnap
{
    public class CartController : MonoBehaviour
    {
        public SplineContainer splineContainer;
        public float speed = 5f;

        public GameObject stopCartObject;

public SlowdownZone currentZone;

        private float defaultSpeed;
        private float currentDistance = 0f;
        private bool isMoving = true;
        private bool canStop = false;  
        private bool isStopped = false;


        void Start()
        {
            defaultSpeed = speed;
        }


        void Update()
        {
            // Press SPACE to stop/resume, but only allowed if inside slowdown zone, fix to incorporate main controller too
            if (Input.GetKeyDown(KeyCode.Space) && canStop)
            {
                if (isStopped)
                    ResumeCart();
                else
                    StopCart();
            }


            if (!isMoving || isStopped)
                return;


            currentDistance += speed * Time.deltaTime;


            float totalLength = splineContainer.CalculateLength();
            float t = currentDistance / totalLength;


            if (t <= 1f)
            {
                splineContainer.Evaluate(t, out float3 pos, out float3 tangent, out float3 up);
                transform.position = (Vector3)pos;


                Vector3 forward = new Vector3(tangent.x, 0f, tangent.z).normalized;
                if (forward.sqrMagnitude > 0f)
                {
                    transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
                }
            }
        }


        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }


        public void ResetSpeed()
        {
            speed = defaultSpeed;
            isStopped = false;
            isMoving = true;
            canStop = false;
        }


        // Called by slowdown zones
        public void AllowStop(bool value)
        {
            canStop = value;
            if (!value)
                ResumeCart();
        }


        public void StopCart()
        {
            if (!canStop) return;
            isStopped = true;
            isMoving = false;
             if (stopCartObject != null) stopCartObject.SetActive(true);
        }


        public void ResumeCart()
        {
            isStopped = false;
            isMoving = true;
             if (stopCartObject != null) stopCartObject.SetActive(false);
        }


        public bool IsStopped() => isStopped;
        public bool CanStop() => canStop;
    }
}
