using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Fishing
{
    public class FishDetailsUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Image fishImage;
        public TextMeshProUGUI fishNameText;
        public TextMeshProUGUI lengthText;
        public TextMeshProUGUI rarityText;
        public TextMeshProUGUI funFactText;
        public Image rarityIcon;

        [Header("Badge Icons")]
        public Sprite commonIcon;
        public Sprite uncommonIcon;
        public Sprite rareIcon;
        public Sprite epicIcon;
        public Sprite legendaryIcon;

        private Dictionary<string, (string label, Color color, Sprite icon)> rarityInfo;

        private void Awake()
        {
            rarityInfo = new Dictionary<string, (string, Color, Sprite)>
            {
                { "★",     ("Common", Color.black, commonIcon) },
                { "★★",    ("Uncommon", Color.black, uncommonIcon) },
                { "★★★",   ("Rare", new Color32(87, 46, 178, 255), rareIcon) },
                { "★★★★",  ("Epic", new Color32(145, 23, 24, 255), epicIcon) },
                { "★★★★★", ("Legendary", new Color32(170, 119, 16, 255), legendaryIcon) }
            };
        }

        public void ShowFishDetails(FishData fish)
        {
            FishingAudioManager.instance.PlaySFX(FishingAudioManager.instance.clickSFX);
            fishNameText.text = fish.fishName;
            fishImage.sprite = fish.fishSprite;
            lengthText.text = $"{fish.lengthRange.min:0.0} - {fish.lengthRange.max:0.0} cm";
            funFactText.text = $"{fish.funFact}";

            if (rarityInfo.TryGetValue(fish.rarity, out var info))
            {
                rarityText.text = info.label;
                rarityText.color = info.color;
                rarityIcon.sprite = info.icon;
                rarityIcon.enabled = info.icon != null;
            }
            else
            {
                rarityText.text = "Unknown";
                rarityText.color = Color.white;
                rarityIcon.enabled = false;
            }
        }
    }
}
