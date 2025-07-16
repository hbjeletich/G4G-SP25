using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public class ShipWaterSpawner : MonoBehaviour
    {
        [Header("Water Spawning Settings")]
        public GameObject waterTilePrefab;
        public float tileSize = 10f;
        public float spawnYOffset = 20f;
        public float xOffset = 4.7f;
        public float zOffset = -5.7f;

        private Camera playerCamera;
        private Queue<GameObject> activeTiles = new Queue<GameObject>();
        private float lastSpawnY = 0f;

        private void Start()
        {
            playerCamera = Camera.main;
            lastSpawnY = playerCamera.transform.position.y;
        }

        private void Update()
        {
            SpawnWaterTiles();
            CleanupWaterTiles();
        }

        private void SpawnWaterTiles()
        {
            float cameraY = playerCamera.transform.position.y;
            float spawnThreshold = cameraY + spawnYOffset;

            while (lastSpawnY < spawnThreshold)
            {
                Vector3 spawnPosition = new Vector3(xOffset, lastSpawnY, zOffset);

                // Rotate to XY plane (facing the camera)
                Quaternion waterRotation = Quaternion.Euler(90f, 0f, 0f);

                GameObject waterTile = Instantiate(waterTilePrefab, spawnPosition, waterRotation);
                activeTiles.Enqueue(waterTile);
                lastSpawnY += tileSize;
            }
        }

        private void CleanupWaterTiles()
        {
            float cameraY = playerCamera.transform.position.y;
            float cleanupThreshold = cameraY - 20f;

            while (activeTiles.Count > 0)
            {
                GameObject tile = activeTiles.Peek();

                if (tile == null)
                {
                    activeTiles.Dequeue();
                    continue;
                }

                if (tile.transform.position.y > cleanupThreshold)
                    break;

                activeTiles.Dequeue();
                Destroy(tile);
            }
        }
    }
}