using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingInventoryManager : MonoBehaviour
{
    public static FishingInventoryManager Instance; // Singleton for easy access

    private Dictionary<FishData, int> fishInventory = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddFish(FishData fish)
    {
        if (fishInventory.ContainsKey(fish))
            fishInventory[fish]++;
        else
            fishInventory[fish] = 1;
    }

    public Dictionary<FishData, int> GetInventory()
    {
        return fishInventory;
    }
}
