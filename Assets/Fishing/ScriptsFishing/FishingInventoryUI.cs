using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FishingInventoryUI : MonoBehaviour
{
    public Transform inventoryPanel; // Parent UI element for inventory
    public GameObject fishEntryPrefab; // Prefab for each fish entry

    public Image fishImage;  // Displays selected fish sprite
    public Text fishName;    // Displays selected fish name
    public Text rarity;      // Displays rarity
    public Text lengthRange; // Displays min-max length

    private Dictionary<FishData, GameObject> uiEntries = new();

    public void UpdateInventoryUI()
    {
        // Clear previous UI entries (optional for refreshing)
        foreach (Transform child in inventoryPanel)
            Destroy(child.gameObject);
        uiEntries.Clear();

        // Get inventory data
        Dictionary<FishData, int> inventory = FishingInventoryManager.Instance.GetInventory();

        foreach (var item in inventory)
        {
            FishData fish = item.Key;
            int count = item.Value;

            // Instantiate a new UI entry
            GameObject entry = Instantiate(fishEntryPrefab, inventoryPanel);
            entry.GetComponentInChildren<Text>().text = $"{fish.fishName} x{count}";

            // Set fish image
            entry.transform.Find("FishImage").GetComponent<Image>().sprite = fish.fishSprite;

            // Add click event to show details
            entry.GetComponent<Button>().onClick.AddListener(() => DisplayFishDetails(fish));

            uiEntries[fish] = entry;
        }
    }

    public void DisplayFishDetails(FishData fish)
    {
        fishImage.sprite = fish.fishSprite;
        fishName.text = fish.fishName;
        rarity.text = $"Rarity: {fish.rarity}";
        lengthRange.text = $"Size: {fish.lengthRange}";
    }
}
