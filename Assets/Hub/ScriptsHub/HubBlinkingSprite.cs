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
    private float alpha; // color alpha of image
    private bool fadingOut = true; // represents state

    void Start()
    {
        // checks for either UI image or sprite renderer
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
            // if fading out, decrease alpha
            alpha -= blinkSpeed * Time.deltaTime;
            if (alpha <= minAlpha)
            {
                // switch if faded
                alpha = minAlpha;
                fadingOut = false;
            }
        }
        else
        {
            // if fading in, increase alpha
            alpha += blinkSpeed * Time.deltaTime;
            if (alpha >= maxAlpha)
            {
                // switch if hit max
                alpha = maxAlpha;
                fadingOut = true;
            }
        }

        if (uiImage != null) // null check for UI image
        {
            // adjust color to fit transparency
            Color col = uiImage.color;
            col.a = alpha;
            uiImage.color = col;
        }
        else if (spriteRenderer != null) // backup for sprite renderer
        {
            // adjust color to fit transparency
            Color col = spriteRenderer.color;
            col.a = alpha;
            spriteRenderer.color = col;
        }
    }
}
