using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
public class ShipTitleScreenButtons : MonoBehaviour
{
    [Header("Button References")]
    public GameObject tutorialButton;
    public GameObject mainGameButton;
    void Start() {}

    public void TutorialButtonPressed()
    {
        StartCoroutine(LoadTutorialScene());
    }   

    public void MainGameButtonPressed()
    {
        StartCoroutine(LoadMainGameScene());
    }

    private IEnumerator LoadTutorialScene()
    {
        // Load the tutorial scene
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("ShipTutorial");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadMainGameScene()
    {
        // Load the main game scene
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("ShipMainGame");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
}
