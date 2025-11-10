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
            // Singleton setup... Only one game manager exists across scenes, so it persists when switching scenes.. Other
            //scripts can easily access it by gamemanager.instance.. It is a global system.....
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

       
        /// Returns the full list of animals that can appear anywhere. Used for spawning animals.
        public List<AnimalData> GetAllAnimals()
        {
            // Defensive: ensure callers always get a valid list
            return allAnimals ?? new List<AnimalData>();
        }

      
        /// Returns a random animal from the full list.
        /// Picks a random one for spawning
       
        public AnimalData GetRandomAnimal()
        {
            var list = GetAllAnimals();
            if (list.Count == 0)
            {
                Debug.LogWarning("[GameManager] No animals available!");
                return null;
            }

            return list[Random.Range(0, list.Count)];
        }

        // Keeps track of all animals the player has photographed. HashSet ensures no duplicates. Uses the animal
        //name as a key..
private HashSet<string> capturedAnimals = new HashSet<string>();

        // Marks animals as captured.
        public void RegisterCapturedAnimal(string animalName)
        {
            if (string.IsNullOrEmpty(animalName)) return;
            capturedAnimals.Add(animalName);
            Debug.Log($"[GameManager] Added captured animal: {animalName}");
        }

        // Checks if player has photographed the animal already
        public bool HasCaptured(string animalName) => capturedAnimals.Contains(animalName);

        // Gets the whole collection of captured animals
        public HashSet<string> GetCapturedAnimals() => capturedAnimals;


       
    }
}
