using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Sewing;

namespace Sewing
{
    public class CustomerScene : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    private InputAction footRaiseAction;
    public bool movementComplete = false;
    public Transform waypoint;
    public float moveSpeed = 3f; // Speed of movement
    public CustomerSceneUIManager customerSceneUIManager;  // Assign in inspector wahoo

    public float fadeDuration = 1f;
    public float targetAlpha = 1f;

    private Color originalColor;
    private bool isFading = false;

    public GameObject speechObject;

     void Awake()
    {
        var actionMap = inputActions.FindActionMap("MotionTracking");
        footRaiseAction = actionMap.FindAction("FootRaise");
        footRaiseAction.performed += OnFootRaise;
    }
   void Start ()
    {
        StartCoroutine(MoveToWaypoint(waypoint));

        Renderer renderer = speechObject.GetComponent<Renderer>();
        originalColor = renderer.material.color;
        Color transparentColor = originalColor;
        transparentColor.a = 0f;
        renderer.material.color = transparentColor;
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
        if (movementComplete == true){
        ChangeScene("3. Scissors");
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
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
           
            yield return null; // Wait for the next frame
        }

        transform.position = targetPosition;
        Debug.Log("Person done moving!");
        FadeIn();
    }
    public void FadeIn()
    {
        isFading = true;
        StartCoroutine(Fade(0, targetAlpha, fadeDuration)); // Fade from 0 to targetAlpha
    
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float time = 0;
        float initialAlpha = originalColor.a;

        while (time < duration)
        {
            float t = time / duration;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, t);

            if (speechObject.GetComponent<Renderer>() != null) // For 3D objects
            {
                speechObject.GetComponent<Renderer>().material.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            }

            yield return null;
            time += Time.deltaTime;
        }
        movementComplete = true;
        customerSceneUIManager.ShowCompletionUI();
        isFading = false; // Set to false after the fade is complete
    }
}
}
