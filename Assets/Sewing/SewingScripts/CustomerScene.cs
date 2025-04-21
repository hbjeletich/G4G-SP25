using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Sewing;

namespace Sewing{
    public class CustomerScene : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    private InputAction footRaiseAction;
    private bool movementComplete = false;

     void Awake()
    {
        var actionMap = inputActions.FindActionMap("MotionTracking");
        footRaiseAction = actionMap.FindAction("FootRaise");
        footRaiseAction.performed += OnFootRaise;
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
    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
}