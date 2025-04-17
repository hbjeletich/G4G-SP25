using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fishing
{
    public class FishingShowInventoryButton : MonoBehaviour
    {
        [Header("Inventory UI Elements")]
        public CanvasGroup inventoryCanvasGroup;
        public CanvasGroup inventoryButtonCanvasGroup;
        public FishingInventoryUI inventoryUI;

        private bool isInventoryVisible = false;

        private void Start()
        {
            InitialHide();
        }

        public void ToggleInventory()
        {
            if (isInventoryVisible)
            {
                HideInventory();
                ShowInventoryButton();
            }
            else
            {
                ShowInventory();
                HideInventoryButton();
            }
        }

        private void InitialHide()
        {
            inventoryCanvasGroup.alpha = 0f;
            inventoryCanvasGroup.interactable = false;
            inventoryCanvasGroup.blocksRaycasts = false;

            inventoryButtonCanvasGroup.alpha = 1f;
            inventoryButtonCanvasGroup.interactable = true;
            inventoryButtonCanvasGroup.blocksRaycasts = true;
        }   

        public void ShowInventory()
        {
            FishingAudioManager.instance.PlaySFX(FishingAudioManager.instance.clickSFX);
            
            inventoryCanvasGroup.alpha = 1f;
            inventoryCanvasGroup.interactable = true;
            inventoryCanvasGroup.blocksRaycasts = true;
            isInventoryVisible = true;

            inventoryUI.RefreshUI();
            Time.timeScale = 0f; // Pause the game when inventory is open
        }

        public void HideInventory()
        {
            FishingAudioManager.instance.PlaySFX(FishingAudioManager.instance.clickSFX);
            
            inventoryCanvasGroup.alpha = 0f;
            inventoryCanvasGroup.interactable = false;
            inventoryCanvasGroup.blocksRaycasts = false;
            isInventoryVisible = false;
            Time.timeScale = 1f; // Resume the game when inventory is closed
        }

        public void ShowInventoryButton()
        {
            inventoryButtonCanvasGroup.alpha = 1f;
            inventoryButtonCanvasGroup.interactable = true;
            inventoryButtonCanvasGroup.blocksRaycasts = true;
        }

        public void HideInventoryButton()
        {
            inventoryButtonCanvasGroup.alpha = 0f;
            inventoryButtonCanvasGroup.interactable = false;
            inventoryButtonCanvasGroup.blocksRaycasts = false;
        }
    }
}
