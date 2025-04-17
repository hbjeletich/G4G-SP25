using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swimming
{
    public class BubblesManager : MonoBehaviour
    {
        [Header("Bubble Prefabs")]
        [SerializeField] private GameObject[] bubblePrefabs;

        [Header("Spawn Settings")]
        [SerializeField] private int totalBubbles = 30;
        [SerializeField] private float spawnDuration = 2f;
        [SerializeField] private Vector2 screenBorderPadding = new Vector2(50f, 50f);
        [SerializeField] private float horizontalSpawnWidth = 30f;

        [Header("Bubble Properties")]
        [SerializeField] private float lifetimeMin = 3f;
        [SerializeField] private float lifetimeMax = 8f;
        [SerializeField] private float riseSpeedMin = 0.5f;
        [SerializeField] private float riseSpeedMax = 2.0f;
        [SerializeField] private float wobbleAmount = 0.3f;
        [SerializeField] private float wobbleSpeed = 1.5f;

        private Camera mainCamera;
        private List<GameObject> activeBubbles = new List<GameObject>();
        private PlayerController player;

        private void Awake()
        {
            mainCamera = Camera.main;
            player = FindObjectOfType<PlayerController>();

            if (player == null)
            {
                Debug.LogWarning("PlayerController not found - bubbles will use camera position only");
            }
        }

        private void Start()
        {
            StartCoroutine(SpawnBubbles());
        }

        private IEnumerator SpawnBubbles()
        {
            float delayBetweenSpawns = spawnDuration / totalBubbles;

            for (int i = 0; i < totalBubbles; i++)
            {
                SpawnBubble();
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }

        private void SpawnBubble()
        {
            if (bubblePrefabs.Length == 0)
            {
                Debug.LogError("No bubble prefabs assigned to BubbleSpawner!");
                return;
            }

            int randomIndex = Random.Range(0, bubblePrefabs.Length);
            GameObject bubblePrefab = bubblePrefabs[randomIndex];

            float randomX;

            // use player position for X if available
            if (player != null)
            {
                randomX = player.transform.position.x + Random.Range(-horizontalSpawnWidth / 2, horizontalSpawnWidth / 2);
            }
            else
            {
                // fallacbk to camera
                float screenWidth = 2f * mainCamera.orthographicSize * mainCamera.aspect;
                randomX = Random.Range(-screenWidth / 2 + screenBorderPadding.x, screenWidth / 2 - screenBorderPadding.x);
            }

            // just below bottom edge
            Vector3 bottomEdge = mainCamera.transform.position - new Vector3(0, mainCamera.orthographicSize, 0);
            Vector3 spawnPosition = new Vector3(randomX, bottomEdge.y - 1f, 0f);

            GameObject bubble = Instantiate(bubblePrefab, spawnPosition, Quaternion.identity, transform);
            activeBubbles.Add(bubble);

            StartCoroutine(AnimateBubble(bubble));
        }

        private IEnumerator AnimateBubble(GameObject bubble)
        {
            float lifetime = Random.Range(lifetimeMin, lifetimeMax);
            float riseSpeed = Random.Range(riseSpeedMin, riseSpeedMax);
            float wobblePhase = Random.Range(0f, 6.28f); // 0 to 2 pi

            Vector3 bubblePos = bubble.transform.position;
            float startTime = Time.time;

            // rise behavior
            while (Time.time - startTime < lifetime)
            {
                // update position
                float xOffset = Mathf.Sin((Time.time + wobblePhase) * wobbleSpeed) * wobbleAmount;
                bubblePos.y += riseSpeed * Time.deltaTime;
                bubblePos.x += xOffset * Time.deltaTime;

                bubble.transform.position = bubblePos;

                // slightly rotate the bubble
                bubble.transform.Rotate(0, 0, 15 * Time.deltaTime);

                yield return null;

                // check if bubble is far off screen
                Vector2 screenBounds = GetScreenBounds();
                if (bubble.transform.position.y > screenBounds.y + 2f)
                {
                    break; // end early if bubble has gone far beyond top of screen
                }
            }

            // remove and destroy bubble
            activeBubbles.Remove(bubble);
            Destroy(bubble);
        }

        private Vector2 GetScreenBounds()
        {
            float screenHeight = mainCamera.orthographicSize;
            float screenWidth = screenHeight * mainCamera.aspect;

            return new Vector2(screenWidth, screenHeight);
        }

        // method to stop all bubbles
        public void StopAllBubbles()
        {
            StopAllCoroutines();
            foreach (GameObject bubble in activeBubbles)
            {
                Destroy(bubble);
            }
            activeBubbles.Clear();
        }
    }
}

