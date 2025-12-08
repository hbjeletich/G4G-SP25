using UnityEngine;

namespace CameraSnap
{
    [CreateAssetMenu(fileName = "AnimalData", menuName = "PhotoGame/Animal")]
    public class AnimalData : ScriptableObject
    {
        [Header("Basic Info")]
        public string animalName;
        public GameObject animalPrefab;

        [Header("Animation")]
        public bool spriteFacesLeft = true;

        [Header("Behavior Settings")]
        public bool canWalk = true;
        

        [Header("Walking Settings")]
        public float moveSpeed = 1f;
        public float patrolDistance = 2f;
        public float idleTime = 2f;
        public float walkTime = 3f;

        [Header("UI Settings")]
        public Sprite foundImage;
        public Sprite silhouetteImage;
        public AudioClip captureSound;
    
        [Header("Spawn Settings")]
        [Tooltip("Select a rarity tier: 1 star = Common (most likely), 2 stars= Moderate, 3 stars = Rare (least likely).")]
        public SpawnRarity spawnRarity = SpawnRarity.Moderate;

        public enum SpawnRarity
        {
            Common = 1,
            Moderate = 2,
            Rare = 3
        }
    }
}
