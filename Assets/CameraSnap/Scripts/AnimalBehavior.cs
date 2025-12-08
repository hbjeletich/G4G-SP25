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
    // Child sprite transform / renderer used for flipping without touching root rotation
    private Transform spriteChildTransform;
    private Vector3 spriteChildOriginalScale;
    private SpriteRenderer spriteRenderer;
    // If true, the spawner or another initializer already set the start direction
    private bool initialDirectionApplied = false;
    // Track previous movement direction to detect changes
    private bool prevMovingLeft;


        void Start()
        {
            startPosition = transform.position;




            if (animator == null) animator = GetComponent<Animator>();


            // cache child sprite renderer/transform for flipping
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteChildTransform = spriteRenderer.transform;
                spriteChildOriginalScale = spriteChildTransform.localScale;
            }


            if (!initialDirectionApplied)
            {
                isMovingLeft = animalData != null && animalData.spriteFacesLeft;
                ApplySpriteFlip();
            }


            prevMovingLeft = isMovingLeft;


            StartCoroutine(SimpleBehaviorLoop());
        }


        IEnumerator SimpleBehaviorLoop()
        {
            while (true)
            {
                animator?.SetBool("isWalking", false);
                float idle = animalData?.idleTime ?? 2f;
                yield return new WaitForSeconds(idle);


                if (animalData?.canWalk ?? false)
                {
                    float walk = animalData?.walkTime ?? 3f;
                    animator?.SetBool("isWalking", true);
                    isWalking = true;
                    yield return new WaitForSeconds(walk);
                    isWalking = false;
                    animator?.SetBool("isWalking", false);
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
            }


            // If direction changed this frame, update visual flip
            if (isMovingLeft != prevMovingLeft)
            {
                ApplySpriteFlip();
                prevMovingLeft = isMovingLeft;
            }
        }


        public void SetStartDirection(bool facingLeft)
        {
            isMovingLeft = facingLeft;
            initialDirectionApplied = true;
            ApplySpriteFlip();
        }


        private void ApplySpriteFlip()
        {
            bool desiredFacingLeft = isMovingLeft;


            if (spriteRenderer != null && animalData != null)
            {
                spriteRenderer.flipX = (animalData.spriteFacesLeft != desiredFacingLeft);
                return;
            }


            if (spriteChildTransform != null)
            {
                int sign = (animalData != null && animalData.spriteFacesLeft == desiredFacingLeft) ? 1 : -1;
                Vector3 s = spriteChildOriginalScale;
                s.x = Mathf.Abs(spriteChildOriginalScale.x) * sign;
                spriteChildTransform.localScale = s;
                return;
            }


            Vector3 scale = transform.localScale;
            scale.x = desiredFacingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
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


