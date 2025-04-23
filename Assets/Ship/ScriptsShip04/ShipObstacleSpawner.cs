using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

public class ShipObstacleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ObstacleType
    {
        public GameObject prefab;
        public int minPerRow = 0;
        public int maxPerRow = 2;

        // Higher weight = more likely to spawn
        public float spawnWeight = 1f;
    }

    [Header("Obstacle Settings")]
    public List<ObstacleType> obstacleTypes;
    // public float distanceBetweenRows = 5f;
    public float xSpawnRange = 4f;
    public int numberOfLanes = 5;

    [Header("Row Spacing (Y Axis)")]
    public float minDistanceBetweenRows = 4f;
    public float maxDistanceBetweenRows = 8f;

    [Header("Spawn Control")]
    public int maxRowsAhead = 10;
    public Transform player;
    public float spawnYOffset = 20f;

    [Header("Difficulty Ramping")]
    public float difficultyRampDuration = 120f; // time until max difficulty (in seconds)
    public AnimationCurve rowSpacingCurve; // 0 to 1 input, output spacing
    public AnimationCurve densityCurve; // 0 to 1 input, output max obstacles
    private float startTime;

    private float lastSpawnY = 0f;
    private Queue<GameObject> activeObstacles = new Queue<GameObject>();
    private bool spawningEnabled = false;

   private void Start()
    {
        lastSpawnY = player.position.y + spawnYOffset; // start ahead of the player
        spawningEnabled = true;
        startTime = Time.time; // Record the start time for difficulty ramping
    }

    private void Update()
    {
        float spawnThreshold = Camera.main.transform.position.y + spawnYOffset;
        float t = Mathf.Clamp01((Time.time - startTime) / difficultyRampDuration);
        float currentMinSpacing = Mathf.Lerp(11f, minDistanceBetweenRows, rowSpacingCurve.Evaluate(t));
        float currentMaxSpacing = Mathf.Lerp(11f, maxDistanceBetweenRows, rowSpacingCurve.Evaluate(t));

        while (lastSpawnY < spawnThreshold)
        {
            float randomizedDistance = Random.Range(minDistanceBetweenRows, maxDistanceBetweenRows);
            lastSpawnY += randomizedDistance;
            SpawnRow(lastSpawnY);
        }
        CleanUpObstacles();
    }

    void SpawnRow(float y)
    {
        float camHeight = 2f * Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        float leftBound = Camera.main.transform.position.x - camWidth / 2f;
        float rightBound = Camera.main.transform.position.x + camWidth / 2f;

        int obstaclesToSpawn = Random.Range(2, 4);

        float additionalBuffer = 0.5f;

        List<(float xPos, float halfWidth)> occupiedPositions = new List<(float, float)>();

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            GameObject chosen = ChooseRandomObstacle();
            if (chosen == null) continue;

            float halfWidth = chosen.GetComponent<Renderer>().bounds.extents.x;
            float xPos;
            int maxAttempts = 10;
            int attempts = 0;

            do
            {
                xPos = Random.Range(leftBound + halfWidth, rightBound - halfWidth);
                attempts++;
            }
            while (occupiedPositions.Exists(pos => Mathf.Abs(pos.xPos - xPos) < (pos.halfWidth + halfWidth + additionalBuffer)) && attempts < maxAttempts);

            if (attempts >= maxAttempts)
                continue;  

            occupiedPositions.Add((xPos, halfWidth));

            Vector3 spawnPos = new Vector3(xPos, y, -1.5f);
            GameObject instance = Instantiate(chosen, spawnPos, Quaternion.identity);
            activeObstacles.Enqueue(instance);
        }

        Debug.DrawLine(new Vector3(leftBound, y, 0), new Vector3(rightBound, y, 0), Color.green, 1f);
    }

    GameObject ChooseRandomObstacle()
    {
        float totalWeight = 0;
        foreach (var type in obstacleTypes)
            totalWeight += type.spawnWeight;

        float roll = Random.Range(0, totalWeight);
        float cumulative = 0;

        foreach (var type in obstacleTypes)
        {
            cumulative += type.spawnWeight;
            if (roll <= cumulative)
                return type.prefab;
        }

        return null;
    }

    void CleanUpObstacles()
    {
        float cleanupThreshold = Camera.main.transform.position.y - 10f;

        while (activeObstacles.Count > 0 && activeObstacles.Peek() != null)
        {
            GameObject obstacle = activeObstacles.Peek();
            if (obstacle.transform.position.y > cleanupThreshold)
                break;

            activeObstacles.Dequeue();
            Destroy(obstacle);
        }
    }
}
