using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDiagonalScroller : MonoBehaviour
{
    public RectTransform targetRect;
    public float scrollSpeed = 50f; // pixels per second
    public float resetDistance = 1000f; // how far it scrolls before resetting

    private Vector2 originalPos;

    void Start()
    {
        if (targetRect == null)
            targetRect = GetComponent<RectTransform>();

        originalPos = targetRect.anchoredPosition;
    }

    void Update()
    {
        Vector2 newPos = targetRect.anchoredPosition + Vector2.up * scrollSpeed * Time.deltaTime;

        if (Mathf.Abs(newPos.y - originalPos.y) > resetDistance)
        {
            newPos.y = originalPos.y;
        }

        targetRect.anchoredPosition = newPos;
    }
}
