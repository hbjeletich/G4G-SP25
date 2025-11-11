using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Constellation
{
    public class StarPlacementManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject congratsPanel;
        [SerializeField] private float delayBeforeLoad = 3f;

        [Header("Scene Transition")]
        [SerializeField] private string nextSceneName;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        private StarScript[] stars;
        private bool levelCompleted = false;

        void Start()
        {
            // Hide congrats panel at start
            if (congratsPanel != null)
                congratsPanel.SetActive(false);

            // Grab all stars in the scene
            stars = FindObjectsOfType<StarScript>(true);
            if (stars.Length == 0)
            {
                Debug.LogError("No StarScript objects found in the scene!");
                return;
            }

            if (debugMode)
                Debug.Log($"Tracking {stars.Length} stars for placement progress.");
        }

        void Update()
        {
            if (levelCompleted || stars == null || stars.Length == 0)
                return;

            int placedCount = 0;

            // Check all stars’ home status
            foreach (StarScript star in stars)
            {
                if (star == null) continue;
                if (IsStarHome(star))
                    placedCount++;
            }

            if (debugMode && Input.GetKeyDown(KeyCode.P))
                Debug.Log($"Progress: {placedCount}/{stars.Length} stars placed");

            if (placedCount == stars.Length)
                OnLevelComplete();
        }

        private bool IsStarHome(StarScript star)
        {
            // Directly access its foundHome field
            // If you make it private, add a public getter instead.
            var field = typeof(StarScript).GetField("foundHome", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field == null)
            {
                Debug.LogError("StarScript missing foundHome field!");
                return false;
            }

            return (bool)field.GetValue(star);
        }

        private void OnLevelComplete()
        {
            if (congratsPanel == null)
{
    Debug.LogError("❌ Congrats panel is NOT assigned in the inspector!");
    return;
}
else
{
    Debug.Log($"✅ Congrats panel found: {congratsPanel.name}, activeSelf={congratsPanel.activeSelf}, inHierarchy={congratsPanel.activeInHierarchy}");
}

            levelCompleted = true;
            Debug.Log("🌟 All stars have found their homes!");

            if (congratsPanel != null)
{
    congratsPanel.SetActive(true); // 👈 activate it first so coroutine can run

    var fade = congratsPanel.GetComponent<SmoothPanelTransition>();
    if (fade != null)
        fade.Show();
}



            if (!string.IsNullOrEmpty(nextSceneName))
                StartCoroutine(LoadNextSceneAfterDelay());
            else if (debugMode)
                Debug.LogWarning("Next scene name not specified — staying in current scene.");
        }

        private IEnumerator LoadNextSceneAfterDelay()
        {
            yield return new WaitForSeconds(delayBeforeLoad);
            SceneManager.LoadScene(nextSceneName);
        }

        [ContextMenu("Force Complete Level")]
        private void ForceComplete()
        {
            OnLevelComplete();
        }
    }
}
