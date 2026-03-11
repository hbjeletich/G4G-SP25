using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class loadingAsync : MonoBehaviour
{
    [Header("Minimum Wait Time")]
    public float minWaitTime = 3f;

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        float timer = 0f;

        // Get saved scene name
        string nextSceneName = PlayerPrefs.GetString("GameLevel");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false;

        while (timer < minWaitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }
}