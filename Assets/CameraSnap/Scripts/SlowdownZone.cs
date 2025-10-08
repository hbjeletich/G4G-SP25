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
                // Slow down cart
                cart.SetSpeed(slowedSpeed);

                // Show UI icon
                if (uiIcon != null)
                {
                    uiIcon.SetActive(true);
                }

                // Spawn animals
                if (animalSpawner != null)
                {
                    AnimalData mysteryAnimal = GameManager.Instance.GetMysteryAnimalForZone(animalSpawner.zoneIndex);
                    animalSpawner.SpawnAnimals(mysteryAnimal);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            CartController cart = other.GetComponentInParent<CartController>();

            if (cart != null)
            {
                // Reset speed
                cart.ResetSpeed();

                // Hide UI icon
                if (uiIcon != null)
                {
                    uiIcon.SetActive(false);
                }

                // Clear spawned animals
                if (animalSpawner != null)
                {
                    animalSpawner.ClearPreviousAnimals();
                }
            }
        }
    }
}

