using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace CameraSnap
{
    public class EndGameTrigger : MonoBehaviour
    {
        [Header("UI Settings")]
        public TMPro.TMP_Text endGameText; 
        public GameObject endGamePanel;     
        public KeyCode restartKey = KeyCode.R;  // Press R to restart

        private bool hasEnded = false;

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

        private void ShowEndSummary()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("[EndGameTrigger] No GameManager found!");
                return;
            }

            List<AnimalData> allAnimals = GameManager.Instance.GetAllAnimals();
            HashSet<string> captured = GameManager.Instance.GetCapturedAnimals();

            StringBuilder summary = new StringBuilder();
            summary.AppendLine("<b> Photo Summary</b>\n");

            summary.AppendLine("<color=green><b>Captured:</b></color>");
            int capturedCount = 0;
            foreach (var animal in allAnimals)
            {
                if (captured.Contains(animal.animalName))
                {
                    summary.AppendLine($" {animal.animalName}");
                    capturedCount++;
                }
            }

            summary.AppendLine("\n<color=red><b>Missed:</b></color>");
            foreach (var animal in allAnimals)
            {
                if (!captured.Contains(animal.animalName))
                    summary.AppendLine($" {animal.animalName}");
            }

            summary.AppendLine($"\n<b>Total Captured:</b> {capturedCount}/{allAnimals.Count}");
summary.AppendLine("<b> End game. Press R or raise foot to restart</b>\n");

            if (endGameText != null)
                endGameText.text = summary.ToString();

            if (endGamePanel != null)
                endGamePanel.SetActive(true);
                 Time.timeScale = 0f;

            Debug.Log("[EndGameTrigger] End summary displayed. Press R to restart.");
        }

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
