using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace CameraSnap
{
    // Simple scene transition helper for the main menu.
  
    public class SceneTransitionManager : MonoBehaviour
    {
        [Tooltip("Name of the gameplay scene to load (must be in Build Settings)")]
        public string gameplaySceneName = "Gameplay";
        [Tooltip("UI progress bar to update during loading")]
        public UIProgressBar progressBar;
        private float progress = 0f;

        public float Progress => progress; // Expose progress for UI display

    
        private bool isLoading = false;

        private void Awake()
        {
            //  just ensure default values exist

            if(progressBar == null)
            {
                Debug.Log("SceneTransitionManager: No progress bar assigned, progress will be logged to console.");
            }
        }

       

        private void Update()
        {
            // Fallback key press
            if (!isLoading && Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
                Debug.Log("DebugKeyDetect-LoadingScene");
            }
        }

        // Public handler for external triggers (e.g. MainController). Keep simple: call from
        // other systems when a foot-raise is detected on the main menu.
        // Note: InputSystem-specific handlers have been removed to avoid needing direct
        // subscriptions in this class.
        public void OnFootRaisedExternal()
        {
            if (!isLoading) StartGame();
        }

        private void OnFootRaised(InputValue value)
        {
            if (!isLoading) StartGame();
        }
       
        public void StartGame()
        {
            if (isLoading) return;
            if (string.IsNullOrEmpty(gameplaySceneName))
            {
                return;
            }

            if(progressBar != null)
            {
                progressBar.Show();
            }

            isLoading = true;
            StartCoroutine(LoadSceneCoroutine());
        }

        private IEnumerator LoadSceneCoroutine()
        {
            yield return null;
            progress = 0f;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(gameplaySceneName);
            asyncOperation.allowSceneActivation = false;

            float displayedProgress = 0f;

            while (!asyncOperation.isDone)
            {
                float targetProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

                displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime * 0.5f);

                if (progressBar != null)
                    progressBar.SetProgress(displayedProgress);

                Debug.Log($"Loading progress: {displayedProgress * 100f:0}%");

                // Only activate when the bar has visually caught up to 100%
                if (asyncOperation.progress >= 0.9f && displayedProgress >= 0.99f)
                {
                    asyncOperation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }   
}
