using UnityEngine;
using System.Collections;

namespace CameraSnap
{
    public class AnimalBehavior : MonoBehaviour
    {
        [Header("Link to Animal Data")]
        public AnimalData animalData;

        [Header("Animation Settings")]
        public Animator animator;
        public string walkBool = "isWalking";   
        public string hideTrigger = "Hide";   //maybe change to bool, animal will pop up a few seconds and then go back down 

        [Header("Behavior Toggles")]
        public bool canWalk = true;
        public bool canHideInBush = false;

        [Header("Walking Settings")]
        public float moveSpeed = 1f;
        public float patrolDistance = 2f;
        public float idleTime = 2f;
        public float walkTime = 3f;

        private Vector3 spawnPoint;
        private bool isWalking = false;
        private bool movingRight = true;

        void Start()
        {
            spawnPoint = transform.position;

            if (animator == null)
                animator = GetComponent<Animator>();

            StartCoroutine(BehaviorLoop());
        }

        IEnumerator BehaviorLoop()
        {
            while (true)
            {
                // Idle
                SetWalking(false);
                yield return new WaitForSeconds(idleTime);

                // Walk if allowed
                if (canWalk)
                {
                    SetWalking(true);
                    yield return new WaitForSeconds(walkTime);
                    SetWalking(false);
                }

                // Hide animation if enabled
                if (canHideInBush)
                {
                    animator.SetTrigger(hideTrigger);
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        void Update()
        {
            if (!canWalk || !isWalking) return;

            float direction = movingRight ? 1f : -1f;
            transform.position += new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);

            // Flip the sprite so it will walk in the direction it is facing
            Vector3 scale = transform.localScale;
            scale.x = movingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;

            // Turn around at patrol edges so it does not walk off too far
            if (Vector3.Distance(transform.position, spawnPoint) >= patrolDistance)
                movingRight = !movingRight;
        }

        void SetWalking(bool walking)
        {
            isWalking = walking;
            if (animator != null && !string.IsNullOrEmpty(walkBool))
                animator.SetBool(walkBool, walking);
        }
    }
}
