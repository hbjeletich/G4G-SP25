using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Fishing
{
public class FishingInventoryUI : MonoBehaviour
{
    public Transform inventoryPanel;
    public GameObject fishEntryPrefab;

    public Image fishImage;  
    public Text fishName;   
    public Text rarity;
    public Text lengthRange;

    private Dictionary<FishData, GameObject> uiEntries = new();

    public void UpdateInventoryUI()
    {
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
}
