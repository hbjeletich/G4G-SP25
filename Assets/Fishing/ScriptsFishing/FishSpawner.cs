using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish Spawner Settings")]
    public int minFishAmount = 5;
    public int maxFishAmount = 8;

    [Header("Components")]
    public GameObject fishPrefab;
    public List<FishData> fishDataList;

    private void Start()
    {
        SpawnFish();
    }

    private void SpawnFish()
    {
        if (fishDataList == null || fishDataList.Count == 0)
        {
            Debug.LogError("FishSpawner: No FishData objects assigned! Add FishData to the list.");
            return;
        }

        int fishAmount = Random.Range(minFishAmount, maxFishAmount);
        for (int i = 0; i < fishAmount; i++)
        {
            FishData randomFish = fishDataList[Random.Range(0, fishDataList.Count)];
            GameObject fish = Instantiate(fishPrefab, transform.position, Quaternion.identity);
            FishObject fishObject = fish.GetComponent<FishObject>();
            Debug.Log("Spawning " + randomFish.fishName);

            fishObject.fishData = randomFish;
            fishObject.AssignBehaviorByRarity();

            fish.transform.SetParent(transform);
        }
    }
}
