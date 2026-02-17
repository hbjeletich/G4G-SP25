using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Swimming
{
    public class CollectablesManager : MonoBehaviour
    {
        private static CollectablesManager _instance;
        [SerializeField] private Image[] shellImages;

        public GameObject gameOverText;
        private int shellsCollected = 0;
        [SerializeField] private int totalShells = 3;
        [SerializeField] private float gameOverTimer = 5f;

        public static CollectablesManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CollectablesManager>();
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject("CollectablesManager");
                        _instance = singletonObject.AddComponent<CollectablesManager>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            gameOverText.SetActive(false);
        }

        public void CollectShell(int index)
        {
            DataLogger.Instance.LogMinigameEvent("ScubaScavenge", $"Collected shell", $"{index} / {totalShells}");
            shellImages[index].color = Color.white;
            shellsCollected += 1;
            if(shellsCollected >= totalShells)
            {
                GameOver();
            }
        }

        public void GameOver()
        {
            gameOverText.SetActive(true);

            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.enabled = false;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<Rigidbody2D>().isKinematic = true;
                player.GetComponent<Animator>().enabled = false;
            }

            DataLogger.Instance.LogMinigameEvent("ScubaScavenge", "Game Over");

            StartCoroutine(BackToStart());
        }

        private IEnumerator BackToStart()
        {
            yield return new WaitForSeconds(gameOverTimer);

            SceneManager.LoadScene("GameSelectScene");
        }
    }
}
