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


      
        
        public void SpawnAnimals(System.Collections.Generic.List<AnimalData> assignedAnimals = null)
        {
            // If animals are already spawned by this spawner, do not clear or respawn them.
            // This preserves animals in the scene so they don't vanish when the player
            // leaves or the zone completes.
            if (spawnedAnimals != null && spawnedAnimals.Count > 0)
            {
                return;
            }

            if (GameManager.Instance == null || GameManager.Instance.GetAllAnimals().Count == 0)
            {
                return;
            }
            // Build chosen list from assignedAnimals (if provided) then fill from pool
            List<AnimalData> chosen = new List<AnimalData>();
            if (assignedAnimals != null && assignedAnimals.Count > 0)
            {
                foreach (var a in assignedAnimals)
                {
                    if (a != null && !chosen.Contains(a))
                    {
                        chosen.Add(a);
                        if (chosen.Count >= maxAnimals) break;
                    }
                }
            }

            // Fill remaining slots from GameManager pool using weighted rarity selection.
            var pool = GameManager.Instance.GetAllAnimals();
            List<AnimalData> poolCopy = new List<AnimalData>(pool);
            // remove any already chosen
            poolCopy.RemoveAll(x => chosen.Contains(x));
            int need = maxAnimals - chosen.Count;
            // Weighted random selection without replacement using AnimalData.spawnRarity
            for (int pick = 0; pick < need && poolCopy.Count > 0; pick++)
            {
               
                float totalWeight = 0f;
                for (int w = 0; w < poolCopy.Count; w++)
                    totalWeight += GetRarityWeight(poolCopy[w]);

                if (totalWeight <= 0f)
                {
                   
                    break;
                }

                float r = Random.Range(0f, totalWeight);
                float acc = 0f;
                for (int i = 0; i < poolCopy.Count; i++)
                {
                    acc += GetRarityWeight(poolCopy[i]);
                    if (r <= acc)
                    {
                        chosen.Add(poolCopy[i]);
                        poolCopy.RemoveAt(i);
                        break;
                    }
                }
            }

            int animalsSpawned = 0;

            // Spawn LEFT
            if (animalsSpawned < maxAnimals && leftSpawnPoints.Count > 0 && chosen.Count > animalsSpawned)
            {
                SpawnAnimalAt(leftSpawnPoints[Random.Range(0, leftSpawnPoints.Count)], chosen[animalsSpawned], false);
                animalsSpawned++;
            }

            // Spawn RIGHT
            if (animalsSpawned < maxAnimals && rightSpawnPoints.Count > 0 && chosen.Count > animalsSpawned)
            {
                SpawnAnimalAt(rightSpawnPoints[Random.Range(0, rightSpawnPoints.Count)], chosen[animalsSpawned], true);
                animalsSpawned++;
            }

            

        }

//Instantiates the prefav, adds it to the spawned animals tracking list, flips animal too face correctly,
// sets its movement in animal behavior, flipping logic: if spawned on right: face left, if spawned left: face right.
//This makes it so their animations won't appear backwards when they are walking on opposite sides.
//It checks the 'spritefacesleft' to know the default direction of the sprite.
//VVVVVVVVV

        private void SpawnAnimalAt(Transform spawnPoint, AnimalData animalData, bool spawnOnRightSide)
        {
            if (animalData == null || animalData.animalPrefab == null) return;

            GameObject animal = Instantiate(animalData.animalPrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedAnimals.Add(animal);

            // Let the AnimalBehavior set sprite direction to avoid duplicating scale logic here.
            bool shouldFaceLeft = !spawnOnRightSide;

            var behavior = animal.GetComponent<AnimalBehavior>();
            if (behavior != null)
            {
                behavior.animalData = animalData;
                behavior.SetStartDirection(shouldFaceLeft);
            }

            

        }
//Destroy any old spawned animals so new ones don't overlap.
        public void ClearPreviousAnimals()
        {
            foreach (var animal in spawnedAnimals)
            {
                if (animal != null)
                    Destroy(animal);
            }

            spawnedAnimals.Clear();
        }
        

        // Map rarity tiers to selection weights. Adjust these values to tune odds.
        // Common should be the largest number so it is most likely to be chosen.
        private float GetRarityWeight(AnimalData data)
        {
            if (data == null) return 0f;
            switch (data.spawnRarity)
            {
                case AnimalData.SpawnRarity.Rare:
                    return 1f; // least likely
                case AnimalData.SpawnRarity.Moderate:
                    return 3f; // medium
                case AnimalData.SpawnRarity.Common:
                    return 6f; // most likely
                default:
                    return 1f;
            }
        }
//Lets other scripts access animals currently spawned in one zone. To be clear, it prevents one animal
//being spawned multiple times in one zone but it can spawn again in another slowdown zone.
        public List<GameObject> GetSpawnedAnimals()
        {
            return spawnedAnimals;
        }
    }
}
