using UnityEngine;
using UnityEngine.SceneManagement;
// InputSystem removed: SceneTransitionManager is invoked externally from MainController

namespace CameraSnap
{
    // Simple scene transition helper for the main menu.
  
    public class SceneTransitionManager : MonoBehaviour
    {
        [Tooltip("Name of the gameplay scene to load (must be in Build Settings)")]
        public string gameplaySceneName = "Gameplay";

    
        private bool isLoading = false;

        private void Awake()
        {
            //  just ensure default values exist

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

       
        public void StartGame()
        {
            if (isLoading) return;
            if (string.IsNullOrEmpty(gameplaySceneName))
            {
                return;
            }

            isLoading = true;
            SceneManager.LoadScene(gameplaySceneName);
        }
    }
}
