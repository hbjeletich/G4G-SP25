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
        public bool canHideInBush = false;

        [Header("Walking Settings")]
        public float moveSpeed = 1f;
        public float patrolDistance = 2f;
        public float idleTime = 2f;
        public float walkTime = 3f;

        [Header("UI Settings")]
        public Sprite foundImage;
        public Sprite silhouetteImage;
        public AudioClip captureSound;
    }
}
