using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace CameraSnap
{
    public class EndGameTrigger : MonoBehaviour
    {
        [Header("UI Settings")]
        public TMPro.TMP_Text endGameText;  //UI text box shows summary
        public GameObject endGamePanel;     //panel shows when game ends
        public KeyCode restartKey = KeyCode.R;  // Press R to restart

        private bool hasEnded = false;

// When cart enters trigger, checks if it is the cart, ensures end isn't already triggered, 
//stoped the cart from moving, shows the end summary
        private void OnTriggerEnter(Collider other)
        {
            // Detect the cart reaching the end
            CartController cart = other.GetComponentInParent<CartController>();
            if (cart != null && !hasEnded)
            {
                hasEnded = true;
                cart.StopCart();
                ShowEndSummary();
            }
        }
//generates list of captured and missed animals. Gets set of animals that player captured, builds
//UI summary using string formatting..
        private void ShowEndSummary()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("[EndGameTrigger] No GameManager found!");
                return;
            }

            var allAnimals = GameManager.Instance.GetAllAnimals();
            var captured = GameManager.Instance.GetCapturedAnimals();

            if (UIManager.Instance == null)
            {
                Debug.LogError("[EndGameTrigger] UIManager not found in scene. Add a UIManager GameObject and assign UI references.");
                return;
            }

            UIManager.Instance.ShowEndSummary(allAnimals, captured);
            Debug.Log("[EndGameTrigger] End summary displayed via UIManager.");
        }
//If end has been triggered and player presses R, it resumes the time and reloads the same scene 
        private void Update()
        {
            if (hasEnded && Input.GetKeyDown(restartKey))
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
        }
    }
}
