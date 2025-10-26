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

             spriteChild = GetComponentInChildren<SpriteRenderer>().transform;
    originalScale = spriteChild.localScale;
//  INITIAL FLIP WITH DEFAULT FACING RESPECTED
bool defaultFacesLeft = animalData != null && animalData.spriteFacesLeft;

// If sprite default matches movement - positive scale
// If sprite default is opposite movement - negative scale
int sign = (defaultFacesLeft == movingLeft) ? 1 : -1;

Vector3 s = originalScale;
s.x = Mathf.Abs(originalScale.x) * sign;
spriteChild.localScale = s;


          


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


            float direction = movingLeft ? 1f : -1f;
            transform.position += new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);




            // Turn around at patrol edges so it does not walk off too far
           if (Vector3.Distance(transform.position, spawnPoint) >= patrolDistance)
{
    movingLeft = !movingLeft;

    bool defaultFacesLeft = animalData != null && animalData.spriteFacesLeft;
    int sign = (defaultFacesLeft == movingLeft) ? 1 : -1;

    Vector3 s = originalScale;
    s.x = Mathf.Abs(originalScale.x) * sign;
    spriteChild.localScale = s;
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


    // Also flip the sprite immediately to match movement
    Vector3 scale = transform.localScale;
    scale.x = isMovingLeft ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
    transform.localScale = scale;
}


    }
}
