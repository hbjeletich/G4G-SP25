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
            }
        }

        // Public handler so the main-menu input system can call this directly
        // or subscribe to it as an event target.
        public void OnFootRaised(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && !isLoading)
            {
                StartGame();
            }
        }

        
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
