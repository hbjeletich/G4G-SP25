
using UnityEngine;
using System.Collections.Generic;

namespace CameraSnap
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("All Animals")]
        public List<AnimalData> allAnimals;

        private AnimalData[] mysteryAnimals = new AnimalData[3];

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
            PickMysteryAnimals();
        }

        void PickMysteryAnimals()
        {
            for (int zone = 0; zone < 3; zone++)
            {
                List<AnimalData> animalsInZone = GetAnimalsForZone(zone);
                if (animalsInZone.Count > 0)
                {
                    mysteryAnimals[zone] = animalsInZone[Random.Range(0, animalsInZone.Count)];
                    Debug.Log($"[GameManager] Picked mystery animal for zone {zone}: {mysteryAnimals[zone].animalName}");
                }
                else
                {
                    Debug.LogWarning($"[GameManager] No animals found for zone {zone}!");
                }
            }
        }

        public AnimalData GetMysteryAnimalForZone(int zoneIndex)
        {
            if (zoneIndex < 0 || zoneIndex >= mysteryAnimals.Length) return null;
            return mysteryAnimals[zoneIndex];
        }

        public List<AnimalData> GetAnimalsForZone(int zoneIndex)
        {
            return allAnimals.FindAll(animal => animal.zoneIndex == zoneIndex);
        }
    }
}
