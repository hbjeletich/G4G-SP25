using UnityEngine;

namespace CameraSnap
{

    [CreateAssetMenu(fileName = "AnimalData", menuName = "PhotoGame/Animal")]
    public class AnimalData : ScriptableObject
    {
        public string animalName;

        [Header("Zone assignment (0, 1, or 2)")]
        public int zoneIndex;

        [Header("Graphics")]
        public Sprite silhouetteSprite;
        public Sprite revealedSprite;

        [Header("In-game prefab")]
        public GameObject animalPrefab;
    }
}