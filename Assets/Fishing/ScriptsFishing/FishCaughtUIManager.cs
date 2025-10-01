using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Fishing
{
    public class FishCaughtUIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public CanvasGroup canvasGroup;
        public Image fishImage;
        public TextMeshProUGUI fishNameText;
        public TextMeshProUGUI lengthText;
        public TextMeshProUGUI rarityText;
        public CanvasGroup newCatchCanvasGroup;

        [Header("Display Settings")]
        public float fadeDuration = 0.3f;
        public float displayDuration = 5f;

        [Header("Badge Icons")]
        public Image rarityIcon;
        public Sprite commonIcon;
        public Sprite uncommonIcon;
        public Sprite rareIcon;
        public Sprite epicIcon;
        public Sprite legendaryIcon;

        private Coroutine currentRoutine;
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

        public void ShowFish(FishData fishData, float length, bool isNewCatch)
        {
            fishImage.sprite = fishData.fishSprite;
            fishNameText.text = fishData.fishName;
            lengthText.text = $"{length:0.0} cm";

            if (rarityInfo.TryGetValue(fishData.rarity, out var info))
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

            if (newCatchCanvasGroup != null)
            {
                newCatchCanvasGroup.alpha = isNewCatch ? 1f : 0f;
                newCatchCanvasGroup.interactable = isNewCatch;
                newCatchCanvasGroup.blocksRaycasts = isNewCatch;
            }

            if (currentRoutine != null)
                StopCoroutine(currentRoutine);

            currentRoutine = StartCoroutine(DisplayRoutine());
        }

        private IEnumerator DisplayRoutine()
        {
            yield return StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));
            yield return new WaitForSecondsRealtime(displayDuration);
            yield return StartCoroutine(FadeCanvasGroup(1f, 0f, fadeDuration));
            currentRoutine = null;

            // Resume time
            Time.timeScale = 1f;
        }

        private IEnumerator FadeCanvasGroup(float from, float to, float duration)
        {
            float elapsed = 0f;
            canvasGroup.alpha = from;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = to;
            canvasGroup.interactable = to > 0f;
            canvasGroup.blocksRaycasts = to > 0f;
        }
    }
}
