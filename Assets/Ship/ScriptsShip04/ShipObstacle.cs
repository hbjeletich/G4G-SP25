using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
public class ShipObstacle : MonoBehaviour
{
    private Collider obstacleCollider;
    private Renderer obstacleRenderer;

    private void Awake()
    {
        obstacleCollider = GetComponent<Collider>();
        obstacleRenderer = GetComponent<Renderer>();
    }

    public void TriggerObstacleEffect()
    {
        obstacleCollider.enabled = false; 
        StartCoroutine(FadeOutObstacle());
    }
    
    private IEnumerator FadeOutObstacle()
    {
        // Ensure the material supports transparency
        obstacleRenderer.material.SetFloat("_Surface", 1); // 🔹 1 = Transparent Mode for URP
        obstacleRenderer.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        Color originalColor = obstacleRenderer.material.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.05f); // Almost invisible
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(originalColor.a, targetColor.a, elapsedTime / duration);
            obstacleRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obstacleRenderer.material.color = targetColor;
    }
}
}
