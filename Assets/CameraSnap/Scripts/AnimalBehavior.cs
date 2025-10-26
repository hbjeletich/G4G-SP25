using UnityEngine;
using System.Collections;

namespace CameraSnap
{
    public class AnimalBehavior : MonoBehaviour
    {
        [Header("Link to Animal Data")]
        public AnimalData animalData;
        [HideInInspector] public bool isCaptured = false;

        [Header("Animation Settings")]
        public Animator animator;
        public string walkBool = "isWalking";
        public string hideTrigger = "Hide";   //maybe change to bool, animal will pop up a few seconds and then go back down

        private Vector3 spawnPoint;
        private bool isWalking = false;
        private bool movingLeft = true;
        private Transform spriteChild;
        private Vector3 originalScale;

        private Camera mainCamera;

        void Start()
        {
            mainCamera = Camera.main;
            spawnPoint = transform.position;

           
          if (animator == null)
                animator = GetComponent<Animator>();


            // get sprite child safely (keeps your original approach but avoids null crash)
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                spriteChild = sr.transform;
                originalScale = spriteChild.localScale;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: No SpriteRenderer found in children.");
            }

            // INITIAL FLIP WITH DEFAULT FACING RESPECTED (inline like your original)
            bool defaultFacesLeft = animalData != null && animalData.spriteFacesLeft;

            // If sprite default matches movement - positive scale
            // If sprite default is opposite movement - negative scale
            int sign = (defaultFacesLeft == movingLeft) ? 1 : -1;

            if (spriteChild != null)
            {
                Vector3 s = originalScale;
                s.x = Mathf.Abs(originalScale.x) * sign;
                spriteChild.localScale = s;
            }

            StartCoroutine(BehaviorLoop());
        }

        IEnumerator BehaviorLoop()
        {
            while (true)
            {
                // Idle
                SetWalking(false);

                // use animalData idleTime (guard if animalData missing)
                float idle = (animalData != null) ? animalData.idleTime : 2f;
                yield return new WaitForSeconds(idle);

                // Walk if allowed (uses animalData.canWalk)
                bool canWalk = (animalData != null) ? animalData.canWalk : false;
                float walkTime = (animalData != null) ? animalData.walkTime : 3f;

                if (canWalk)
                {
                    SetWalking(true);
                    yield return new WaitForSeconds(walkTime);
                    SetWalking(false);
                }

                // Hide animation if enabled
                bool canHide = (animalData != null) ? animalData.canHideInBush : false;
                if (canHide && animator != null)
                {
                    animator.SetTrigger(hideTrigger);
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        void Update()
        {
            bool canWalk = (animalData != null) ? animalData.canWalk : false;
            if (!canWalk || !isWalking) return;

            float direction = movingLeft ? 1f : -1f;
            float moveSpeed = (animalData != null) ? animalData.moveSpeed : 1f;
            transform.position += new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);

            float patrolDistance = (animalData != null) ? animalData.patrolDistance : 2f;

            // Turn around at patrol edges so it does not walk off too far
            if (Vector3.Distance(transform.position, spawnPoint) >= patrolDistance)
            {
                movingLeft = !movingLeft;

                // flip sprite inline (like original)
                if (spriteChild != null)
                {
                    bool defaultFacesLeft = animalData != null && animalData.spriteFacesLeft;
                    int sign = (defaultFacesLeft == movingLeft) ? 1 : -1;

                    Vector3 s = originalScale;
                    s.x = Mathf.Abs(originalScale.x) * sign;
                    spriteChild.localScale = s;
                }
            }
        }

        void SetWalking(bool walking)
        {
            isWalking = walking;
            if (animator != null && !string.IsNullOrEmpty(walkBool))
                animator.SetBool(walkBool, walking);
        }

        void LateUpdate()
        {
            if (mainCamera == null)
                return;

            // Make sure animal faces the camera
            Vector3 direction = mainCamera.transform.position - transform.position;
            direction.y = 0f; // keep upright
            transform.rotation = Quaternion.LookRotation(-direction);
        }

        public void SetMovingDirection(bool isMovingLeft)
        {
            movingLeft = isMovingLeft;

            // Also flip the sprite immediately to match movement (uses transform.localScale like you originally had)
            Vector3 scale = transform.localScale;
            scale.x = isMovingLeft ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}
