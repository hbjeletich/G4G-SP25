using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

//This script allows the cart to move along a spline. This allows for easily editing the spline in the scene and it working
//without issue.

namespace CameraSnap
{
    public class CartController : MonoBehaviour
    {
        public SplineContainer splineContainer;  //path cart follows
        public float speed = 5f;  //how fast it travels

        public SlowdownZone currentZone;   //tracks which slowdown zone cart is in.

        private float defaultSpeed;
        private float currentDistance = 0f;
        private bool isMoving = true;   //cart is actively progressing
        private bool canStop = false;   //true in slowdown zone
        private bool isStopped = false; //cart is stopped
        public AudioSource audioSource;

        void Start()
        {
            defaultSpeed = speed;  //stores original speed to be restored when cart is resumed.
        }

        void Update()
        {
            // Simulate squat stop using keyboard (matches motion capture behavior)
            if (Input.GetKeyDown(KeyCode.Space) && canStop)
            {
                // Only allow stopping, not resuming
                if (!isStopped)
                {
                    StopCart();
                    Debug.Log("[Keyboard] Space pressed → Cart stopped (squat simulated)");
                }
                else
                {
                    Debug.Log("[Keyboard] Space pressed but cart already stopped. Waiting for zone completion to auto-resume.");
                }
            }

            //Allows movement if:
            if (!isMoving || isStopped)
                return;

            //Increase distance travelled, calculate progress along the spline, position and rotate
            currentDistance += speed * Time.deltaTime;

            float totalLength = splineContainer.CalculateLength();
            float t = currentDistance / totalLength;

            //if cart reaches end of spline, the movement stops
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

        //The speed control is used by slowdown zone and other scripts...
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

        // Called by slowdown zones. When entering a zone, allow stopping. When leaving, disable stopping and resume cart
        public void AllowStop(bool value)
        {
            canStop = value;
            if (!value)
                ResumeCart();
        }

        //Handles the stopping and starting, shows and hides the UI indicator for showing if the cart is stopped.
        public void StopCart()
        {
            if (!canStop) return;

            isStopped = true;
            isMoving = false;

            // Rely on UIManager to control stop-cart UI
            if (UIManager.Instance == null)
            {
                Debug.LogError("[CartController] UIManager not found; stop cart UI cannot be shown.");
            }
            else
            {
                UIManager.Instance.SetStopCartVisible(true);
            }

            // If stopping inside a slowdown zone, advance the player guide to the next step
            if (currentZone != null && UIManager.Instance != null)
            {
                Debug.Log("[CartController] Stopped in zone -> advancing guide to HipAbduction");
                UIManager.Instance.SetGuideState(UIManager.GuideState.HipAbduction);
            }

            if (audioSource != null)
                audioSource.Stop();
        }

        public void ResumeCart()
        {
            isStopped = false;
            isMoving = true;

            // Rely on UIManager to control stop-cart UI
            if (UIManager.Instance == null)
            {
                Debug.LogError("[CartController] UIManager not found; stop cart UI cannot be hidden.");
            }
            else
            {
                UIManager.Instance.SetStopCartVisible(false);
            }

            if (audioSource != null)
                audioSource.Play();
        }

        //Used by other scripts such as camera mode to check if player can take photos.
        public bool IsStopped() => isStopped;
        public bool CanStop() => canStop;
    }
}
