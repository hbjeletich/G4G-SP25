using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishing
{
    public class FishingSunRaySpawner : MonoBehaviour
    {
        public GameObject sunRayPrefab;
        public int raysPerBurst = 3;
        public float spawnInterval = 4f;
        public Vector2 spawnArea = new Vector2(5f, 3f);
        public Transform spawnCenter;

        private float timer = 0f;

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnRays();
            }
        }

        void SpawnRays()
        {
            for (int i = 0; i < raysPerBurst; i++)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-spawnArea.x / 5, spawnArea.x / 5),
                    Random.Range(-spawnArea.y / 2, spawnArea.y / 2),
                    0f
                );

                GameObject ray = Instantiate(sunRayPrefab, spawnCenter.position + offset, Quaternion.identity);
                ray.transform.SetParent(spawnCenter);
            }
        }
    }
}
