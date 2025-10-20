using UnityEngine;

//This is to create a scriptable object for the animals that appear in game. I used this idea from seeing the fishing game in 
//previous g4g projects. This includes data like the animal name and the prefab that the animal uses so I can use it in code to 
//spawn animals.

namespace CameraSnap
{
    [CreateAssetMenu(fileName = "AnimalData", menuName = "PhotoGame/Animal")]
    public class AnimalData : ScriptableObject
    {
        [Header("Animal Info")]
        public string animalName;

        [Header("In-Game Prefab")]
        public GameObject animalPrefab;

        
    }
}
