using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HubBlinkingSprite : MonoBehaviour
{
    public float blinkSpeed = 1f; // How fast it fades
    public float minAlpha = 0.2f;
    public float maxAlpha = 1f;

    private Image uiImage;
    private SpriteRenderer spriteRenderer;
    private float alpha;
    private bool fadingOut = true;

    void Start()
    {
        uiImage = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        alpha = maxAlpha;

        if (uiImage == null && spriteRenderer == null)
        {
            Debug.LogWarning("No Image or SpriteRenderer found on this GameObject!");
        }
    }

    void Update()
    {
        if (fadingOut)
        {
            alpha -= blinkSpeed * Time.deltaTime;
            if (alpha <= minAlpha)
            {
                alpha = minAlpha;
                fadingOut = false;
            }
        }
        else
        {
            alpha += blinkSpeed * Time.deltaTime;
            if (alpha >= maxAlpha)
            {
                alpha = maxAlpha;
                fadingOut = true;
            }
        }

        if (uiImage != null)
        {
            Color col = uiImage.color;
            col.a = alpha;
            uiImage.color = col;
        }
        else if (spriteRenderer != null)
        {
            Color col = spriteRenderer.color;
            col.a = alpha;
            spriteRenderer.color = col;
        }
    }
}
