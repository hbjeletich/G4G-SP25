using UnityEngine;

//This is the script to allow the player to look left and right. The player is restricted to how much they can look left and right
//so that they are not able to do a full 360, so they can use both feet to look left and right instead of one foot to go all the way
//around

namespace CameraSnap
{
    public class CameraPan : MonoBehaviour
    //How fast the camera rotates, how far it can rotate, tracks how far it currently rotated.
    {
        public float panSpeed = 50f;
        public float maxYaw = 60f; // Max pan angle left/right, player cannot do 360 look
        private float currentYaw = 0f;
//Hides and locks the mouse cursor so player can't freely move it.
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
//Detects arrow keys input, adjusts rotation based on that, stops rotation if player reaches max
//allowed angle.
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
        //Public function that allows other scripts rotate the camera.
        public void ManualPan(float input)
{
    currentYaw += input * panSpeed * Time.deltaTime;
    currentYaw = Mathf.Clamp(currentYaw, -maxYaw, maxYaw);
    transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);
}

    }
}
