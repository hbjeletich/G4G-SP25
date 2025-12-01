using UnityEngine;
using System.Collections.Generic;

namespace CameraSnap
{
    public class EndGameTrigger : MonoBehaviour
    {
        [Header("Settings")]
        public KeyCode restartKey = KeyCode.R;  // Press R to restart

        private bool hasEnded = false;

// When cart enters trigger, checks if it is the cart, ensures end isn't already triggered, 
//stoped the cart from moving, shows the end panel
        private void OnTriggerEnter(Collider other)
        {
            // Detect the cart reaching the end
            CartController cart = other.GetComponentInParent<CartController>();
            if (cart != null && !hasEnded)
            {
                hasEnded = true;
                cart.StopCart();

                // Inline end summary behavior: populate end panel slots, reveal captured animals, show panel and pause.
                if (GameManager.Instance == null || UIManager.Instance == null) return;

                var allAnimals = GameManager.Instance.GetAllAnimals();
                var captured = GameManager.Instance.GetCapturedAnimals();

                UIManager.Instance.SetEndPanelAnimals(new System.Collections.Generic.List<AnimalData>(allAnimals));

                if (captured != null)
                {
                    foreach (var name in captured)
                    {
                        UIManager.Instance.RevealEndPanelTarget(name);
                    }
                }

                UIManager.Instance.ShowEndPanel();
                Time.timeScale = 0f;
            }
        }
        // end trigger handling inlined above
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
