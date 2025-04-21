using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Ship
{
public class PointsGainPopup : MonoBehaviour
{
    [Header("Popup Settings")]
    public TextMeshProUGUI popupText;
    public float floatSpeed = 50f;
    public float fadeDuration = 1f;
    public Vector3 floatOffset = new Vector3(0, 75f, 0);

    private CanvasGroup canvasGroup;
    private Vector3 startPos;
    private float timer;

    private void Awake()
    {
        popupText = GetComponent<TextMeshProUGUI>();
        if (popupText == null)
        {
            popupText = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (popupText == null)  // Fallback 
        {
            Debug.LogError("TextMeshProUGUI component not found on the object or its children.");
        }

        canvasGroup = GetComponent<CanvasGroup>();
        startPos = transform.localPosition;
    }

    public void Setup(string text, Color color)
    {
        popupText.text = text;
        popupText.color = color;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Move upward
        transform.localPosition = startPos + floatOffset * (timer / fadeDuration);

        // Fade out
        canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

        if (timer >= fadeDuration)
            Destroy(gameObject);
    }
}
}
