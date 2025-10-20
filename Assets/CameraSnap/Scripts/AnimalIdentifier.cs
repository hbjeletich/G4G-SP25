using UnityEngine;
   

//This script is attached to animal prefabs so that it can be connected to the animal data scriptable object. I also attached a 
//billboarding effect for the 2d sprites of the animals so that they always face the camera. It is attached here so I didn't have
//to make a seperate script, and it is specific just to the animals anyways.

namespace CameraSnap
{
    public class AnimalIdentifier : MonoBehaviour
    {
        public AnimalData animalData;

        [HideInInspector] public bool isCaptured = false;

        private Camera mainCamera;

        void Start()
        {
            mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            if (mainCamera == null)
                return;

            // Make sure animal faces the camera
            Vector3 direction = mainCamera.transform.position - transform.position;
            direction.y = 0f; // keep upright
            transform.rotation = Quaternion.LookRotation(-direction);
        }
        
    }
}
