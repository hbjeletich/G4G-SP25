using UnityEngine;
using UnityEngine.SceneManagement;

public class nextScene : MonoBehaviour
{
    [Header("Scene to load")]
    public string sceneName;

    public void LoadScene()
    {
        // Save the next scene name before loading loading screen
        PlayerPrefs.SetString("GameLevel", sceneName);

        SceneManager.LoadScene("LoadingScene"); 
    }
}