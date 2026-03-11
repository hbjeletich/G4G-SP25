using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constellation
{
    public class StarEffect : MonoBehaviour
    {
        public Transform starEffectObject;
        public Renderer renderer;
        private Material material;
        private float distanceTo = 0f;
        public float maxDistance = 3f;

        public Vector3 totalMaxScale = new Vector3(1f, 1f, 1f);
        public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
        private Vector3 maxScale;
        public float scaleSpeed = 2f;

        private Constellation.PlayerController playerController = null;

        private CircleCollider2D starCollider;
        void Start()
        {
            starCollider = GetComponent<CircleCollider2D>();
            starCollider.isTrigger = true;

            // collider radius is max distance
            //maxDistance = starCollider.radius;

            material = renderer.material;
        }

        void Update()
        {
            if(playerController != null)
            {
                distanceTo = Vector3.Distance(transform.position, playerController.transform.position);
            }
            else
            {
                distanceTo = maxDistance;
            }

            material.SetFloat("_distanceTo", distanceTo);

            // pulse between min and max scale
            float scale = (Mathf.Sin(Time.time * scaleSpeed) + 1f)
                / 2f; // oscillates between 0 and 1
            Vector3 currentScale = Vector3.Lerp(minScale, maxScale, scale);
            starEffectObject.localScale = currentScale;

            // max scale gets smaller as distance decreases
            maxScale = Vector3.Lerp(totalMaxScale, minScale, distanceTo / maxDistance);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerController = other.GetComponent<Constellation.PlayerController>();
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerController = null;
            }
        }

        void OnDrawGizmos()
        {
            // draw a circle around the star to show the max distance
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxDistance);
        }


    }
}
