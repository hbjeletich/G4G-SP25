using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swimming
{
    public class Fish : MonoBehaviour
    {
        // movement
        [SerializeField] private float normalSpeed = 1.5f;
        [SerializeField] private float fleeSpeed = 4f;
        [SerializeField] private float rotationSpeed = 2f;

        // flee mechanic
        [SerializeField] private float detectionRadius = 3f;
        [SerializeField] private float fleeThreshold = 2f;

        // components
        private Rigidbody2D rigidbody2D;
        private CircleCollider2D detectionCollider;
        private SpriteRenderer spriteRenderer;

        private Collider2D spawnAreaCollider;

        // state variables
        private Vector2 moveDirection;
        private bool isFleeing = false;
        private Transform playerTransform;
        private Rigidbody2D playerRigidbody;

        // i didnt end up adding fleeing because it would change the player speed stuff, but it will be there!

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            detectionCollider = gameObject.AddComponent<CircleCollider2D>();
            detectionCollider.radius = detectionRadius;
            detectionCollider.isTrigger = true;

            // random direction
            float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
            moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        void Start()
        {
            // find player
            playerTransform = FindFirstObjectByType<PlayerController>().transform;
            if (playerTransform != null)
            {
                playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();
            }

            spawnAreaCollider = transform.parent.parent.GetComponent<Collider2D>();
        }

        void FixedUpdate()
        {
            Vector2 direction = moveDirection;

            if (isFleeing && playerTransform != null)
            {
                // if fleeing, move away from player
                Vector2 awayFromPlayer = (Vector2)transform.position - (Vector2)playerTransform.position;
                direction = awayFromPlayer.normalized;
            }
            else
            {
                // random direction changes when swimming normally
                if (Random.value < 0.02f)
                {
                    direction += Random.insideUnitCircle * 0.3f;
                    direction.Normalize();
                    moveDirection = direction;
                }
            }

            // Check if fish is about to leave its boundary and adjust if needed
            if (spawnAreaCollider != null)
            {
                CheckAndAdjustDirection(ref direction);
            }

            // apply speeed
            float speed;
            if (isFleeing) speed = fleeSpeed;
            else speed = normalSpeed;
            rigidbody2D.velocity = direction * speed;
        }

        private void CheckAndAdjustDirection(ref Vector2 direction)
        {
            // check if leaving boundary
            Vector2 futurePosition = (Vector2)transform.position + direction * 1.5f;

            if (!spawnAreaCollider.OverlapPoint(futurePosition))
            {

                // switch direction
                direction = -direction;
                moveDirection = direction;
            }
        }
    }
}
