using UnityEngine;

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
