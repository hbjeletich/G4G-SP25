using UnityEngine;

namespace CameraSnap
{
    public class CameraPan : MonoBehaviour
    {
        public float panSpeed = 50f;
        public float maxYaw = 60f; // Max pan angle left/right, player cannot do 360 look
        private float currentYaw = 0f;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            float horizontalInput = 0f;

            if (Input.GetKey(KeyCode.RightArrow))
            {
                horizontalInput = 1f;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                horizontalInput = -1f;
            }

            currentYaw += horizontalInput * panSpeed * Time.deltaTime;
            currentYaw = Mathf.Clamp(currentYaw, -maxYaw, maxYaw);

            // Apply yaw to camera (local Y rotation)
            transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);
        }
    }
}
