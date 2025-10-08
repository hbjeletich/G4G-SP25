using UnityEngine;
using System.Collections.Generic;

namespace CameraSnap
{
    public class AnimalSpawner : MonoBehaviour
    {
        [Tooltip("Which zone this spawner represents (e.g., 0, 1, 2)")]
        public int zoneIndex;

        [Tooltip("Where animals will spawn")]
        public List<Transform> spawnPoints = new List<Transform>();

        private List<GameObject> spawnedAnimals = new List<GameObject>();


        //MYSTERY ANIMAL in this context is about the specific animal the player has to find and photograph
        //in this zone. Parts of this code will make sure that this animal is always spawned in the zone

        public void SpawnAnimals(AnimalData mysteryAnimal)
        {
            ClearPreviousAnimals();

            List<AnimalData> animalsToSpawn = GameManager.Instance.GetAnimalsForZone(zoneIndex);

            
            if (mysteryAnimal != null && !animalsToSpawn.Contains(mysteryAnimal))
            {
                animalsToSpawn.Add(mysteryAnimal);
                Debug.Log($"[AnimalSpawner] Added mystery animal: {mysteryAnimal.animalName}");
            }
            //•Loops through each animal to spawn, up to the number of available spawn points.
            for (int i = 0; i < Mathf.Min(animalsToSpawn.Count, spawnPoints.Count); i++)
            {
                GameObject prefab = animalsToSpawn[i].animalPrefab;
                if (prefab != null)
                {
                    Vector3 pos = spawnPoints[i].position;
                    GameObject animal = Instantiate(prefab, pos, Quaternion.identity);
                    spawnedAnimals.Add(animal);
                    Debug.Log($"[AnimalSpawner] Spawned: {animalsToSpawn[i].animalName} at {pos}");
                }
                else
                {
                    Debug.LogWarning($"[AnimalSpawner] Prefab missing for animal: {animalsToSpawn[i].animalName}");
                }
            }
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
    }
}
