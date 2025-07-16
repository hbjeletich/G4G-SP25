using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Ship
{
    public class ShipTitleScreenButtons : MonoBehaviour
    {
        [Header("Button References")]
        public Button menuButton;
        public Button mainGameButton;

        [Header("Menu Screen References")]
        public GameObject menuPanel;
        public Button oneMinuteButton;
        public Button threeMinuteButton;
        public Button fiveMinuteButton;
        public Button infiniteButton;
        public Button backButton;

        private GameMode selectedGameMode = GameMode.OneMinute;

        private bool menuOpen = false;
        void Start() 
        {
            menuOpen = false;
            menuPanel.SetActive(false);
            
            // setup listeners
            menuButton.onClick.AddListener(MenuButtonPressed);
            mainGameButton.onClick.AddListener(MainGameButtonPressed);

            oneMinuteButton.onClick.AddListener(() => StartGameWithMode(GameMode.OneMinute));
            threeMinuteButton.onClick.AddListener(() => StartGameWithMode(GameMode.ThreeMinute));
            fiveMinuteButton.onClick.AddListener(() => StartGameWithMode(GameMode.FiveMinute));
            infiniteButton.onClick.AddListener(() => StartGameWithMode(GameMode.Infinite));

            backButton.onClick.AddListener(BackButtonPressed);
        }

        public void MenuButtonPressed()
        {
            menuOpen = true;
            menuPanel.SetActive(true);
        }   

        public void MainGameButtonPressed()
        {
            StartCoroutine(LoadMainGameScene());
        }

        public void BackButtonPressed()
        {
            menuOpen = false;
            menuPanel.SetActive(false);
        }

        private IEnumerator LoadMainGameScene()
        {
            // Load the main game scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("ShipMainLevel");
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        public static GameMode GetSelectedGameMode()
        {
            return (GameMode)PlayerPrefs.GetInt("SelectedGameMode", (int)GameMode.Infinite);
        }

        private void StartGameWithMode(GameMode mode)
        {
            selectedGameMode = mode;
            PlayerPrefs.SetInt("SelectedGameMode", (int)mode);
            PlayerPrefs.Save();

            Debug.Log($"StartGameWithMode called with: {mode}");
            Debug.Log($"Saved to PlayerPrefs as int: {(int)mode}");

            // Immediately test reading it back
            int readBack = PlayerPrefs.GetInt("SelectedGameMode", -999);
            Debug.Log($"Immediate readback test: {readBack}");
            Debug.Log($"Readback as enum: {(GameMode)readBack}");
        }
    }
}
