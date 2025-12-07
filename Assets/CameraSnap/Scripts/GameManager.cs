using UnityEngine;
using System.Collections.Generic;

//This is more like an animal manager and handles the list of all animals in the game, what has been captured etc.



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

            
        }

       
        /// Returns the full list of animals that can appear anywhere. Used for spawning animals.
        public List<AnimalData> GetAllAnimals()
        {
            // Defensive: ensure callers always get a valid list
            return allAnimals ?? new List<AnimalData>();
        }

       
     
        public List<AnimalData> GetRandomTargets(int count)
        {
            var pool = GetAllAnimals();
            List<AnimalData> copy = new List<AnimalData>(pool);
            List<AnimalData> result = new List<AnimalData>();

            // Fisher-Yates shuffle-like selection: pick random unique entries
            for (int i = 0; i < count && copy.Count > 0; i++)
            {
                int idx = Random.Range(0, copy.Count);
                result.Add(copy[idx]);
                copy.RemoveAt(idx);
            }

            return result;
        }

      
        /// Returns a random animal from the full list.
        /// Picks a random one for spawning
       
        public AnimalData GetRandomAnimal()
        {
            var list = GetAllAnimals();
            if (list.Count == 0)
            {
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
        }

        // Checks if player has photographed the animal already
        public bool HasCaptured(string animalName) => capturedAnimals.Contains(animalName);

        // Gets the whole collection of captured animals
        public HashSet<string> GetCapturedAnimals() => capturedAnimals;


       
    }
}
