using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace Ship
{
    public class ShipUIManager : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI scoreText;
        public Image healthBarFill;

        private void Awake()
        {
            scoreText.text = "Score: 0";
        }

        public void UpdateScore(int score)
        {
            scoreText.text = "Score: " + score;
        }

        public void UpdateHealth(float health)
        {
            StartCoroutine(UpdateHealthBar(health));
            
            // float normalized = Mathf.Clamp01(health / 4f);
            // healthBarFill.fillAmount = normalized;
        }

        private IEnumerator UpdateHealthBar(float targetHealth)
        {
            float currentFill = healthBarFill.fillAmount;
            float elapsedTime = 0f;
            float duration = 0.5f;

            while (elapsedTime < duration)
            {
            elapsedTime += Time.deltaTime;
            healthBarFill.fillAmount = Mathf.Lerp(currentFill, Mathf.Clamp01(targetHealth / 4f), elapsedTime / duration);
            yield return null;
            }

            healthBarFill.fillAmount = Mathf.Clamp01(targetHealth / 4f);
        }
    }
}
