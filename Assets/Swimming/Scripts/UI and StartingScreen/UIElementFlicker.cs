using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Swimming
{
    public class UIElementFlicker : MonoBehaviour
    {
        [SerializeField] private float flickerDuration = 0.5f;
        [SerializeField] private int flickerCount = 3;
        [SerializeField] private Image imageComponent;

        private void Awake()
        {
            if (imageComponent == null)
            {
                imageComponent = GetComponent<Image>();
            }
        }

        private void OnEnable()
        {
            StartCoroutine(AnimateElement());
        }

        private IEnumerator AnimateElement()
        {
            // first do flicker effect
            float flickerInterval = flickerDuration / (flickerCount * 2);

            if (imageComponent != null)
            {
                Color originalColor = imageComponent.color;

                for (int i = 0; i < flickerCount; i++)
                {
                    // off
                    imageComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
                    yield return new WaitForSeconds(flickerInterval);

                    // on
                    imageComponent.color = originalColor;
                    yield return new WaitForSeconds(flickerInterval);
                }
            }
        }
    }
}