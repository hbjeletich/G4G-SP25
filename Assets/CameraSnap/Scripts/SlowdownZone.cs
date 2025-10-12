using UnityEngine;

namespace CameraSnap
{
    public class SlowdownZone : MonoBehaviour
    {
        [Header("Slowdown Settings")]
        public float slowedSpeed = 1f;
        public GameObject uiIcon;

        [Header("Animal Spawning")]
        public AnimalSpawner animalSpawner;

        private void OnTriggerEnter(Collider other)
        {
            CartController cart = other.GetComponentInParent<CartController>();

            if (cart != null)
            {
                cart.SetSpeed(slowedSpeed);
                cart.AllowStop(true); // allow stopping only in zone

                if (uiIcon != null)
                    uiIcon.SetActive(true);

                if (animalSpawner != null)
                    animalSpawner.SpawnAnimals();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            CartController cart = other.GetComponentInParent<CartController>();

            if (cart != null)
            {
                cart.ResetSpeed();   //  resumes normal movement
                cart.AllowStop(false);

                if (uiIcon != null)
                    uiIcon.SetActive(false);

                if (animalSpawner != null)
                    animalSpawner.ClearPreviousAnimals();
            }
        }
    }
}
