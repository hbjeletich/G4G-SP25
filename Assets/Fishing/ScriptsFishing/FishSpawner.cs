using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Fishing
{
public class FishSpawner : MonoBehaviour
{
    [Header("Fish Spawner Settings")]
    public int minFishAmount = 5;
    public int maxFishAmount = 8;
    public Vector2 spawnAreaSize = new Vector2(10f, 5f); // Width & Height of rectangular spawn area
    public Vector2 spawnIntervalRange = new Vector2(3f, 8f); // Min & Max time btwn spawns
    public float minDistance = 1f; // Minimum distance btwn fish

    [Header("Fish Rarity Spawn Chances")]
    public float oneStarChance = 45f;
    public float twoStarChance = 25f;
    public float threeStarChance = 15f;
    public float fourStarChance = 9f;
    public float fiveStarChance = 6f;

    [Header("Components")]
    public GameObject fishPrefab;
    public List<FishData> fishDataList;

    [HideInInspector] public List<GameObject> activeFish = new List<GameObject>();

    private void Awake()
    {
        SpawnInitialFishAmount();
    }

    private void SpawnInitialFishAmount()
    {
        for (int i = 0; i < minFishAmount; i++)
        {
            SpawnFish();
        }
    }
    
    private void Start()
    {
        StartCoroutine(SpawnFishLoop());
    }

    private IEnumerator SpawnFishLoop()
    {
        while (true)
        {
            if (activeFish.Count < maxFishAmount)
            {
                SpawnFish();
            }
            yield return new WaitForSeconds(Random.Range(spawnIntervalRange.x, spawnIntervalRange.y));
        }
    }

    private void SpawnFish()
    {
        if (fishDataList == null || fishDataList.Count == 0)
        {
            Debug.LogError("FishSpawner: No FishData objects assigned!");
            return;
        }

        if (activeFish.Count >= maxFishAmount) return;

        FishData selectedFish = GetRandomFishByRarity();
        if (selectedFish == null) return;

        Vector2 spawnPosition = GetValidSpawnPosition(); // Ensure valid spacing

        GameObject fish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
        FishingAudioManager.instance.PlaySFX(FishingAudioManager.instance.splashSFX); // Play splash when spawned
        FishObject fishObject = fish.GetComponent<FishObject>();
        
        fishObject.fishData = selectedFish;
        fishObject.AssignBehaviorByRarity();
        fish.transform.SetParent(transform);

        activeFish.Add(fish);
    }

    private Vector2 GetValidSpawnPosition()
    {
        for (int attempt = 0; attempt < 10; attempt++) // Try 10 times to find a valid spot
        {
            Vector2 spawnPosition = GetRandomSpawnPosition();
            bool tooClose = false;

            foreach (GameObject fish in activeFish)
            {
                if (Vector2.Distance(spawnPosition, fish.transform.position) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }
            if (!tooClose) return spawnPosition; // Return a valid position
        }

        Debug.LogWarning("Could not find a good spawn position after 10 tries, spawning anyway.");
        return GetRandomSpawnPosition(); // If all else fails, spawn somewhere random
    }

    private FishData GetRandomFishByRarity()
    {
        float totalWeight = oneStarChance + twoStarChance + threeStarChance + fourStarChance + fiveStarChance;
        float randomValue = Random.Range(0, totalWeight);

        List<FishData> possibleFishes;

        if (randomValue < oneStarChance) 
            possibleFishes = fishDataList.Where(f => f.rarity == "★").ToList();
        else if ((randomValue -= oneStarChance) < twoStarChance) 
            possibleFishes = fishDataList.Where(f => f.rarity == "★★").ToList();
        else if ((randomValue -= twoStarChance) < threeStarChance) 
            possibleFishes = fishDataList.Where(f => f.rarity == "★★★").ToList();
        else if ((randomValue -= threeStarChance) < fourStarChance) 
            possibleFishes = fishDataList.Where(f => f.rarity == "★★★★").ToList();
        else 
            possibleFishes = fishDataList.Where(f => f.rarity == "★★★★★").ToList();

        // If we found matching fish, return a random one
        return (possibleFishes.Count > 0) ? possibleFishes[Random.Range(0, possibleFishes.Count)] : null;
    }

    private Vector2 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return (Vector2)transform.position + new Vector2(randomX, randomY);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
}
