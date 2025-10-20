using UnityEngine;
using System.Collections.Generic;

//This script controls the animal spawning. The main idea is that there are two animals that spawn in each zone. They spawn 
//on the left and right of the player. In the zones, there are multiple spawn points that the animals will randomly choose
//to appear in. The important thing is that there is one animal on the left and right that the player photographs. 
//Animals will not appear twice in a zone, so there are no repeats. 

//NOTE: make sure animals will spawn on both the left and right side. maybe differentiate the spawn points to show
//which is left and right!!!

namespace CameraSnap
{
    public class AnimalSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("Points in the scene where animals can spawn.")]  //manually put in spawn points in scene
        public List<Transform> spawnPoints = new List<Transform>();

        [Tooltip("Maximum number of animals to spawn.")] //Changeable but it should just stay 2
        public int maxAnimals = 2;

        private List<GameObject> spawnedAnimals = new List<GameObject>();

        
        /// Spawns unique random animals (no repeats) in this zone.
        
        public void SpawnAnimals()
        {
            ClearPreviousAnimals();

            if (GameManager.Instance == null || GameManager.Instance.GetAllAnimals().Count == 0)
            {
                Debug.LogWarning("[AnimalSpawner] No animals available to spawn!");
                return;
            }

            List<AnimalData> allAnimals = new List<AnimalData>(GameManager.Instance.GetAllAnimals());

            if (allAnimals.Count == 0)
            {
                Debug.LogWarning("[AnimalSpawner] No animals to spawn!");
                return;
            }

            int spawnCount = Mathf.Min(maxAnimals, spawnPoints.Count, allAnimals.Count);

            // Shuffle the animal list so each spawn is random and unique
            Shuffle(allAnimals);

            for (int i = 0; i < spawnCount; i++)
            {
                AnimalData animalToSpawn = allAnimals[i]; // unique selection
                if (animalToSpawn == null || animalToSpawn.animalPrefab == null)
                {
                    Debug.LogWarning("[AnimalSpawner] Missing animal prefab.");
                    continue;
                }

                Transform spawnPoint = spawnPoints[i];
                Vector3 pos = spawnPoint.position;
                Quaternion rot = spawnPoint.rotation;

                GameObject animal = Instantiate(animalToSpawn.animalPrefab, pos, rot);
                spawnedAnimals.Add(animal);

                Debug.Log($"[AnimalSpawner] Spawned unique animal: {animalToSpawn.animalName} at {pos}");
            }
        }

      
        /// Clears previously spawned animals before respawning.
      
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

        
        /// Helper function to shuffle a list  
        
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
