using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishObject : MonoBehaviour
{
    [Header("Fish Data")]
    public FishData fishData;  // Assigned by FishSpawner

    [Header("Size Scaling (Global for All Fish)")]
    public float minScale = 0.05f; // Smallest possible fish
    public float maxScale = 0.12f;  // Largest possible fish
    public float minWidthFactor = 0.8f; // Thinnest fish
    public float maxWidthFactor = 1.5f; // Fattest fish

    [Header("Tail Wag Pivot Offset")]
    public Vector3 tailPivotOffset = new Vector3(0, -1f, 0); // Offset from fish center

    [Header("Fish Behavior Settings By Rarity")]
    [Header("★")]
    [Range(0, 10)] public float oneStarMoveSpeed = 1.5f;
    [Range(0, 10)] public float oneStarMinMoveTime = 3f;
    [Range(0, 10)] public float oneStarMaxMoveTime = 6f;
    [Range(0, 10)] public float oneStarMinStopTime = 2f;
    [Range(0, 10)] public float oneStarMaxStopTime = 4f;
    [Range(0, 10)] public float oneStarTailWagSpeed = 2f;

    [Header("★★")]
    [Range(0, 10)] public float twoStarMoveSpeed = 2f;
    [Range(0, 10)] public float twoStarMinMoveTime = 2.5f;
    [Range(0, 10)] public float twoStarMaxMoveTime = 5f;
    [Range(0, 10)] public float twoStarMinStopTime = 1.5f;
    [Range(0, 10)] public float twoStarMaxStopTime = 3f;
    [Range(0, 10)] public float twoStarTailWagSpeed = 2.5f;

    [Header("★★★")]
    [Range(0, 10)] public float threeStarMoveSpeed = 2.5f;
    [Range(0, 10)] public float threeStarMinMoveTime = 2f;
    [Range(0, 10)] public float threeStarMaxMoveTime = 4f;
    [Range(0, 10)] public float threeStarMinStopTime = 1f;
    [Range(0, 10)] public float threeStarMaxStopTime = 2.5f;
    [Range(0, 10)] public float threeStarTailWagSpeed = 3f;

    [Header("★★★★")]
    [Range(0, 10)] public float fourStarMoveSpeed = 3f;
    [Range(0, 10)] public float fourStarMinMoveTime = 1.5f;
    [Range(0, 10)] public float fourStarMaxMoveTime = 3.5f;
    [Range(0, 10)] public float fourStarMinStopTime = 1f;
    [Range(0, 10)] public float fourStarMaxStopTime = 2f;
    [Range(0, 10)] public float fourStarTailWagSpeed = 3.5f;

    [Header("★★★★★")]
    [Range(0, 10)] public float fiveStarMoveSpeed = 3.5f;
    [Range(0, 10)] public float fiveStarMinMoveTime = 1f;
    [Range(0, 10)] public float fiveStarMaxMoveTime = 3f;
    [Range(0, 10)] public float fiveStarMinStopTime = 0.5f;
    [Range(0, 10)] public float fiveStarMaxStopTime = 1.5f;
    [Range(0, 10)] public float fiveStarTailWagSpeed = 4f;

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
        AssignRandomSize();
        
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
        float wagAngle = Mathf.Sin(Time.time * tailWagSpeed) * 5f;
        transform.RotateAround(transform.position + tailPivotOffset, Vector3.forward, wagAngle * Time.deltaTime * 3f);
    }

    private IEnumerator RotateRandomly()
    {
        isTurning = true;

        // Pick a random new angle to turn to
        float newAngle = Random.Range(0f, 360f);
        targetRotation = Quaternion.Euler(0, 0, newAngle);

        // Wait until rotation completes before moving
        while (isTurning)
            yield return null;
    }

    private void AssignRandomSize()
    {
        float height = Random.Range(minScale, maxScale);
        float widthFactor = Random.Range(minWidthFactor, maxWidthFactor);
        transform.localScale = new Vector3(height * widthFactor, height, 1);

        // Adjust collider size proportionally
        if (capsuleCollider)
        {
            capsuleCollider.size = new Vector2(capsuleCollider.size.x * height * widthFactor, capsuleCollider.size.y * height);
        }
        Debug.Log($"{fishData.fishName} spawned with size Scale({height}h, {widthFactor}w)");
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