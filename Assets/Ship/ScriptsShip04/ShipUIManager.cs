using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Ship
{
public class ShipUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;

    [Header("Popup Stuff")]
    public GameObject floatingPopupPrefab;
    public RectTransform popupParent;
    public Transform playerTransform;

    private void Awake()
    {
        scoreText.text = "Score: 0";
        healthText.text = "Health: 4";
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateHealth(float health)
    {
        healthText.text = "Health: " + health;
    }

    // public void ShowFloatingText(string text, Vector3 worldPosition, Color color)
    // {
    //     if (floatingPopupPrefab == null || popupParent == null || playerTransform == null)
    //     {
    //         Debug.LogWarning("Popup prefab, popup parent, or player transform not assigned!");
    //         return;
    //     }

    //     // Convert world position to screen space
    //     Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

    //     // Create popup and assign position
    //     GameObject popup = Instantiate(floatingPopupPrefab, popupParent);
    //     RectTransform popupRect = popup.GetComponent<RectTransform>();

    //     // Convert screen position to local UI position
    //     Vector2 localPosition;
    //     if (RectTransformUtility.ScreenPointToLocalPointInRectangle(popupParent, screenPosition, Camera.main, out localPosition))
    //     {
    //         popupRect.anchoredPosition = localPosition;
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Failed to convert screen point to local point.");
    //     }

    //     // Setup popup content
    //     PointsGainPopup popupScript = popup.GetComponent<PointsGainPopup>();
    //     if (popupScript != null)
    //     {
    //         popupScript.Setup(text, color);
    //     }

    //     Debug.Log($"Popup created at {popupRect.anchoredPosition} (World Pos: {worldPosition}, Screen Pos: {screenPosition})");
    // }
}
}
