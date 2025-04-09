using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swimming
{
    public class CameraFollow : MonoBehaviour
    {
        private Vector3 offset;
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime;

        [SerializeField] private SpriteRenderer backgroundSprite;
        private float minX;
        private float maxX;
        private float minY;
        private float maxY;

        private Vector3 currentVelocity = Vector3.zero;
        private Camera camera;

        private void Awake()
        {
            offset = transform.position - target.position;
            camera = GetComponent<Camera>();
            CalculateBounds();
        }

        private void CalculateBounds()
        {
            // if no background sprite, skip this step
            if (backgroundSprite == null) return;

            float vertSize = camera.orthographicSize;
            float horizSize = vertSize * camera.aspect;

            float bgWidth = backgroundSprite.bounds.size.x;
            float bgHeight = backgroundSprite.bounds.size.y;

            Vector3 bgCenter = backgroundSprite.transform.position;

            minX = bgCenter.x - (bgWidth / 2) + horizSize;
            maxX = bgCenter.x + (bgWidth / 2) - horizSize;

            minY = bgCenter.y - (bgHeight / 2) + vertSize;
            maxY = bgCenter.y + (bgHeight / 2) - vertSize;
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = target.position + offset;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        }

    }
}
