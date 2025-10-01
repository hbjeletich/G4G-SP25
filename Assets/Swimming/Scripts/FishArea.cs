using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swimming
{
    public class FishArea : MonoBehaviour
    {
        [SerializeField] private GameObject fishPrefab;
        [SerializeField] private int minFishPerSchool = 3;
        [SerializeField] private int maxFishPerSchool = 7;

        private Collider2D spawnAreaCollider;

        void Awake()
        {
            spawnAreaCollider = GetComponent<Collider2D>();
        }

        void Start()
        {
            SpawnSchool();
            SpawnSchool();
            SpawnSchool();
            SpawnSchool();
            SpawnSchool();
        }


        private void Update()
        {
            
        }

        public void SpawnSchool()
        {
            Vector3 schoolSpawnPoint = GetRandomPositionInCollider();

            GameObject schoolParent = new GameObject("School");
            schoolParent.transform.position = schoolSpawnPoint;
            schoolParent.transform.parent = transform;

            int schoolSize = Random.Range(minFishPerSchool, maxFishPerSchool + 1);

            float schoolRadius = 6f;

            // spawn individual fish
            for (int i = 0; i < schoolSize; i++)
            {
                // random offset near the school center
                Vector2 offset = Random.insideUnitCircle * schoolRadius;
                Vector3 fishPosition = schoolSpawnPoint + new Vector3(offset.x, offset.y, 0);


                // instantiate fish
                GameObject fish = Instantiate(fishPrefab, fishPosition, Quaternion.identity, schoolParent.transform);

                // slight variation in scale
                fish.transform.localScale = Vector3.one * Random.Range(0.3f, 0.5f);
            }

        }

        private Vector3 GetRandomPositionInCollider()
        {
            Bounds bounds = spawnAreaCollider.bounds;

            for (int attempt = 0; attempt < 30; attempt++)
            {
                Vector3 randomPos = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    0
                );

                if (spawnAreaCollider.OverlapPoint(randomPos))
                {
                    return randomPos;
                }
            }
            return spawnAreaCollider.bounds.center;
        }
    }
}