using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Swimming
{
    public class StartScreenManager : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private bool debugMode = false;

        private InputAction raiseFootAction;

        [SerializeField] private GameObject liftLegText;
        [SerializeField] private float liftLegDelay = 3f;
        private float timer = 0;
        private bool gameStarted = false;

        void Awake()
        {
            var motionMap = inputActions.FindActionMap("Foot");
            raiseFootAction = motionMap.FindAction("FootRaised");

            if (liftLegText != null)
            {
                liftLegText.SetActive(false);
            }
        }

        private void OnEnable()
        {
            raiseFootAction.Enable();

            raiseFootAction.performed += OnFootRaised;
        }

        private void OnDisable()
        {
            raiseFootAction.Disable();

            raiseFootAction.performed -= OnFootRaised;
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (timer > liftLegDelay && liftLegText != null)
            {
                liftLegText.SetActive(true);
            }

            if(debugMode)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    StartGame();
                }
            }
        }

        void OnFootRaised(InputAction.CallbackContext ctx)
        {
            StartGame();
        }

        private void StartGame()
        {
            if (!gameStarted)
            {
                gameStarted = true;
                raiseFootAction.performed -= OnFootRaised;
                SceneManager.LoadScene("SwimmingScene");
            }
        }
    }
}
