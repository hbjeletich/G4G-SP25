using UnityEngine;
using System.Collections.Generic;

//Maybe change this script to an animal manager and then game manager is for main menu, tutorial or main stage, end screen or 
//gallery!! note this..


namespace CameraSnap
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("All Animals")]
        [Tooltip("List of all animal types available in the game.")]
        public List<AnimalData> allAnimals;

        void Awake()
        {
            // Singleton setup
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            Debug.Log("[GameManager] Initialized with " + allAnimals.Count + " animals available.");
        }

       
        /// Returns the full list of animals that can appear anywhere.
        
        public List<AnimalData> GetAllAnimals()
        {
            return allAnimals;
        }

      
        /// Returns a random animal from the full list.
        /// Useful if you want to spawn a random animal anywhere.
       
        public AnimalData GetRandomAnimal()
        {
            if (allAnimals == null || allAnimals.Count == 0)
            {
                Debug.LogWarning("[GameManager] No animals available!");
                return null;
            }

            int index = Random.Range(0, allAnimals.Count);
            return allAnimals[index];
        }

        // Keeps track of all animals the player has photographed
private HashSet<string> capturedAnimals = new HashSet<string>();

public void RegisterCapturedAnimal(string animalName)
{
    if (string.IsNullOrEmpty(animalName)) return;
    capturedAnimals.Add(animalName);
    Debug.Log($"[GameManager] Added captured animal: {animalName}");
}

public bool HasCaptured(string animalName)
{
    return capturedAnimals.Contains(animalName);
}

public HashSet<string> GetCapturedAnimals()
{
    return capturedAnimals;
}


       
    }
}
