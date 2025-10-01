using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Sewing;

namespace Sewing
{
    public class IntroSceneTransition : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        private InputAction footRaiseAction;
        // Start is called before the first frame update
        void Awake()
        {
            var actionMap = inputActions.FindActionMap("Foot");
            footRaiseAction = actionMap.FindAction("FootRaised");
            footRaiseAction.performed += OnFootRaise;
            SoundManager.StartBGM();
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
            ChangeScene("2. Customer");
        }

        public void ChangeScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }

        void OnDestroy()
        {
            if (footRaiseAction != null)
            {
                footRaiseAction.performed -= OnFootRaise;
                footRaiseAction.Disable();
            }
        }
    }
}