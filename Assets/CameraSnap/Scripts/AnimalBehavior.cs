using UnityEngine;
using System.Collections;

namespace CameraSnap
{
    // Simple animal behavior - walks back and forth, faces camera, plays animations
    public class AnimalBehavior : MonoBehaviour
    {
        [Header("Settings")]
        public AnimalData animalData;
        public Animator animator;

        [HideInInspector] 
        public bool isCaptured = false;

        private Vector3 startPosition;
        private bool isWalking;
        private bool isMovingLeft;

        void Start()
        {
            startPosition = transform.position;
            isMovingLeft = animalData != null && animalData.spriteFacesLeft;
            
            if (animator == null)
                animator = GetComponent<Animator>();

            StartCoroutine(SimpleBehaviorLoop());
        }

        IEnumerator SimpleBehaviorLoop()
        {
            while (true)
            {
                // Stand still
                animator?.SetBool("isWalking", false);
                yield return new WaitForSeconds(2f);

                if (animalData?.canWalk ?? false)
                {
                    // Walk for a bit
                    animator?.SetBool("isWalking", true);
                    isWalking = true;
                    yield return new WaitForSeconds(3f);
                    isWalking = false;
                    animator?.SetBool("isWalking", false);
                }

                // Maybe hide
                if (animalData?.canHideInBush ?? false)
                {
                    animator?.SetTrigger("Hide");
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        void Update()
        {
            if (!isWalking) return;

            // Simple movement
            float direction = isMovingLeft ? -1f : 1f;
            float speed = animalData?.moveSpeed ?? 1f;
            transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);

            // Turn around if too far
            float maxDistance = animalData?.patrolDistance ?? 2f;
            if (Mathf.Abs(transform.position.x - startPosition.x) >= maxDistance)
            {
                isMovingLeft = !isMovingLeft;
                // Use same flipping logic as SetStartDirection to respect spriteFacesLeft
                Vector3 scale = transform.localScale;
                scale.x = isMovingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }

        public void SetStartDirection(bool facingLeft)
        {
            isMovingLeft = facingLeft;
            Vector3 scale = transform.localScale;
            scale.x = facingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        void LateUpdate()
        {
            // Billboard effect - make sprite always face the camera
            if (Camera.main != null)
            {
                Vector3 dirToCamera = Camera.main.transform.position - transform.position;
                dirToCamera.y = 0; // Keep the sprite upright
                transform.rotation = Quaternion.LookRotation(-dirToCamera);
            }
        }
    }
}
