using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GolfStartingScreenManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    private InputAction footRaiseAction;
    public bool hasFootRaise = false;

    public Animator screenTransitionAnimator;
    private float transitionAnimLen = .5f;
    public AK.Wwise.Event TitleBGM;
    public AK.Wwise.Event StartTone;

    private void Awake()
    {
        hasFootRaise = false;

        var actionMap = inputActions.FindActionMap("Foot");
        footRaiseAction = actionMap.FindAction("FootRaised");

        //footRaiseAction.performed += startGolfGame;
        TitleBGM.Post(gameObject);
    }

    void Update(){
        if(Input.GetMouseButtonDown(0) && hasFootRaise == false){
            //simulated mouse click
            StartCoroutine(transitionToGolfGame());
            hasFootRaise = true;
        }
    }
    

    private void OnEnable()
    {
        footRaiseAction.Enable();
        footRaiseAction.performed += startGolfGame;
    }

    private void OnDisable()
    {
        footRaiseAction.Disable();
        footRaiseAction.performed -= startGolfGame;
    }

    public void startGolfGame(InputAction.CallbackContext ctx){
        if(hasFootRaise == false){
            StartCoroutine(transitionToGolfGame());
        }

        hasFootRaise = true;
    }

    IEnumerator transitionToGolfGame(){
        screenTransitionAnimator.CrossFadeInFixedTime("TransitionToGame", 0);
        TitleBGM.Stop(gameObject);
        Debug.Log("Title BGM stopped.");
        StartTone.Post(gameObject);
        yield return new WaitForSeconds(transitionAnimLen);


        StartCoroutine(LoadSceneCoroutine("GolfGame"));
    }

    IEnumerator LoadSceneCoroutine(string scene){
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone){
            yield return null;
        }
    }
}
