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

        [Header("Viginette Settings")]
        public CanvasGroup viginetteCanvasGroup;
        public float fadeInDuration = 0.5f;
        public float fadeOutDuration = 0.5f;

        [Header("Timer UI")]
        public GameObject timerPanel;
        public TextMeshProUGUI timerText;

        [Header("Game Complete UI")]
        public GameObject gameCompletePanel;
        public TextMeshProUGUI finalScoreText;

        private void Awake()
        {
            HideGameComplete();
            scoreText.text = "Score: 0";
        }

        public void UpdateScore(int score)
        {
            scoreText.text = "Score: " + score;
        }

        public void RedViginette()
        {
            StartCoroutine(RedViginetteCoroutine());
        }

        private IEnumerator RedViginetteCoroutine()
        {
            // Fade in
            float timer = 0;
            while (timer <= fadeInDuration)
            {
                viginetteCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeInDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            viginetteCanvasGroup.alpha = 1;
            
            // Wait
            yield return new WaitForSeconds(0.5f);

            // Fade out
            timer = 0;
            while (timer <= fadeOutDuration)
            {
                viginetteCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeOutDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            viginetteCanvasGroup.alpha = 0;
        }

        public void ShowTimer(bool show)
        {
            if(timerPanel != null)
            {
                timerPanel.SetActive(show);
            }
        }

        public void UpdateTimer(float timeRemaining)
        {
            if(timerText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                // change color as time runs out
                if (timeRemaining <= 10f)
                {
                    timerText.color = Color.red;
                }
                else if (timeRemaining <= 30f)
                {
                    timerText.color = Color.yellow;
                } 
                else
                {
                    timerText.color = Color.white;
                }
            }
        }

        public void UpdateInfiniteTimer(float timeElapsed)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timeElapsed / 60f);
                int seconds = Mathf.FloorToInt(timeElapsed % 60f);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                timerText.color = Color.white;
            }
        }

        public void ShowGameComplete()
        {
            if (gameCompletePanel != null)
            {
                timerText.text = "";
                gameCompletePanel.SetActive(true);

                if (finalScoreText != null)
                {
                    finalScoreText.text = "Final Score: " + ShipGameManager.totalPoints.ToString();
                }
            }
        }

        public void HideGameComplete()
        {
            if(gameCompletePanel != null)
            {
                gameCompletePanel.SetActive(false);
            }
        }
    }
}
