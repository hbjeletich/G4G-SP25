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

        void Awake()
        {
            var motionMap = inputActions.FindActionMap("MotionTracking");
            raiseFootAction = motionMap.FindAction("FootRaise");

            if (liftLegText != null)
            {
                liftLegText.SetActive(false);
            }
        }

        private void OnEnable()
        {
            raiseFootAction.Enable();

            raiseFootAction.performed += _ => StartGame();
        }

        private void OnDisable()
        {
            raiseFootAction.Disable();

            raiseFootAction.performed -= _ => StartGame();
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

        private void StartGame()
        {
            SceneManager.LoadScene("SwimmingScene");
        }
    }
}
