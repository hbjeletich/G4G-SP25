using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishing
{
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

// Reference   
// public class InventoryManager : MonoBehaviour
// {
//     public static InventoryManager instance;
//     private List<ItemScriptable> inventoryItems = new List<ItemScriptable>();

//     private void Awake()
//     {
//         if (instance == null)
//         {
//             instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }

//         Debug.Log("Inventory initialized. Item count: " + inventoryItems.Count);
//     }

//     private void Start()
//     {
//         LoadInventory(); // Load inventory from the current save slot
//     }

//     public void AddItem(ItemScriptable newItem)
//     {
//         if (!inventoryItems.Contains(newItem))
//         {
//             inventoryItems.Add(newItem);
//             SaveInventory(); // Save the inventory to the current save slot
//         }
//     }

//     public List<ItemScriptable> GetItemsByCategory(ItemScriptable.ItemType category)
//     {
//         return inventoryItems.FindAll(item => item.itemType == category);
//     }

//     public void SaveInventory()
//     {
//         GameData data = SaveManager.GetGameData();
        
//         // Update collected items
//         data.collectedItems.Clear();
//         foreach (var item in inventoryItems)
//         {
//             data.collectedItems.Add(item.item); // Save item names
//         }
        
//         SaveManager.SetGameData(data);
//         SaveManager.instance.SaveGameData(); // Save to the current save slot
//     }

//     public void LoadInventory()
//     {
//         GameData data = SaveManager.GetGameData();
//         inventoryItems.Clear(); // Clear current inventory

//         foreach (string itemName in data.collectedItems)
//         {
//             // Load items by name
//             ItemScriptable item = Resources.Load<ItemScriptable>("Items/" + itemName);
//             if (item != null)
//             {
//                 inventoryItems.Add(item);
//             }
//         }
//     }

//     public bool HasItem(ItemScriptable item)
//     {
//         return inventoryItems.Contains(item);
//     }
// }
}

