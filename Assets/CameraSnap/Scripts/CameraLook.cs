using UnityEngine;

namespace CameraSnap
{
    public class CameraPan : MonoBehaviour
    {
        public float maxYaw = 60f;
        public float panSpeed = 30f;
        public float deadzone = 0.05f;
        public float decelerationSpeed = 5f;

        private float currentYaw = 0f;
        private float currentInput = 0f;

        void Update()
        {
            if (Mathf.Abs(currentInput) <= deadzone)
            {
                currentInput = Mathf.Lerp(currentInput, 0f, decelerationSpeed * Time.deltaTime);
            }

            currentYaw += currentInput * panSpeed * Time.deltaTime;
            currentYaw = Mathf.Clamp(currentYaw, -maxYaw, maxYaw);

            transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);
        }

        public void ManualPan(float weightShiftX)
        {
            currentInput = weightShiftX;
        }
    }
}