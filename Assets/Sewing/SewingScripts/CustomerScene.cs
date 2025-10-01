using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Sewing;
using UnityEngine.Events;

namespace Sewing
{
    public class CustomerScene : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        private InputAction footRaiseAction;
        private bool movementComplete = false;
        private bool movementStarted = false;
        public Transform waypoint;
        public float moveSpeed = 3f; // Speed of movement
        //public CustomerSceneUIManager customerSceneUIManager;  // Assign in inspector wahoo

        public float fadeDuration = 1f;
        public float targetAlpha = 1f;
        private bool isFading = false;

        private Color originalColor;

        public GameObject speechObject;

        public UnityEvent OnFadeEvent;

        private Vector3 debugStartPosition;


        void Awake()
        {
            //Debug.Log($"[{Time.time:F2}] CustomerScene Awake called on {gameObject.name}");
            //Debug.Log($"Awake Stack Trace:\n{System.Environment.StackTrace}");

            var actionMap = inputActions.FindActionMap("Foot");
            footRaiseAction = actionMap.FindAction("FootRaised");
            footRaiseAction.performed += OnFootRaise;
        }
        void Start ()
        {
            //Debug.Log($"[{Time.time:F2}] CustomerScene Start called - Position: {transform.position}");
            if (!movementStarted)
            {
                movementStarted = true;
                StartCoroutine(MoveToWaypoint(waypoint));
            }

            SpriteRenderer renderer = speechObject.GetComponent<SpriteRenderer>();
            originalColor = renderer.color;
            Color transparentColor = originalColor;
            transparentColor.a = 0f;
            renderer.color = transparentColor;
        }
        void Update()
        {
            // Check if position suddenly changed
            //if (Vector3.Distance(transform.position, debugStartPosition) < 0.1f && movementStarted && !movementComplete)
            //{
            //    Debug.LogWarning($"[{Time.time:F2}] POSITION RESET DETECTED! Back to start position!");
            //}
        }

        private void OnEnable()
        {
            footRaiseAction.Enable();
        }

        private void OnDisable()
        {
            footRaiseAction.Disable();
        }

        private void OnFootRaise(InputAction.CallbackContext ctx)
        {

            if (movementComplete == true)
            {
                Debug.Log("Changing scene to Scissors");
                ChangeScene("3. Scissors");
            } else
            {
                Debug.Log("Movement not complete, ignoring foot raise");
            }
        }
        public void ChangeScene(string sceneName) 
        {
            SceneManager.LoadScene(sceneName);
        }


        private IEnumerator MoveToWaypoint(Transform waypoint)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = waypoint.position;
            float journeyLength = Vector3.Distance(startPosition, targetPosition);
            float elapsedTime = 0f; 

            while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                Vector3 positionBefore = transform.position;

                elapsedTime += Time.deltaTime;
                float fractionOfJourney = (elapsedTime * moveSpeed) / journeyLength;

                fractionOfJourney = Mathf.Clamp01(fractionOfJourney);

                transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

                if (Vector3.Distance(transform.position, positionBefore) > 1f)
                {
                    Debug.LogError($"[{Time.time:F2}] EXTERNAL POSITION CHANGE DETECTED! From: {positionBefore} To: {transform.position}");
                }

                yield return null;
            }
            transform.position = targetPosition;
            Debug.Log("Person done moving!");
            FadeIn();
        }
        public void FadeIn()
        {
            isFading = true;
            StartCoroutine(Fade(0, targetAlpha, fadeDuration));
    
        }

        IEnumerator Fade(float startAlpha, float endAlpha, float duration)
        {
            OnFadeEvent.Invoke();
            float time = 0;
            float initialAlpha = originalColor.a;

            while (time < duration)
            {
                float t = time / duration;
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, t);

                if (speechObject.GetComponent<SpriteRenderer>() != null) // For 3D objects
                {
                    speechObject.GetComponent<SpriteRenderer>().color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
                }

                yield return null;
                time += Time.deltaTime;
            }
            movementComplete = true;
            isFading = false; // Set to false after the fade is complete
        }

        void OnDestroy()
        {
            Debug.Log($"[{Time.time:F2}] CustomerScene OnDestroy called on {gameObject.name}");
            Debug.Log($"Destroy Stack Trace:\n{System.Environment.StackTrace}");

            if (footRaiseAction != null)
            {
                footRaiseAction.performed -= OnFootRaise;
                footRaiseAction.Disable();
            }
        }
    }
}
