using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishObject : MonoBehaviour
{
    [Header("Fish Data")]
    public FishData fishData;  // Assigned by FishSpawner

    [Header("Size Scaling (Global for All Fish)")]
    public SizeRange tinySize = new SizeRange { min = 0.05f, max = 0.065f };
    public SizeRange smallSize = new SizeRange { min = 0.065f, max = 0.075f };
    public SizeRange mediumSize = new SizeRange { min = 0.075f, max = 0.08f };
    public SizeRange largeSize = new SizeRange { min = 0.08f, max = 0.095f };
    public SizeRange hugeSize = new SizeRange { min = 0.095f, max = 1.1f };
    public float minWidthFactor = 0.5f; // Thinnest fish
    public float maxWidthFactor = 1.1f; // Fattest fish

    [Header("Avoidance Settings")]
    public float avoidanceRadius = 1f; // Radius to detect nearby fish for avoidance
    public float avoidanceStrength = 1f; // How much to steer away from nearby fish

    [Header("Fish Behavior Settings By Rarity")]
    [Header("★")]
    public float oneStarMoveSpeed = 1.5f;
    public float oneStarMinMoveTime = 3f;
    public float oneStarMaxMoveTime = 6f;
    public float oneStarMinStopTime = 2f;
    public float oneStarMaxStopTime = 4f;
    public float oneStarTailWagSpeed = 2f;

    [Header("★★")]
    public float twoStarMoveSpeed = 2f;
    public float twoStarMinMoveTime = 2.5f;
    public float twoStarMaxMoveTime = 5f;
    public float twoStarMinStopTime = 1.5f;
    public float twoStarMaxStopTime = 3f;
    public float twoStarTailWagSpeed = 2.5f;

    [Header("★★★")]
    public float threeStarMoveSpeed = 2.5f;
    public float threeStarMinMoveTime = 2f;
    public float threeStarMaxMoveTime = 4f;
    public float threeStarMinStopTime = 1f;
    public float threeStarMaxStopTime = 2.5f;
    public float threeStarTailWagSpeed = 3f;

    [Header("★★★★")]
    public float fourStarMoveSpeed = 3f;
    public float fourStarMinMoveTime = 1.5f;
    public float fourStarMaxMoveTime = 3.5f;
    public float fourStarMinStopTime = 1f;
    public float fourStarMaxStopTime = 2f;
    public float fourStarTailWagSpeed = 3.5f;

    [Header("★★★★★")]
    public float fiveStarMoveSpeed = 3.5f;
    public float fiveStarMinMoveTime = 1f;
    public float fiveStarMaxMoveTime = 3f;
    public float fiveStarMinStopTime = 0.5f;
    public float fiveStarMaxStopTime = 1.5f;
    public float fiveStarTailWagSpeed = 4f;

    // Components + internal variables
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;
    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isMoving = false;
    private bool isTurning = false;
    private float moveSpeed, minMoveTime, maxMoveTime, minStopTime, maxStopTime, tailWagSpeed;
    private Quaternion targetRotation;

    // Movement variables
    private float currentSpeed = 0f; // Used for acceleration/deceleration
    private float accelerationTime = 1f; // Time to reach full speed
    private float decelerationTime = 1f; // Time to fully stop
    private float accelerationRate;
    private float decelerationRate;
    private float targetSpeed; // Randomized speed per movement phase
    private bool isDecelerating = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        AssignBehaviorByRarity();
        AssignFishSize();
        
        StartCoroutine(FishBehaviorLoop());
    }

    void Update()
    {
        if (isTurning)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                isTurning = false;
                isMoving = true; // Start moving AFTER turning finishes
                targetSpeed = Random.Range(moveSpeed * 0.5f, moveSpeed * 1.5f); // Randomized speed
                accelerationRate = targetSpeed / accelerationTime;
                decelerationRate = targetSpeed / decelerationTime;
                isDecelerating = false;
                currentSpeed = 0f; // Reset speed before accelerating
            }
        }

        if (isMoving && !isDecelerating)
        {
            AvoidNearbyFish();
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelerationRate * Time.deltaTime);
            MoveForward();
        }
        else if (isDecelerating)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, decelerationRate * Time.deltaTime);
            MoveForward();
            if (currentSpeed <= 0.01f) // Ensure it stops completely
            {
                currentSpeed = 0;
                isDecelerating = false;
            }
        }
        
        TailWag();
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     Debug.Log("Fish collided with: " + collision.gameObject.name);
    // }

    private void MoveForward()
    {
        transform.position += -transform.up * currentSpeed * Time.deltaTime;
    }

    private IEnumerator FishBehaviorLoop()
    {
        while (true)
        {
            isMoving = false;
            isDecelerating = true;
            yield return new WaitUntil(() => currentSpeed == 0); // Gradual stop before stopping completely
            
            yield return RotateRandomly();

            isMoving = true;
            yield return new WaitForSeconds(Random.Range(minMoveTime, maxMoveTime));

            isDecelerating = true;
            yield return new WaitUntil(() => currentSpeed == 0); // Gradual stop

            isMoving = false;
            yield return new WaitForSeconds(Random.Range(minStopTime, maxStopTime));
        }
    }

    private void TailWag()
    {
        float wagAngle = Mathf.Sin(Time.time * tailWagSpeed) * 7f;
        transform.RotateAround(transform.position, Vector3.forward, wagAngle * Time.deltaTime * 3f);
    }

    private IEnumerator RotateRandomly()
    {
        isTurning = true;

        // Randomly rotate to a new direction
        float newAngle = transform.eulerAngles.z + Random.Range(90f, 180f);
        targetRotation = Quaternion.Euler(0, 0, newAngle);

        while (isTurning)
            yield return null;
    }

    private void AvoidNearbyFish()
    {
        float dynamicAvoidanceRadius = Mathf.Lerp(0.8f, 1.5f, currentSpeed / moveSpeed);
        Collider2D[] nearbyColliders = Physics2D.OverlapCapsuleAll(
            transform.position, 
            new Vector2(dynamicAvoidanceRadius, dynamicAvoidanceRadius * 2f),
            CapsuleDirection2D.Vertical, 0f
        );

        Vector2 avoidanceForce = Vector2.zero;
        int numAvoiding = 0;

        foreach (var col in nearbyColliders)
        {
            if (col.gameObject == gameObject) continue; // Ignore self

            Vector2 awayFromObject = (Vector2)transform.position - col.ClosestPoint(transform.position);
            float distance = awayFromObject.magnitude;

            if (distance > 0)
            {
                avoidanceForce += awayFromObject.normalized / distance;
                numAvoiding++;
            }
        }

        if (numAvoiding > 0)
        {
            avoidanceForce /= numAvoiding;
            SteerAway(avoidanceForce * 1.5f); // Increase force slightly to help fish move away
        }
    }

    private void SteerAway(Vector2 avoidanceForce)
    {
        Vector2 newDirection = ((Vector2)(-transform.up) + avoidanceForce * avoidanceStrength).normalized;

        float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(0, 0, angle + 90f); // Rotate smoothly
        isTurning = true;
    }

   private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            StopAndTurn();

            // Get the closest point on the wall collider
            Vector2 closestPoint = collision.collider.ClosestPoint(transform.position);
            Vector2 wallNormal = (Vector2)transform.position - closestPoint;

            // Instead of teleporting, steer the fish away from the wall smoothly
            SteerAway(wallNormal.normalized * 2f); // Apply steering force
        }
    }

    private void StopAndTurn()
    {
        isMoving = false; 
        isDecelerating = true; 
        StartCoroutine(WaitAndTurn());
    }

    private IEnumerator WaitAndTurn()
    {
        yield return new WaitUntil(() => currentSpeed == 0); // Ensure fish fully stops

        float waitTime = Random.Range(1f, 3f); // Random wait before turning
        yield return new WaitForSeconds(waitTime);

        yield return RotateRandomly(); // Rotate to a new direction

        isMoving = true; // Resume movement
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
    }

    [System.Serializable]
    public struct SizeRange
    {
        [Range(0.01f, 1.1f)] public float min;
        [Range(0.015f, 1.1f)] public float max;
    }

    private void AssignFishSize()
    {
        SizeRange range = GetSizeRange(fishData.size);
        float finalHeight = Random.Range(range.min, range.max);
        float widthFactor = Random.Range(minWidthFactor, maxWidthFactor);

        transform.localScale = new Vector3(finalHeight * widthFactor, finalHeight, 1);
        Debug.Log($"{fishData.fishName} spawned as {fishData.size} with Length {finalHeight} (range: {range.min} - {range.max})");
    }

    private SizeRange GetSizeRange(FishData.Size size)
    {
        return size switch
        {
            FishData.Size.Tiny => tinySize,
            FishData.Size.Small => smallSize,
            FishData.Size.Medium => mediumSize,
            FishData.Size.Large => largeSize,
            FishData.Size.Huge => hugeSize,
            _ => mediumSize // Default fallback
        };
    }

    public void AssignBehaviorByRarity()
    {
        if (fishData == null)
        {
            Debug.LogWarning("FishData is missing on: " + gameObject.name);
            return;
        }

        switch (fishData.rarity)
        {
            case "★":
                moveSpeed = oneStarMoveSpeed;
                minMoveTime = oneStarMinMoveTime;
                maxMoveTime = oneStarMaxMoveTime;
                minStopTime = oneStarMinStopTime;
                maxStopTime = oneStarMaxStopTime;
                tailWagSpeed = oneStarTailWagSpeed;
                break;
            case "★★":
                moveSpeed = twoStarMoveSpeed;
                minMoveTime = twoStarMinMoveTime;
                maxMoveTime = twoStarMaxMoveTime;
                minStopTime = twoStarMinStopTime;
                maxStopTime = twoStarMaxStopTime;
                tailWagSpeed = twoStarTailWagSpeed;
                break;
            case "★★★":
                moveSpeed = threeStarMoveSpeed;
                minMoveTime = threeStarMinMoveTime;
                maxMoveTime = threeStarMaxMoveTime;
                minStopTime = threeStarMinStopTime;
                maxStopTime = threeStarMaxStopTime;
                tailWagSpeed = threeStarTailWagSpeed;
                break;
            case "★★★★":
                moveSpeed = fourStarMoveSpeed;
                minMoveTime = fourStarMinMoveTime;
                maxMoveTime = fourStarMaxMoveTime;
                minStopTime = fourStarMinStopTime;
                maxStopTime = fourStarMaxStopTime;
                tailWagSpeed = fourStarTailWagSpeed;
                break;
            case "★★★★★":
                moveSpeed = fiveStarMoveSpeed;
                minMoveTime = fiveStarMinMoveTime;
                maxMoveTime = fiveStarMaxMoveTime;
                minStopTime = fiveStarMinStopTime;
                maxStopTime = fiveStarMaxStopTime;
                tailWagSpeed = fiveStarTailWagSpeed;
                break;
        }
    }
}