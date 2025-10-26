using UnityEngine;
using System.Collections.Generic;

namespace CameraSnap
{
    public class AnimalSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("Left side spawn points.")]
        public List<Transform> leftSpawnPoints;

        [Tooltip("Right side spawn points.")]
        public List<Transform> rightSpawnPoints;

        [Tooltip("Maximum number of animals to spawn (should be 2).")]
        public int maxAnimals = 2;

        private List<GameObject> spawnedAnimals = new List<GameObject>();

        public void SpawnAnimals()
        {
            ClearPreviousAnimals();

            if (GameManager.Instance == null || GameManager.Instance.GetAllAnimals().Count == 0)
            {
                Debug.LogWarning("[AnimalSpawner] No animals available to spawn!");
                return;
            }

            List<AnimalData> allAnimals = new List<AnimalData>(GameManager.Instance.GetAllAnimals());
            Shuffle(allAnimals);

            int animalsSpawned = 0;

            // Spawn LEFT
            if (animalsSpawned < maxAnimals && leftSpawnPoints.Count > 0)
            {
                SpawnAnimalAt(leftSpawnPoints[Random.Range(0, leftSpawnPoints.Count)], allAnimals[animalsSpawned], false);
                animalsSpawned++;
            }

            // Spawn RIGHT
            if (animalsSpawned < maxAnimals && rightSpawnPoints.Count > 0)
            {
                SpawnAnimalAt(rightSpawnPoints[Random.Range(0, rightSpawnPoints.Count)], allAnimals[animalsSpawned], true);
                animalsSpawned++;
            }
            Debug.Log($"Spawning animals... Left:{leftSpawnPoints.Count}, Right:{rightSpawnPoints.Count}, Max:{maxAnimals}, TotalAnimals:{allAnimals.Count}");

        }

        private void SpawnAnimalAt(Transform spawnPoint, AnimalData animalData, bool spawnOnRightSide)
        {
            if (animalData == null || animalData.animalPrefab == null) return;

            GameObject animal = Instantiate(animalData.animalPrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedAnimals.Add(animal);

            Vector3 scale = animal.transform.localScale;

            
            bool shouldFaceLeft = !spawnOnRightSide;

            if (animalData.spriteFacesLeft)
                scale.x = shouldFaceLeft ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            else
                scale.x = shouldFaceLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);

            animal.transform.localScale = scale;

            var behavior = animal.GetComponent<AnimalBehavior>();
            if (behavior != null)
            {
                behavior.animalData = animalData;
                behavior.SetMovingDirection(shouldFaceLeft);
            }

            Debug.Log($"[AnimalSpawner] Spawned {animalData.animalName} at {spawnPoint.position}, facingLeft={shouldFaceLeft}");
            Debug.Log($"Spawned {animalData.animalName} at {(spawnOnRightSide ? "RIGHT" : "LEFT")}");

        }

        public void ClearPreviousAnimals()
        {
            foreach (var animal in spawnedAnimals)
            {
                if (animal != null)
                    Destroy(animal);
            }

            spawnedAnimals.Clear();
            Debug.Log("[AnimalSpawner] Cleared previous animals");
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public List<GameObject> GetSpawnedAnimals()
        {
            return spawnedAnimals;
        }
    }
}
