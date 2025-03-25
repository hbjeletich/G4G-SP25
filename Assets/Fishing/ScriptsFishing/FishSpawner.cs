using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish Spawner Settings")]
    public float minFishAmount = 5f;
    public float maxFishAmount = 8f;

    [Header("Components")]
    public GameObject fishPrefab;
    public FishData[] fishData;           

    private void Start()
    {
        SpawnFish();
    }

    private void SpawnFish()
    {
        int fishAmount = Random.Range((int)minFishAmount, (int)maxFishAmount);
        for (int i = 0; i < fishAmount; i++)
        {
            FishData randomFish = fishData[Random.Range(0, fishData.Length)];
            GameObject fish = Instantiate(fishPrefab, transform.position, Quaternion.identity);
            fish.GetComponent<FishObject>().fishData = randomFish;
            fish.transform.SetParent(transform);
        }
    }
}
