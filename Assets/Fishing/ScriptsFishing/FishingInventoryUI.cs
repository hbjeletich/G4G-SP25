using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fishing
{
    public class FishingInventoryUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject fishButtonPrefab;
        public Transform fishButtonContainer;
        public FishDetailsUI fishDetailsUI; 

        public int itemsPerPage = 12;
        private List<FishData> currentFishList = new();
        private int currentPage = 0;

        public void RefreshUI()
        {
            // Clear old buttons
            foreach (Transform child in fishButtonContainer)
            {
                Destroy(child.gameObject);
            }

            Dictionary<FishData, int> inventory = FishingInventoryManager.Instance.GetInventory();
            currentFishList = new List<FishData>(inventory.Keys);

            int startIndex = currentPage * itemsPerPage;
            int endIndex = Mathf.Min(startIndex + itemsPerPage, currentFishList.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                FishData fish = currentFishList[i];
                int count = inventory[fish];

                GameObject buttonGO = Instantiate(fishButtonPrefab, fishButtonContainer);
                
                // Setup the visual
                FishInventoryButton inventoryButton = buttonGO.GetComponent<FishInventoryButton>();
                inventoryButton.Setup(fish, count, fish.fishSprite);

                // Add click event
                Button uiButton = buttonGO.GetComponent<Button>();
                uiButton.onClick.AddListener(() => fishDetailsUI.ShowFishDetails(fish));
            }
        }

        public void NextPage()
        {
            if ((currentPage + 1) * itemsPerPage < currentFishList.Count)
            {
                currentPage++;
                RefreshUI();
            }
            else
            {
                FishingAudioManager.instance.PlaySFX(FishingAudioManager.instance.cantSFX);
                Debug.Log("No more pages available.");
            }
        }

        public void PreviousPage()
        {
            if (currentPage > 0)
            {
                currentPage--;
                RefreshUI();
            }
            else
            {
                FishingAudioManager.instance.PlaySFX(FishingAudioManager.instance.cantSFX);
                Debug.Log("No previous pages available.");
            }
        }
    }
}
