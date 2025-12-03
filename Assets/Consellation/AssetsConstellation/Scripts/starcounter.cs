using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

namespace Constellation
{
    public class StarCounterPositionBased : MonoBehaviour
    {
        [SerializeField] private TMP_Text counterText; // optional UI
        [SerializeField] private float checkInterval = 0.1f; // seconds between checks
        [SerializeField] private float tolerance = 0.5f; // distance threshold to consider "home"

        private StarScript[] stars;
        private DestinationScript[] destinations;
        private int totalStars;
        private int starsAtHome;

        void Start()
        {
            // Find all stars and destinations
            stars = FindObjectsOfType<StarScript>();
            destinations = FindObjectsOfType<DestinationScript>();
            totalStars = stars.Length;
            starsAtHome = 0;

            if (totalStars == 0)
                Debug.LogWarning("StarCounterPositionBased: No stars found in scene.");

            // Auto-create TMP text if none assigned
            if (counterText == null)
            {
                GameObject textObj = new GameObject("StarCounterText");
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    GameObject canvasGO = new GameObject("AutoCounterCanvas");
                    canvas = canvasGO.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasGO.AddComponent<CanvasScaler>();
                    canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                }
                textObj.transform.SetParent(canvas.transform);
                counterText = textObj.AddComponent<TextMeshProUGUI>();
                counterText.fontSize = 36;
                counterText.alignment = TextAlignmentOptions.TopRight;
                counterText.color = Color.white;

                RectTransform rt = counterText.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(1, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.pivot = new Vector2(1, 1);
                rt.anchoredPosition = new Vector2(-20, -20);
            }

            UpdateCounter();
            StartCoroutine(CheckStarsRoutine());
        }

        private IEnumerator CheckStarsRoutine()
        {
            while (true)
            {
                starsAtHome = 0;

                for (int i = 0; i < stars.Length; i++)
                {
                    if (stars[i] == null || destinations[i] == null)
                        continue;

                    float distance = Vector3.Distance(stars[i].transform.position, destinations[i].transform.position);
                    if (distance <= tolerance)
                        starsAtHome++;
                }

                UpdateCounter();
                yield return new WaitForSeconds(checkInterval);
            }
        }

        private void UpdateCounter()
        {
            if (counterText != null)
                counterText.text = $"{starsAtHome}/{totalStars}";
        }
    }
}
