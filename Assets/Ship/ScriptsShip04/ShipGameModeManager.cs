using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public enum GameMode
    {
        OneMinute,
        ThreeMinute,
        FiveMinute,
        Infinite
    }
    public class ShipGameModeManager : MonoBehaviour
    {
        [Header("Game Mode Setttings")]
        public GameMode currentGameMode = GameMode.Infinite;
        public float gameTimer = 0f;
        public bool gameStarted = false;
        public bool gameEnded = false;

        [Header("UI References")]
        public ShipUIManager shipUIManager;

        public static ShipGameModeManager instance;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            } else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            if(shipUIManager == null)
            {
                shipUIManager = FindObjectOfType<ShipUIManager>();
            }

            InitializeGameMode();
        }

        private void Update()
        {
            if(gameStarted && !gameEnded)
            {
                gameTimer += Time.deltaTime;
                if (IsTimedMode())
                {
                    UpdateTimerUI();

                    if (HasTimerExpired())
                    {
                        TriggerGameComplete();
                    }
                }
                else
                {
                    UpdateInfiniteTimerUI();
                }
            }
        }

        public void InitializeGameMode()
        {
            gameTimer = 0f;
            gameStarted = true;
            gameEnded = false;

            if(shipUIManager != null)
            {
                if (IsTimedMode())
                {
                    shipUIManager.UpdateTimer(GetGameModeTime());
                }
                else
                {
                    shipUIManager.UpdateInfiniteTimer(0f);
                }
            }
        }

        public void SetGameMode(GameMode mode)
        {
            currentGameMode = mode;
            InitializeGameMode();
        }

        public float GetGameModeTime()
        {
            switch(currentGameMode)
            {
                case GameMode.OneMinute:
                    return 60f;
                case GameMode.ThreeMinute:
                    return 180f;
                case GameMode.FiveMinute:
                    return 300f;
                case GameMode.Infinite:
                default:
                    return 0f;
            }
        }

        public bool IsTimedMode()
        {
            return currentGameMode != GameMode.Infinite;
        }

        private bool HasTimerExpired()
        {
            return gameTimer >= GetGameModeTime();
        }

        private void UpdateTimerUI()
        {
            if(shipUIManager != null)
            {
                float timeRemaining = GetGameModeTime() - gameTimer;
                shipUIManager.UpdateTimer(timeRemaining);
            }
        }

        private void UpdateInfiniteTimerUI()
        {
            if (shipUIManager != null)
            {
                shipUIManager.UpdateInfiniteTimer(gameTimer);
            }
        }

        private void TriggerGameComplete()
        {
            gameEnded = true;
            gameStarted = false;

            ShipController shipController = FindObjectOfType<ShipController>();
            if (shipController != null)
            {
                shipController.DisablePlayerController();
            }

            if (shipUIManager != null)
            {
                shipUIManager.ShowGameComplete();
            }

            StartCoroutine(ReturnToTitleScreen());
        }

        private IEnumerator ReturnToTitleScreen()
        {
            yield return new WaitForSeconds(3f);

            if (ShipGameManager.instance != null)
            {
                ShipGameManager.instance.TriggerGameOver();
            }
        }
    }
}
