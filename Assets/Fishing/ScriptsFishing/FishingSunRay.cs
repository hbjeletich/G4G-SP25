using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

namespace Fishing
{
    public class FishingSunRay : MonoBehaviour
    {
        public float lifetime = 5f;
        public float fadeInTime = 1f;
        public float fadeOutTime = 1f;
        public Vector2 movementOffset = new Vector2(0.2f, 0.1f);

        private SpriteRenderer sr;
        private float timer;
        private Vector3 startPos;
        private Vector3 endPos;

        private Color targetColor = new Color(1f, 0.937f, 0.807f, 0.252f); 
        private Color transparentColor;

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            timer = 0f;

            transparentColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
            sr.color = transparentColor;

            startPos = transform.position;
            endPos = startPos + new Vector3(
                Random.Range(-movementOffset.x, movementOffset.x),
                Random.Range(-movementOffset.y, movementOffset.y),
                0f
            );
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer < fadeInTime)
            {
                float t = timer / fadeInTime;
                sr.color = Color.Lerp(transparentColor, targetColor, t);
            }
            else if (timer > lifetime - fadeOutTime)
            {
                float t = (timer - (lifetime - fadeOutTime)) / fadeOutTime;
                sr.color = Color.Lerp(targetColor, transparentColor, t);
            }
            else
            {
                sr.color = targetColor;
            }

            // Handle motion
            float moveProgress = Mathf.Clamp01(timer / lifetime);
            transform.position = Vector3.Lerp(startPos, endPos, moveProgress);

            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}

