using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Fishing
{
    public class FishInventoryButton : MonoBehaviour
    {
        private Image fishIconImage;
        private TextMeshProUGUI countText;
        private FishData fishData;

        private void Awake()
        {
            // Initialize components
            fishIconImage = GetComponentInChildren<Image>();
            countText = GetComponentInChildren<TextMeshProUGUI>();
        }
        public void Setup(FishData fish, int count, Sprite fishSprite)
        {
            fishData = fish;
            fishIconImage.sprite = fishSprite;
            countText.text = $"{count}";
        }

        public FishData GetFishData()
        {
            return fishData;
        }
    }
}
